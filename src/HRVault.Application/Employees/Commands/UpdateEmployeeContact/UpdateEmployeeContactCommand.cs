using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeContact;

public record UpdateEmployeeContactCommand(
    Guid EmployeeId,
    Guid ContactId,
    ContactType Type,
    string Value,
    bool IsPrimary,
    string? Notes
) : IRequest;