using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeContact;

public class UpdateEmployeeContactCommandHandler
    : IRequestHandler<UpdateEmployeeContactCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeContactRepository _contactRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeContactCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeContactRepository contactRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _contactRepository = contactRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateEmployeeContactCommand request,
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
            await _contactRepository.GetByIdAndEmployeeIdAsync(
                request.ContactId,
                request.EmployeeId,
                cancellationToken);

        if (contact is null)
        {
            throw new NotFoundException(
                "Contact not found.");
        }

        contact.Type = request.Type;
        contact.Value = request.Value;
        contact.IsPrimary = request.IsPrimary;
        contact.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}