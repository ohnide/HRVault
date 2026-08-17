using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class Company : SoftDeleteEntity
{
    public string Name { get; set; } = string.Empty;

    public string VatNumber { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? Address { get; set; }
	
	public string? HrNotificationEmail { get; set; }

    public ICollection<User> Users { get; set; }
        = new List<User>();

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();

    public ICollection<Department> Departments { get; set; }
        = new List<Department>();

    public ICollection<Position> Positions { get; set; }
        = new List<Position>();

    public ICollection<Role> Roles { get; set; }
        = new List<Role>();
}