using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeAddress;

public class DeleteEmployeeAddressCommandHandler
    : IRequestHandler<DeleteEmployeeAddressCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeAddressCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeAddressRepository addressRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _addressRepository = addressRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteEmployeeAddressCommand request,
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

        var address =
            await _addressRepository.GetByIdAndEmployeeIdAsync(
                request.AddressId,
                request.EmployeeId,
                cancellationToken);

        if (address is null)
        {
            throw new NotFoundException(
                "Address not found.");
        }

        await _addressRepository.DeleteAsync(
            address,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}