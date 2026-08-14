namespace HRVault.Application.Departments.DTOs;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentDepartmentId { get; set; }
}