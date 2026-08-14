using MediatR;

namespace HRVault.Application.Employees.Commands.UpsertEmployeeEmergencyContact;

public record UpsertEmployeeEmergencyContactCommand(
    Guid EmployeeId,
    string Name,
    string Relationship,
    string Phone,
    string? Email,
    string? Notes
) : IRequest;