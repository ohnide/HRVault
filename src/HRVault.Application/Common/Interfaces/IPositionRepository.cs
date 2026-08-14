using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IPositionRepository : IRepository<Position>
{
    Task<List<Position>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Position?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);
}