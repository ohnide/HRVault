using MediatR;

namespace HRVault.Application.Absences.Commands.ApproveEmployeeAbsence;

public record ApproveEmployeeAbsenceCommand(
    Guid Id
) : IRequest;