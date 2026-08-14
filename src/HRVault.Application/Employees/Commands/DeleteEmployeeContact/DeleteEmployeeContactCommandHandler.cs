using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeContact;

public class DeleteEmployeeContactCommandHandler
    : IRequestHandler<DeleteEmployeeContactCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeContactRepository _contactRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeContactCommandHandler(
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
        DeleteEmployeeContactCommand request,
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

        await _contactRepository.DeleteAsync(
            contact,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}