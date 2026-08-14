using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class Department : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentDepartmentId { get; set; }

    public Department? ParentDepartment { get; set; }

    public ICollection<Department> Children { get; set; }
        = new List<Department>();

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();
}