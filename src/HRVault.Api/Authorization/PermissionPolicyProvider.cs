using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HRVault.Api.Authorization;

public class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (!policyName.StartsWith("Permission:"))
            return await base.GetPolicyAsync(policyName);

        var permission = policyName["Permission:".Length..];

        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(
                new PermissionRequirement(permission))
            .Build();

        return policy;
    }
}