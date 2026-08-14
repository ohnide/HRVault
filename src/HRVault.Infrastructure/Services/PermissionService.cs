using HRVault.Application.Common.Interfaces;
using HRVault.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PermissionService(
        ApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated)
            return false;

        if (_currentUser.UserId is null)
            return false;

        if (_currentUser.IsAdministrator)
            return true;

        return await _context.UserRoles
            .Where(x => x.UserId == _currentUser.UserId.Value)
            .SelectMany(x => x.Role.RolePermissions)
            .AnyAsync(
                x =>
                    x.Permission.Code == permissionCode &&
                    x.Permission.IsActive,
                cancellationToken);
    }
}