using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class User : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public Guid? EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdministrator { get; set; }

    public bool IsPlatformAdministrator { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? PasswordChangedAt { get; set; }
	
	public ICollection<RefreshToken> RefreshTokens { get; set; }
		= new List<RefreshToken>();

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
}