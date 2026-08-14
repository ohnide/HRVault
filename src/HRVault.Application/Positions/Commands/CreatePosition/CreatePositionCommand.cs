using MediatR;

namespace HRVault.Application.Positions.Commands.CreatePosition;

public record CreatePositionCommand(
    string Code,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Guid>;