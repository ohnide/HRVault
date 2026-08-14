using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeAddress;

public record DeleteEmployeeAddressCommand(
    Guid EmployeeId,
    Guid AddressId
) : IRequest;