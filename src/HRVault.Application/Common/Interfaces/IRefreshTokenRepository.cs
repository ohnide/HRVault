using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task<List<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}