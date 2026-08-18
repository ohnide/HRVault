namespace HRVault.Application.Vacations.DTOs;

public class VacationRequestDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Days { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public string? Notes { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedBy { get; set; }
}