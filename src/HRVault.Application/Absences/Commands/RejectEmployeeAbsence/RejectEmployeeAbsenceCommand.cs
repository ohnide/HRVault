using MediatR;

namespace HRVault.Application.Absences.Commands.RejectEmployeeAbsence;

public record RejectEmployeeAbsenceCommand(
    Guid Id
) : IRequest;