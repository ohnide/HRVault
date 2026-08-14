using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeEmergencyContact;

public record DeleteEmployeeEmergencyContactCommand(
    Guid EmployeeId
) : IRequest;