using HRVault.Application.Attendance.Models;

namespace HRVault.Application.Attendance.Interfaces;

public interface IAttendanceWeekCalculationService
{
    AttendanceWeekResult Calculate(
        DateOnly weekStart,
        IReadOnlyCollection<AttendanceWeekDayInput> days,
        int requiredWorkingDays,
        DateOnly currentLocalDate);
}
