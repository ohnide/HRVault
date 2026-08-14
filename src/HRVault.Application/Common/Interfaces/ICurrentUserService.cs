namespace HRVault.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? CompanyId { get; }

    string? Email { get; }

    string? Name { get; }

    bool IsAuthenticated { get; }

    bool IsAdministrator { get; }

    bool IsPlatformAdministrator { get; }
}