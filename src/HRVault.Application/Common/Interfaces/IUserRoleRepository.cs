using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<List<Role>> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);
}