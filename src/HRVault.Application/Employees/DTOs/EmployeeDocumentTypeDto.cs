namespace HRVault.Application.Employees.DTOs;

public class EmployeeDocumentTypeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool HasExpiration { get; set; }

    public int? ExpirationWarningDays { get; set; }
}