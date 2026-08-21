namespace HRVault.Application.Attendance.Models;

public class AttendanceDayResult
{
    public DateOnly LocalDate { get; set; }

    public AttendanceDayStatus Status { get; set; }

    public TimeSpan ExpectedTime { get; set; }
    public TimeSpan WorkedTime { get; set; }
    public TimeSpan BreakTime { get; set; }
    public TimeSpan Balance { get; set; }

    public DateTime? FirstEntryUtc { get; set; }
    public DateTime? LastExitUtc { get; set; }

    public TimeSpan LateTime { get; set; }
    public TimeSpan EarlyLeaveTime { get; set; }
    public TimeSpan Overtime { get; set; }

    public List<AttendanceAlert> Alerts { get; set; }
        = new();
}
