using HRVault.Application.Attendance.DTOs;
using HRVault.Application.Attendance.Interfaces;
using HRVault.Application.Attendance.Models;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Attendance.Queries.GetEmployeeAttendanceDay;

public class GetEmployeeAttendanceDayQueryHandler
    : IRequestHandler<GetEmployeeAttendanceDayQuery, AttendanceDayDto>
{
    private readonly IAttendanceReadRepository _attendanceRepository;
    private readonly ITimePunchRepository _timePunchRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompanyTimeZoneService _timeZoneService;
    private readonly IAttendanceCalculationService _calculationService;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeAttendanceDayQueryHandler(
        IAttendanceReadRepository attendanceRepository,
        ITimePunchRepository timePunchRepository,
        IEmployeeRepository employeeRepository,
        ICompanyTimeZoneService timeZoneService,
        IAttendanceCalculationService calculationService,
        ICurrentUserService currentUser)
    {
        _attendanceRepository = attendanceRepository;
        _timePunchRepository = timePunchRepository;
        _employeeRepository = employeeRepository;
        _timeZoneService = timeZoneService;
        _calculationService = calculationService;
        _currentUser = currentUser;
    }

    public async Task<AttendanceDayDto> Handle(
        GetEmployeeAttendanceDayQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Funcionário não encontrado.");
        }

        var assignment =
            await _attendanceRepository.GetEmployeeScheduleForDateAsync(
                request.EmployeeId,
                companyId,
                request.Date,
                cancellationToken);

        if (assignment is null)
        {
            throw new NotFoundException(
                "O funcionário não tem um horário atribuído para esta data.");
        }

        var workSchedule = assignment.WorkSchedule;

        var timeZone =
            await _timeZoneService.GetAsync(
                companyId,
                cancellationToken);

        var dayRange =
            _timeZoneService.GetUtcDayRange(
                request.Date,
                timeZone);

        /*
         * Procuramos também no dia seguinte.
         * Isto permite fechar, por exemplo:
         *
         * 21/08 22:00 Entrada
         * 22/08 02:00 Saída
         *
         * O CalculationService filtra a sequência e não atribui
         * picagens normais do dia seguinte ao dia anterior.
         */
        var searchToUtc =
            dayRange.ToUtc.AddDays(1);

        var punches =
            await _timePunchRepository.GetEmployeePunchesAsync(
                request.EmployeeId,
                companyId,
                dayRange.FromUtc,
                searchToUtc,
                cancellationToken);

        var dayOfWeek =
            request.Date.DayOfWeek;

        var scheduleDay =
            workSchedule.Days
                .FirstOrDefault(
                    x => x.DayOfWeek == dayOfWeek);

        var scheduleInput =
            BuildScheduleInput(
                workSchedule.Type,
                scheduleDay);

        var punchInputs =
            punches
                .Where(x => !x.IsVoided)
                .Select(x => new AttendancePunchInput
                {
                    TimestampUtc = x.TimestampUtc,
                    Direction = x.Direction
                })
                .ToList();

        var result =
            _calculationService.Calculate(
                request.Date,
                scheduleInput,
                punchInputs,
                timeZone,
                DateTime.UtcNow);

        return new AttendanceDayDto
        {
            EmployeeId = request.EmployeeId,
            Date = request.Date,

            WorkScheduleId = workSchedule.Id,
            WorkScheduleName = workSchedule.Name,
            WorkScheduleType = workSchedule.Type.ToString(),

            Status = result.Status.ToString(),

            ExpectedTime = FormatDuration(result.ExpectedTime),
            WorkedTime = FormatDuration(result.WorkedTime),
            BreakTime = FormatDuration(result.BreakTime),
            Balance = FormatSignedDuration(result.Balance),

            LateTime = FormatDuration(result.LateTime),
            EarlyLeaveTime = FormatDuration(result.EarlyLeaveTime),
            Overtime = FormatDuration(result.Overtime),

            FirstEntryUtc = result.FirstEntryUtc,
            LastExitUtc = result.LastExitUtc,

            Alerts = result.Alerts
                .Select(x => new AttendanceAlertDto
                {
                    Type = x.Type.ToString(),
                    Message = x.Message
                })
                .ToList()
        };
    }

    private static AttendanceDaySchedule BuildScheduleInput(
        WorkScheduleType type,
        HRVault.Domain.Entities.WorkScheduleDay? day)
    {
        /*
         * Isenção de horário não depende de horas fixas.
         * Ainda assim, se o modelo tiver dias definidos,
         * respeitamos IsWorkingDay.
         */
        var isWorkingDay =
            day?.IsWorkingDay ??
            (type == WorkScheduleType.ScheduleExempt);

        return new AttendanceDaySchedule
        {
            Type = type,
            IsWorkingDay = isWorkingDay,

            RequiredDailyTime =
                day?.RequiredDailyTime,

            Periods =
                day?.Periods
                    .OrderBy(x => x.StartTime)
                    .Select(x => new AttendancePeriodDefinition
                    {
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    })
                    .ToList()
                ?? new List<AttendancePeriodDefinition>()
        };
    }

    private static string FormatDuration(
        TimeSpan value)
    {
        var totalMinutes =
            (long)Math.Round(
                value.TotalMinutes,
                MidpointRounding.AwayFromZero);

        if (totalMinutes < 0)
            totalMinutes = -totalMinutes;

        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        return $"{hours:00}:{minutes:00}";
    }

    private static string FormatSignedDuration(
        TimeSpan value)
    {
        var prefix =
            value < TimeSpan.Zero
                ? "-"
                : value > TimeSpan.Zero
                    ? "+"
                    : "";

        return prefix +
            FormatDuration(value);
    }
}
