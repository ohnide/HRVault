using HRVault.Application.Attendance.Interfaces;
using HRVault.Application.Attendance.Models;
using HRVault.Domain.Enums;

namespace HRVault.Application.Attendance.Services;

public class AttendanceCalculationService
    : IAttendanceCalculationService
{
    private sealed record PunchPair(
        AttendancePunchInput Entry,
        AttendancePunchInput Exit);

    public AttendanceDayResult Calculate(
        DateOnly localDate,
        AttendanceDaySchedule schedule,
        IReadOnlyCollection<AttendancePunchInput> punches,
        TimeZoneInfo companyTimeZone,
        DateTime nowUtc)
    {
        var result = new AttendanceDayResult
        {
            LocalDate = localDate
        };

        if (!schedule.IsWorkingDay &&
            schedule.Type != WorkScheduleType.ScheduleExempt)
        {
            result.Status = AttendanceDayStatus.NonWorkingDay;
            return result;
        }

        result.ExpectedTime =
            CalculateExpectedTime(schedule);

        var orderedPunches =
            punches
                .OrderBy(x => x.TimestampUtc)
                .ToList();

        var relevantPunches =
            BuildRelevantPunchSequence(
                localDate,
                orderedPunches,
                companyTimeZone);

        if (relevantPunches.Count == 0)
        {
            result.Status =
                IsLocalDateClosed(
                    localDate,
                    companyTimeZone,
                    nowUtc)
                    ? AttendanceDayStatus.NoPunches
                    : AttendanceDayStatus.InProgress;

            result.Balance =
                schedule.Type ==
                    WorkScheduleType.ScheduleExempt
                    ? TimeSpan.Zero
                    : -result.ExpectedTime;

            return result;
        }

        var pairs = new List<PunchPair>();
        AttendancePunchInput? pendingEntry = null;

        var hasInvalidSequence = false;
        var missingEntry = false;

        foreach (var punch in relevantPunches)
        {
            switch (punch.Direction)
            {
                case AttendanceEventDirection.Entry:
                    if (pendingEntry is not null)
                    {
                        hasInvalidSequence = true;
                    }

                    pendingEntry = punch;
                    result.FirstEntryUtc ??=
                        punch.TimestampUtc;
                    break;

                case AttendanceEventDirection.Exit:
                    if (pendingEntry is null)
                    {
                        missingEntry = true;
                        hasInvalidSequence = true;
                        result.LastExitUtc =
                            punch.TimestampUtc;
                        break;
                    }

                    if (punch.TimestampUtc <
                        pendingEntry.TimestampUtc)
                    {
                        hasInvalidSequence = true;
                        break;
                    }

                    pairs.Add(
                        new PunchPair(
                            pendingEntry,
                            punch));

                    result.LastExitUtc =
                        punch.TimestampUtc;

                    pendingEntry = null;
                    break;

                default:
                    hasInvalidSequence = true;
                    break;
            }
        }

        result.WorkedTime =
            pairs.Aggregate(
                TimeSpan.Zero,
                (sum, pair) =>
                    sum +
                    (pair.Exit.TimestampUtc -
                     pair.Entry.TimestampUtc));

        if (pairs.Count > 1)
        {
            for (var i = 1; i < pairs.Count; i++)
            {
                var gap =
                    pairs[i].Entry.TimestampUtc -
                    pairs[i - 1].Exit.TimestampUtc;

                if (gap > TimeSpan.Zero)
                {
                    result.BreakTime += gap;
                }
            }
        }

        if (schedule.Type ==
            WorkScheduleType.ScheduleExempt)
        {
            result.Balance = TimeSpan.Zero;
            result.Overtime = TimeSpan.Zero;
            result.LateTime = TimeSpan.Zero;
            result.EarlyLeaveTime = TimeSpan.Zero;
        }
        else
        {
            result.Balance =
                result.WorkedTime -
                result.ExpectedTime;

            result.Overtime =
                result.Balance > TimeSpan.Zero
                    ? result.Balance
                    : TimeSpan.Zero;
        }

        if (schedule.Type ==
            WorkScheduleType.Fixed)
        {
            CalculateFixedScheduleMetrics(
                localDate,
                schedule,
                result,
                companyTimeZone);
        }

        var dayClosed =
            IsLocalDateClosed(
                localDate,
                companyTimeZone,
                nowUtc);

        if (pendingEntry is not null)
        {
            if (dayClosed)
            {
                result.Status =
                    AttendanceDayStatus.Incomplete;

                result.Alerts.Add(
                    new AttendanceAlert
                    {
                        Type =
                            AttendanceAlertType.MissingExit,

                        Message =
                            "Falta uma picagem de saída."
                    });
            }
            else
            {
                result.Status =
                    AttendanceDayStatus.InProgress;
            }
        }
        else if (hasInvalidSequence)
        {
            result.Status =
                dayClosed
                    ? AttendanceDayStatus.Incomplete
                    : AttendanceDayStatus.InProgress;
        }
        else
        {
            result.Status =
                AttendanceDayStatus.Complete;
        }

        if (missingEntry && dayClosed)
        {
            result.Alerts.Add(
                new AttendanceAlert
                {
                    Type =
                        AttendanceAlertType.MissingEntry,

                    Message =
                        "Existe uma saída sem entrada correspondente."
                });
        }

        if (hasInvalidSequence &&
            dayClosed &&
            result.Alerts.Count == 0)
        {
            result.Alerts.Add(
                new AttendanceAlert
                {
                    Type =
                        AttendanceAlertType.InvalidSequence,

                    Message =
                        "A sequência de picagens está incompleta ou inválida."
                });
        }

        return result;
    }

    private static List<AttendancePunchInput>
        BuildRelevantPunchSequence(
            DateOnly localDate,
            IReadOnlyCollection<AttendancePunchInput> orderedPunches,
            TimeZoneInfo timeZone)
    {
        var result =
            new List<AttendancePunchInput>();

        var pendingEntry = false;

        foreach (var punch in orderedPunches)
        {
            var punchLocal =
                ConvertUtcToLocal(
                    punch.TimestampUtc,
                    timeZone);

            var punchDate =
                DateOnly.FromDateTime(
                    punchLocal);

            if (punchDate < localDate)
                continue;

            if (punchDate == localDate)
            {
                result.Add(punch);

                if (punch.Direction ==
                    AttendanceEventDirection.Entry)
                {
                    pendingEntry = true;
                }
                else if (
                    punch.Direction ==
                        AttendanceEventDirection.Exit &&
                    pendingEntry)
                {
                    pendingEntry = false;
                }

                continue;
            }

            if (!pendingEntry)
                break;

            result.Add(punch);

            if (punch.Direction ==
                AttendanceEventDirection.Exit)
            {
                pendingEntry = false;
                break;
            }
        }

        return result;
    }

    private static TimeSpan CalculateExpectedTime(
        AttendanceDaySchedule schedule)
    {
        return schedule.Type switch
        {
            WorkScheduleType.Fixed =>
                schedule.Periods.Aggregate(
                    TimeSpan.Zero,
                    (sum, period) =>
                        sum +
                        CalculatePeriodDuration(
                            period.StartTime,
                            period.EndTime)),

            WorkScheduleType.Flexible =>
                schedule.RequiredDailyTime.HasValue
                    ? schedule.RequiredDailyTime.Value.ToTimeSpan()
                    : TimeSpan.Zero,

            WorkScheduleType.WeeklyVariable =>
                schedule.RequiredDailyTime.HasValue
                    ? schedule.RequiredDailyTime.Value.ToTimeSpan()
                    : TimeSpan.Zero,

            WorkScheduleType.ScheduleExempt =>
                TimeSpan.Zero,

            _ => TimeSpan.Zero
        };
    }

    private static TimeSpan CalculatePeriodDuration(
        TimeOnly start,
        TimeOnly end)
    {
        var duration =
            end.ToTimeSpan() -
            start.ToTimeSpan();

        if (duration < TimeSpan.Zero)
        {
            duration +=
                TimeSpan.FromDays(1);
        }

        return duration;
    }

    private static void CalculateFixedScheduleMetrics(
        DateOnly localDate,
        AttendanceDaySchedule schedule,
        AttendanceDayResult result,
        TimeZoneInfo timeZone)
    {
        if (schedule.Periods.Count == 0)
            return;

        var firstPeriod =
            schedule.Periods
                .OrderBy(x => x.StartTime)
                .First();

        var lastPeriod =
            schedule.Periods
                .OrderBy(x => x.StartTime)
                .Last();

        var expectedStartUtc =
            LocalToUtc(
                localDate,
                firstPeriod.StartTime,
                timeZone);

        var expectedEndDate =
            localDate;

        if (lastPeriod.EndTime <
            lastPeriod.StartTime)
        {
            expectedEndDate =
                localDate.AddDays(1);
        }

        var expectedEndUtc =
            LocalToUtc(
                expectedEndDate,
                lastPeriod.EndTime,
                timeZone);

        if (result.FirstEntryUtc.HasValue &&
            result.FirstEntryUtc.Value >
            expectedStartUtc)
        {
            result.LateTime =
                result.FirstEntryUtc.Value -
                expectedStartUtc;
        }

        if (result.LastExitUtc.HasValue &&
            result.LastExitUtc.Value <
            expectedEndUtc)
        {
            result.EarlyLeaveTime =
                expectedEndUtc -
                result.LastExitUtc.Value;
        }
    }

    private static DateTime ConvertUtcToLocal(
        DateTime utc,
        TimeZoneInfo timeZone)
    {
        var normalizedUtc =
            utc.Kind == DateTimeKind.Utc
                ? utc
                : DateTime.SpecifyKind(
                    utc,
                    DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(
            normalizedUtc,
            timeZone);
    }

    private static DateTime LocalToUtc(
        DateOnly date,
        TimeOnly time,
        TimeZoneInfo timeZone)
    {
        var local =
            date.ToDateTime(
                time,
                DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(
            local,
            timeZone);
    }

    private static bool IsLocalDateClosed(
        DateOnly localDate,
        TimeZoneInfo timeZone,
        DateTime nowUtc)
    {
        var normalizedNowUtc =
            nowUtc.Kind == DateTimeKind.Utc
                ? nowUtc
                : DateTime.SpecifyKind(
                    nowUtc,
                    DateTimeKind.Utc);

        var localNow =
            TimeZoneInfo.ConvertTimeFromUtc(
                normalizedNowUtc,
                timeZone);

        var today =
            DateOnly.FromDateTime(
                localNow);

        return localDate < today;
    }
}
