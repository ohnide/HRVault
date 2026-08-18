namespace HRVault.Application.Absences.DTOs;

public class EmployeeAbsenceFilterDto
{
    public Guid? EmployeeId { get; set; }

    public Guid? AbsenceTypeId { get; set; }

    public string? Status { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}