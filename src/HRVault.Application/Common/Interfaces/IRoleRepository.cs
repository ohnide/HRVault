using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<List<Role>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeRoleId = null,
        CancellationToken cancellationToken = default);
}