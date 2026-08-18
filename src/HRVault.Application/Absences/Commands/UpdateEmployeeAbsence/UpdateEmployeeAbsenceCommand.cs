using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Absences.Commands.UpdateEmployeeAbsence;

public record UpdateEmployeeAbsenceCommand(
    Guid Id,
    Guid EmployeeId,
    Guid AbsenceTypeId,
    DateTime StartDateTime,
    DateTime EndDateTime,
    AbsenceStatus Status,
    string? Reason,
    string? Notes
) : IRequest;