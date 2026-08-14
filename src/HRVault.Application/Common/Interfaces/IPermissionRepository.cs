using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<List<Permission>> GetAllActiveAsync(
        CancellationToken cancellationToken = default);
}