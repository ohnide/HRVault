namespace HRVault.Application.Absences.DTOs;

public class EmployeeAbsenceDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public Guid AbsenceTypeId { get; set; }

    public string AbsenceTypeName { get; set; }
        = string.Empty;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}