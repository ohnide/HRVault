namespace HRVault.Application.Employees.DTOs;

public class EmployeeEmergencyContactDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Notes { get; set; }
}