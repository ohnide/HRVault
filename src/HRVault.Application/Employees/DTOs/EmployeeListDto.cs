namespace HRVault.Application.Employees.DTOs;

public class EmployeeListDto
{
    public Guid Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Department { get; set; }

    public string? Position { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateOnly HireDate { get; set; }
}