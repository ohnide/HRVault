using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Absences.Commands.CreateEmployeeAbsence;

public class CreateEmployeeAbsenceCommandHandler
    : IRequestHandler<CreateEmployeeAbsenceCommand, Guid>
{
    private readonly IEmployeeAbsenceRepository _absenceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAbsenceTypeRepository _absenceTypeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeAbsenceCommandHandler(
        IEmployeeAbsenceRepository absenceRepository,
        IEmployeeRepository employeeRepository,
        IAbsenceTypeRepository absenceTypeRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _absenceRepository = absenceRepository;
        _employeeRepository = employeeRepository;
        _absenceTypeRepository = absenceTypeRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateEmployeeAbsenceCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var companyId =
            _currentUser.CompanyId.Value;

        if (request.EndDateTime <= request.StartDateTime)
        {
            throw new BusinessRuleException(
                "End date must be greater than start date.");
        }

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var absenceType =
            await _absenceTypeRepository.GetByIdAndCompanyAsync(
                request.AbsenceTypeId,
                companyId,
                cancellationToken);

        if (absenceType is null)
        {
            throw new NotFoundException(
                "Absence type not found.");
        }

        var hasOverlap =
            await _absenceRepository.HasOverlapAsync(
                request.EmployeeId,
                request.StartDateTime,
                request.EndDateTime,
                companyId,
                cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            throw new ConflictException(
                "The employee already has an overlapping absence.");
        }

        var status =
            absenceType.RequiresApproval
                ? AbsenceStatus.Pending
                : AbsenceStatus.Approved;

        var absence =
            new EmployeeAbsence
            {
                CompanyId = companyId,
                EmployeeId = request.EmployeeId,
                AbsenceTypeId = request.AbsenceTypeId,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                Status = status,
                Reason =
                    string.IsNullOrWhiteSpace(request.Reason)
                        ? null
                        : request.Reason.Trim(),
                Notes =
                    string.IsNullOrWhiteSpace(request.Notes)
                        ? null
                        : request.Notes.Trim()
            };

        await _absenceRepository.AddAsync(
            absence,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return absence.Id;
    }
}