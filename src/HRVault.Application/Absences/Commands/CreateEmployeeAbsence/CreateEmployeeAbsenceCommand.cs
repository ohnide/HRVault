using MediatR;

namespace HRVault.Application.Absences.Commands.CreateEmployeeAbsence;

public record CreateEmployeeAbsenceCommand(
    Guid EmployeeId,
    Guid AbsenceTypeId,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string? Reason,
    string? Notes
) : IRequest<Guid>;