using MediatR;

namespace HRVault.Application.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<Guid>;