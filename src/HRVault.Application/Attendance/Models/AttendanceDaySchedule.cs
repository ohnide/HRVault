using HRVault.Domain.Enums;

namespace HRVault.Application.Attendance.Models;

public class AttendanceDaySchedule
{
    public WorkScheduleType Type { get; set; }

    public bool IsWorkingDay { get; set; }

    public TimeOnly? RequiredDailyTime { get; set; }

    public List<AttendancePeriodDefinition> Periods { get; set; }
        = new();
}
