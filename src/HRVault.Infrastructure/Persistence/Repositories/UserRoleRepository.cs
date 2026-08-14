using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class UserRoleRepository
    : IUserRoleRepository
{
    private readonly ApplicationDbContext _context;

    public UserRoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserRole?> GetAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.RoleId == roleId,
                cancellationToken);
    }

    public async Task<List<Role>> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        await _context.UserRoles.AddAsync(
            userRole,
            cancellationToken);
    }

    public Task DeleteAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        _context.UserRoles.Remove(userRole);

        return Task.CompletedTask;
    }
}