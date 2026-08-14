using MediatR;

namespace HRVault.Application.Roles.Commands.AssignPermissionToRole;

public record AssignPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId) : IRequest;