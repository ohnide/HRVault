using MediatR;

namespace HRVault.Application.Absences.Commands.CreateAbsenceType;

public record CreateAbsenceTypeCommand(
    string Name,
    string? Description,
    bool RequiresApproval,
    bool RequiresDocument,
    bool IsPaid
) : IRequest<Guid>;