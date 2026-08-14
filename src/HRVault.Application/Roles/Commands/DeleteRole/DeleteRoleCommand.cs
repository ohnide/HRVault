using MediatR;

namespace HRVault.Application.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest;