using MediatR;

namespace HRVault.Application.Users.Commands.AssignRoleToUser;

public record AssignRoleToUserCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;