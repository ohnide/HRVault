using HRVault.Application.Authentication.DTOs;
using MediatR;

namespace HRVault.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponse>;