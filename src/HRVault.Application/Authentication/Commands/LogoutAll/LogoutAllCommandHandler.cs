using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Authentication.Commands.LogoutAll;

public class LogoutAllCommandHandler
    : IRequestHandler<LogoutAllCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutAllCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var tokens =
            await _refreshTokenRepository
                .GetActiveByUserIdAsync(
                    _currentUser.UserId.Value,
                    cancellationToken);

        if (tokens.Count == 0)
            return;

        var now = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            token.RevokedAt = now;

            await _refreshTokenRepository.UpdateAsync(
                token,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}