using HRVault.Application.Common.Interfaces;

namespace HRVault.Api.Middleware;

public class TenantStatusMiddleware
{
    private readonly RequestDelegate _next;

    public TenantStatusMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentUserService currentUser,
        ICompanyRepository companyRepository)
    {
        if (!currentUser.IsAuthenticated)
        {
            await _next(context);
            return;
        }

        if (currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException(
                "User company could not be determined.");
        }

        var company = await companyRepository.GetByIdAsync(
            currentUser.CompanyId.Value,
            context.RequestAborted);

        if (company is null)
        {
            throw new UnauthorizedAccessException(
                "The company is inactive or no longer available.");
        }

        await _next(context);
    }
}