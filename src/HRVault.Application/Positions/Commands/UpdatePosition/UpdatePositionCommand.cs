using MediatR;

namespace HRVault.Application.Positions.Commands.UpdatePosition;

public record UpdatePositionCommand(
    Guid Id,
    Guid CompanyId,
    string Code,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Guid>;