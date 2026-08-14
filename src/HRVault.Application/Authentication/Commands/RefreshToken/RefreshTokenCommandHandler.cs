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
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
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

        if (storedToken.IsRevoked)
		{
			if (!string.IsNullOrWhiteSpace(
					storedToken.ReplacedByTokenHash))
			{
				var activeTokens =
					await _refreshTokenRepository
						.GetActiveByUserIdAsync(
							storedToken.UserId,
							cancellationToken);

				var now = DateTime.UtcNow;

				foreach (var activeToken in activeTokens)
				{
					activeToken.RevokedAt = now;

					await _refreshTokenRepository.UpdateAsync(
						activeToken,
						cancellationToken);
				}

				await _unitOfWork.SaveChangesAsync(
					cancellationToken);
			}

			throw new UnauthorizedAccessException(
				"Refresh token has been revoked.");
		}

		if (storedToken.IsExpired)
		{
			throw new UnauthorizedAccessException(
				"Refresh token has expired.");
		}

        var user = await _userRepository.GetByIdAsync(
            storedToken.UserId,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User is inactive or no longer available.");
        }

        var roles =
            await _userRoleRepository.GetRolesByUserIdAsync(
                user.Id,
                cancellationToken);

        var jwtUser = new JwtUser
        {
            UserId = user.Id,
            CompanyId = user.CompanyId,
            Email = user.Email,
            FullName = user.Name,
            IsAdministrator = user.IsAdministrator,
            IsPlatformAdministrator =
                user.IsPlatformAdministrator,
            Roles = roles
                .Select(x => x.Name)
                .ToList()
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