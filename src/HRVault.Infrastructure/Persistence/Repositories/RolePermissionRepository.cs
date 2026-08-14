using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _context;

    public RolePermissionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .AsNoTracking()
            .Include(x => x.Permission)
            .Where(x => x.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .AnyAsync(
                x =>
                    x.RoleId == roleId &&
                    x.PermissionId == permissionId,
                cancellationToken);
    }

    public async Task AddAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddAsync(
            rolePermission,
            cancellationToken);
    }
	
	public async Task<RolePermission?> GetAsync(
		Guid roleId,
		Guid permissionId,
		CancellationToken cancellationToken = default)
	{
		return await _context.RolePermissions
			.FirstOrDefaultAsync(
				x =>
					x.RoleId == roleId &&
					x.PermissionId == permissionId,
				cancellationToken);
	}

    public Task DeleteAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        _context.RolePermissions.Remove(rolePermission);

        return Task.CompletedTask;
    }
}