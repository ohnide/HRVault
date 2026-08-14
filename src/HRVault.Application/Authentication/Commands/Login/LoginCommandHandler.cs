using HRVault.Application.Authentication.DTOs;
using HRVault.Application.Authentication.Interfaces;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
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

        var accessToken =
            _jwtTokenGenerator.GenerateToken(jwtUser);

        var refreshToken =
            _refreshTokenService.GenerateToken();

        var refreshTokenHash =
            _refreshTokenService.HashToken(refreshToken);

        var refreshTokenEntity =
			new HRVault.Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(
            refreshTokenEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}