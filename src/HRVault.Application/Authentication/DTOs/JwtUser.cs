namespace HRVault.Application.Authentication.DTOs;

public class JwtUser
{
    public Guid UserId { get; set; }

    public Guid CompanyId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public bool IsAdministrator { get; set; }

    public bool IsPlatformAdministrator { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}