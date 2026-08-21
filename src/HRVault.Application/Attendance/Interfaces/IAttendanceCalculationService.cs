using HRVault.Application.Attendance.Models;

namespace HRVault.Application.Attendance.Interfaces;

public interface IAttendanceCalculationService
{
    AttendanceDayResult Calculate(
        DateOnly localDate,
        AttendanceDaySchedule schedule,
        IReadOnlyCollection<AttendancePunchInput> punches,
        TimeZoneInfo companyTimeZone,
        DateTime nowUtc);
}
