using HRVault.Domain.Enums;

namespace HRVault.Application.Employees.DTOs;

public class EmployeeProfileDto
{
    public DateOnly? BirthDate { get; set; }

    public Gender? Gender { get; set; }

    public MaritalStatus? MaritalStatus { get; set; }

    public string? Nationality { get; set; }

    public DocumentType? DocumentType { get; set; }

    public string? DocumentNumber { get; set; }

    public string? TaxNumber { get; set; }

    public string? SocialSecurityNumber { get; set; }

    public string? SnsNumber { get; set; }
}