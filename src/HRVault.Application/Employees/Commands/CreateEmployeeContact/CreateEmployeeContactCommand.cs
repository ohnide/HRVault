using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeContact;

public record CreateEmployeeContactCommand(
    Guid EmployeeId,
    ContactType Type,
    string Value,
    bool IsPrimary,
    string? Notes
) : IRequest<Guid>;