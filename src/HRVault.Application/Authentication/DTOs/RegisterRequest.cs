namespace HRVault.Application.Authentication.DTOs;

public class RegisterRequest
{
    public Guid CompanyId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}