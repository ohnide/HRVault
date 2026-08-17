using MediatR;

namespace HRVault.Application.Absences.Commands.UpdateAbsenceType;

public record UpdateAbsenceTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    bool RequiresApproval,
    bool RequiresDocument,
    bool IsPaid
) : IRequest;