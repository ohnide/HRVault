namespace HRVault.Application.Attendance.Models;

public class AttendanceAlert
{
    public AttendanceAlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;
}
