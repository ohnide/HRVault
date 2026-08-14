using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Authentication.Commands.Logout;

public class LogoutCommandHandler
    : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            _refreshTokenService.HashToken(
                request.RefreshToken);

        var storedToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        // Logout idempotente:
        // token inexistente ou já revogado não revela informação.
        if (storedToken is null ||
            storedToken.RevokedAt.HasValue)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateAsync(
            storedToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}