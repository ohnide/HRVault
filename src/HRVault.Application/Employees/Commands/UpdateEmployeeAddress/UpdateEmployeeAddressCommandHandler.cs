using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeAddress;

public class UpdateEmployeeAddressCommandHandler
    : IRequestHandler<UpdateEmployeeAddressCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeAddressCommandHandler(
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
        UpdateEmployeeAddressCommand request,
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

        address.Type = request.Type;
        address.Street = request.Street;
        address.PostalCode = request.PostalCode;
        address.City = request.City;
        address.District = request.District;
        address.Country = request.Country;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}