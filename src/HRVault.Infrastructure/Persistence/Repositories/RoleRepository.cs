using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class RoleRepository
    : BaseRepository<Role>,
      IRoleRepository
{
    public RoleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Role?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<List<Role>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Roles
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeRoleId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Roles
            .AsNoTracking()
            .Where(x =>
                x.CompanyId == companyId &&
                x.Name == name);

        if (excludeRoleId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}