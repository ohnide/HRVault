using MediatR;

namespace HRVault.Application.Absences.Commands.DeleteEmployeeAbsence;

public record DeleteEmployeeAbsenceCommand(
    Guid Id
) : IRequest;