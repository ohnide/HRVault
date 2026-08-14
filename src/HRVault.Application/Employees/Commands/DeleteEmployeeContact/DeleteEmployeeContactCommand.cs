using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeContact;

public record DeleteEmployeeContactCommand(
    Guid EmployeeId,
    Guid ContactId
) : IRequest;