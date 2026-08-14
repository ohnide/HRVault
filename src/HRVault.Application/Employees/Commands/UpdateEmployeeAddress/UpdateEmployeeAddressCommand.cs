using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeAddress;

public record UpdateEmployeeAddressCommand(
    Guid EmployeeId,
    Guid AddressId,
    string Type,
    string Street,
    string PostalCode,
    string City,
    string? District,
    string Country
) : IRequest;