using HRVault.Application.Authentication.DTOs;
using HRVault.Application.Authentication.Interfaces;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _refreshTokenService.HashToken(
                request.RefreshToken);

        var storedToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (storedToken is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        if (!storedToken.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Refresh token is expired or revoked.");
        }

        var user = await _userRepository.GetByIdAsync(
            storedToken.UserId,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User is inactive or no longer available.");
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

        var newAccessToken =
            _jwtTokenGenerator.GenerateToken(jwtUser);

        var newRefreshToken =
            _refreshTokenService.GenerateToken();

        var newRefreshTokenHash =
            _refreshTokenService.HashToken(
                newRefreshToken);

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByTokenHash =
            newRefreshTokenHash;

        await _refreshTokenRepository.UpdateAsync(
            storedToken,
            cancellationToken);

        var newRefreshTokenEntity =
			new HRVault.Domain.Entities.RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

        await _refreshTokenRepository.AddAsync(
            newRefreshTokenEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}