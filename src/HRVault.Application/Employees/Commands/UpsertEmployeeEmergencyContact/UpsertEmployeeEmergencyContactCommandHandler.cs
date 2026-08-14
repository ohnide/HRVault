using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpsertEmployeeEmergencyContact;

public class UpsertEmployeeEmergencyContactCommandHandler
    : IRequestHandler<UpsertEmployeeEmergencyContactCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeEmergencyContactRepository _contactRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertEmployeeEmergencyContactCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeEmergencyContactRepository contactRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _contactRepository = contactRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpsertEmployeeEmergencyContactCommand request,
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

        var contact =
            await _contactRepository.GetByEmployeeIdAsync(
                request.EmployeeId,
                cancellationToken);

        if (contact is null)
        {
            contact = new EmployeeEmergencyContact
            {
                EmployeeId = request.EmployeeId
            };

            await _contactRepository.AddAsync(
                contact,
                cancellationToken);
        }

        contact.Name = request.Name;
        contact.Relationship = request.Relationship;
        contact.Phone = request.Phone;
        contact.Email = request.Email;
        contact.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}