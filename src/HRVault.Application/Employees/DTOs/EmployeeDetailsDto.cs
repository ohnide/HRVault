using HRVault.Domain.Enums;

namespace HRVault.Application.Employees.DTOs;

public class EmployeeDetailsDto
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public Guid? PositionId { get; set; }

    public string? PositionName { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? WorkEmail { get; set; }

    public string? PersonalEmail { get; set; }

    public string? MobilePhone { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public EmployeeStatus Status { get; set; }

    public EmployeeProfileDto? Profile { get; set; }

    public List<EmployeeAddressDto> Addresses { get; set; }
        = new();

    public List<EmployeeContactDto> Contacts { get; set; }
        = new();

    public EmployeeEmergencyContactDto? EmergencyContact { get; set; }
}