namespace HRVault.Application.Attendance.DTOs;

public class AttendanceDayDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }

    public Guid? WorkScheduleId { get; set; }
    public string? WorkScheduleName { get; set; }
    public string? WorkScheduleType { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ExpectedTime { get; set; } = "00:00";
    public string WorkedTime { get; set; } = "00:00";
    public string BreakTime { get; set; } = "00:00";
    public string Balance { get; set; } = "00:00";

    public string LateTime { get; set; } = "00:00";
    public string EarlyLeaveTime { get; set; } = "00:00";
    public string Overtime { get; set; } = "00:00";

    public DateTime? FirstEntryUtc { get; set; }
    public DateTime? LastExitUtc { get; set; }

    public List<AttendanceAlertDto> Alerts { get; set; } = new();
}

public class AttendanceAlertDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
