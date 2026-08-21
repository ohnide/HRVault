namespace HRVault.Application.Employees.WorkSchedules.DTOs;

public class EmployeeWorkScheduleDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }

    public Guid WorkScheduleId { get; set; }
    public string WorkScheduleName { get; set; } = string.Empty;
    public string WorkScheduleType { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public bool IsCurrent { get; set; }
}
