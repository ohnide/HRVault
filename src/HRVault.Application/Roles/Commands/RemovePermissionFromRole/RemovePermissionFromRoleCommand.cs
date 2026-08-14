using MediatR;

namespace HRVault.Application.Roles.Commands.RemovePermissionFromRole;

public record RemovePermissionFromRoleCommand(
    Guid RoleId,
    Guid PermissionId) : IRequest;