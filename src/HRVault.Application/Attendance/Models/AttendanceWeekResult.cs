namespace HRVault.Application.Attendance.Models;

public class AttendanceWeekResult
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }

    public AttendanceWeekStatus Status { get; set; }

    public int RequiredWorkingDays { get; set; }
    public int WorkedDays { get; set; }
    public int MissingWorkingDays { get; set; }

    public TimeSpan ExpectedTime { get; set; }
    public TimeSpan WorkedTime { get; set; }
    public TimeSpan BreakTime { get; set; }
    public TimeSpan Balance { get; set; }
    public TimeSpan Overtime { get; set; }

    public List<AttendanceAlert> Alerts { get; set; } = new();
}
