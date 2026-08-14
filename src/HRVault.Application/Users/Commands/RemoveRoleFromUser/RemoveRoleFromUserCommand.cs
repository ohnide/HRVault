using MediatR;

namespace HRVault.Application.Users.Commands.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;