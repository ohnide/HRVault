namespace HRVault.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default);
}