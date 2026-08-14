using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeContact;

public class CreateEmployeeContactCommandHandler
    : IRequestHandler<CreateEmployeeContactCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeContactRepository _contactRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeContactCommandHandler(
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

    public async Task<Guid> Handle(
        CreateEmployeeContactCommand request,
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

        var contact = new EmployeeContact
        {
            EmployeeId = request.EmployeeId,
            Type = request.Type,
            Value = request.Value,
            IsPrimary = request.IsPrimary,
            Notes = request.Notes
        };

        await _contactRepository.AddAsync(
            contact,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return contact.Id;
    }
}