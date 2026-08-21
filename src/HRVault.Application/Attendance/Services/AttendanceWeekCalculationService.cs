using HRVault.Application.Attendance.Interfaces;
using HRVault.Application.Attendance.Models;

namespace HRVault.Application.Attendance.Services;

public class AttendanceWeekCalculationService
    : IAttendanceWeekCalculationService
{
    public AttendanceWeekResult Calculate(
        DateOnly weekStart,
        IReadOnlyCollection<AttendanceWeekDayInput> days,
        int requiredWorkingDays,
        DateOnly currentLocalDate)
    {
        var weekEnd = weekStart.AddDays(6);

        var result = new AttendanceWeekResult
        {
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            RequiredWorkingDays = requiredWorkingDays
        };

        var orderedDays =
            days
                .Where(x =>
                    x.Day.LocalDate >= weekStart &&
                    x.Day.LocalDate <= weekEnd)
                .OrderBy(x => x.Day.LocalDate)
                .ToList();

        result.WorkedDays =
            orderedDays.Count(x => x.HasWorked);

        result.ExpectedTime =
            orderedDays.Aggregate(
                TimeSpan.Zero,
                (sum, x) => sum + x.Day.ExpectedTime);

        result.WorkedTime =
            orderedDays.Aggregate(
                TimeSpan.Zero,
                (sum, x) => sum + x.Day.WorkedTime);

        result.BreakTime =
            orderedDays.Aggregate(
                TimeSpan.Zero,
                (sum, x) => sum + x.Day.BreakTime);

        /*
         * Para semana variável, o saldo útil é calculado
         * sobre os dias efetivamente trabalhados.
         *
         * Os dias que faltam são controlados separadamente
         * por MissingWorkingDays.
         */
        result.Balance =
            result.WorkedTime -
            result.ExpectedTime;

        result.Overtime =
            result.Balance > TimeSpan.Zero
                ? result.Balance
                : TimeSpan.Zero;

        result.MissingWorkingDays =
            Math.Max(
                0,
                requiredWorkingDays - result.WorkedDays);

        var weekClosed =
            currentLocalDate > weekEnd;

        var hasIncompleteDay =
            orderedDays.Any(x =>
                x.Day.Status ==
                    AttendanceDayStatus.Incomplete);

        if (!weekClosed)
        {
            result.Status =
                AttendanceWeekStatus.InProgress;

            // Durante a semana não criamos alerta por
            // dias em falta. Ainda podem ser trabalhados.
            return result;
        }

        if (result.MissingWorkingDays > 0)
        {
            result.Alerts.Add(
                new AttendanceAlert
                {
                    Type =
                        AttendanceAlertType.MissingWorkingDays,

                    Message =
                        $"Foram trabalhados {result.WorkedDays} " +
                        $"de {requiredWorkingDays} dias obrigatórios."
                });
        }

        if (hasIncompleteDay ||
            result.MissingWorkingDays > 0)
        {
            result.Status =
                AttendanceWeekStatus.Incomplete;
        }
        else
        {
            result.Status =
                AttendanceWeekStatus.Complete;
        }

        return result;
    }
}
