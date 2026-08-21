namespace HRVault.Application.Attendance.DTOs;

public class AttendanceWeekDto
{
    public Guid EmployeeId { get; set; }

    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }

    public string Status { get; set; } = string.Empty;

    public int RequiredWorkingDays { get; set; }
    public int WorkedDays { get; set; }
    public int MissingWorkingDays { get; set; }

    public string ExpectedTime { get; set; } = "00:00";
    public string WorkedTime { get; set; } = "00:00";
    public string BreakTime { get; set; } = "00:00";
    public string Balance { get; set; } = "00:00";
    public string Overtime { get; set; } = "00:00";

    public List<AttendanceAlertDto> Alerts { get; set; } = new();

    public List<AttendanceWeekDayDto> Days { get; set; } = new();
}

public class AttendanceWeekDayDto
{
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

    public bool HasWorked { get; set; }

    public List<AttendanceAlertDto> Alerts { get; set; } = new();
}
