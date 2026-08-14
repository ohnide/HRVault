using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpsertEmployeeProfile;

public class UpsertEmployeeProfileCommandHandler
    : IRequestHandler<UpsertEmployeeProfileCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeProfileRepository _profileRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertEmployeeProfileCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeProfileRepository profileRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _profileRepository = profileRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpsertEmployeeProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var profile =
            await _profileRepository.GetByEmployeeIdAsync(
                request.EmployeeId,
                cancellationToken);

        if (profile is null)
        {
            profile = new EmployeeProfile
            {
                EmployeeId = request.EmployeeId
            };

            await _profileRepository.AddAsync(
                profile,
                cancellationToken);
        }

        profile.BirthDate = request.BirthDate;
        profile.Gender = request.Gender;
        profile.MaritalStatus = request.MaritalStatus;
        profile.Nationality = request.Nationality;
        profile.DocumentType = request.DocumentType;
        profile.DocumentNumber = request.DocumentNumber;
        profile.TaxNumber = request.TaxNumber;
        profile.SocialSecurityNumber =
            request.SocialSecurityNumber;
        profile.SnsNumber = request.SnsNumber;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}