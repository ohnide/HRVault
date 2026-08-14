using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeAddress;

public record CreateEmployeeAddressCommand(
    Guid EmployeeId,
    string Type,
    string Street,
    string PostalCode,
    string City,
    string? District,
    string Country
) : IRequest<Guid>;