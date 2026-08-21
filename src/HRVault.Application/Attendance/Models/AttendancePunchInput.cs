using HRVault.Domain.Enums;

namespace HRVault.Application.Attendance.Models;

public class AttendancePunchInput
{
    public DateTime TimestampUtc { get; set; }
    public AttendanceEventDirection Direction { get; set; }
}
