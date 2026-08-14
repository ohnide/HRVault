using HRVault.Application.Authentication.DTOs;
using MediatR;

namespace HRVault.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<LoginResponse>;