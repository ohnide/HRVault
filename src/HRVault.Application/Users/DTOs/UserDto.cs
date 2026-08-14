namespace HRVault.Application.Users.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? EmployeeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsAdministrator { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? PasswordChangedAt { get; set; }
}