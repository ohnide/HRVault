using HRVault.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HRVault.Api.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission =
            await _permissionService.HasPermissionAsync(
                requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}