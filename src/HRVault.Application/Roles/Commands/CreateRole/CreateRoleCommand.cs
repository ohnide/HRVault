using MediatR;

namespace HRVault.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<Guid>;