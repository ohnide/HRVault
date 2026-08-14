using HRVault.Application.Authentication.DTOs;
using HRVault.Application.Authentication.Interfaces;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Email ou password inválidos.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive.");
        }

        if (!_passwordHasher.Verify(
                request.Password,
                user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Email ou password inválidos.");
        }

        var jwtUser = new JwtUser
        {
            UserId = user.Id,
            CompanyId = user.CompanyId,
            Email = user.Email,
            FullName = user.Name,
            IsAdministrator = user.IsAdministrator,
            IsPlatformAdministrator =
                user.IsPlatformAdministrator,
            Roles = new List<string>()
        };

        var token =
            _jwtTokenGenerator.GenerateToken(jwtUser);

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = string.Empty,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}