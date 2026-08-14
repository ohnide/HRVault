using MediatR;

namespace HRVault.Application.Authentication.Commands.Logout;

public record LogoutCommand(
    string RefreshToken
) : IRequest;