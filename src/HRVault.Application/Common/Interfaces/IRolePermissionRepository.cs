using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default);
		
	Task<RolePermission?> GetAsync(
		Guid roleId,
		Guid permissionId,
		CancellationToken cancellationToken = default);
}