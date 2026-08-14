using HRVault.Domain.Enums;

namespace HRVault.Application.Employees.DTOs;

public class EmployeeContactDto
{
    public Guid Id { get; set; }

    public ContactType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public string? Notes { get; set; }
}