using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class UserRepository
    : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    // Usado no login.
    // O email é globalmente único, por isso não filtramos por CompanyId.
    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    // Obtém um utilizador apenas se pertencer à empresa indicada.
    public async Task<User?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    // Lista apenas os utilizadores da empresa atual.
    public async Task<List<User>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    // Verifica se o email já está a ser utilizado.
    // excludeUserId permite ao Update manter o email do próprio utilizador.
    public async Task<bool> EmailExistsAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Users
            .AsNoTracking()
            .Where(x => x.Email == email);

        if (excludeUserId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(
            cancellationToken);
    }
}