using MediatR;

namespace HRVault.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;