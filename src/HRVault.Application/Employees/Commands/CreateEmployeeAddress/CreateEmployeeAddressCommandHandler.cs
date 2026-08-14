using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeAddress;

public class CreateEmployeeAddressCommandHandler
    : IRequestHandler<CreateEmployeeAddressCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeAddressCommandHandler(
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

    public async Task<Guid> Handle(
        CreateEmployeeAddressCommand request,
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

        var address = new EmployeeAddress
        {
            EmployeeId = request.EmployeeId,
            Type = request.Type,
            Street = request.Street,
            PostalCode = request.PostalCode,
            City = request.City,
            District = request.District,
            Country = request.Country
        };

        await _addressRepository.AddAsync(
            address,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return address.Id;
    }
}