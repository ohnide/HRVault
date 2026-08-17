using MediatR;

namespace HRVault.Application.Absences.Commands.DeleteAbsenceType;

public record DeleteAbsenceTypeCommand(
    Guid Id
) : IRequest;