using System.Security.Claims;
using HRVault.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRVault.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        Guid.TryParse(
            User?.FindFirst(
                ClaimTypes.NameIdentifier)?.Value,
            out var id)
                ? id
                : null;

    public Guid? CompanyId =>
        Guid.TryParse(
            User?.FindFirst("companyId")?.Value,
            out var id)
                ? id
                : null;

    public string? Email =>
        User?.FindFirst(
            ClaimTypes.Email)?.Value;

    public string? Name =>
        User?.FindFirst(
            ClaimTypes.Name)?.Value;

    public bool IsAdministrator =>
        bool.TryParse(
            User?.FindFirst(
                "isAdministrator")?.Value,
            out var value)
            && value;

    public bool IsPlatformAdministrator =>
        bool.TryParse(
            User?.FindFirst(
                "isPlatformAdministrator")?.Value,
            out var value)
            && value;
}