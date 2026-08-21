using HRVault.Application.Attendance.DTOs;
using HRVault.Application.Attendance.Interfaces;
using HRVault.Application.Attendance.Models;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Attendance.Queries.GetEmployeeAttendanceWeek;

public class GetEmployeeAttendanceWeekQueryHandler
    : IRequestHandler<GetEmployeeAttendanceWeekQuery, AttendanceWeekDto>
{
    private readonly IAttendanceReadRepository _attendanceRepository;
    private readonly ITimePunchRepository _timePunchRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompanyTimeZoneService _timeZoneService;
    private readonly IAttendanceCalculationService _dayCalculationService;
    private readonly IAttendanceWeekCalculationService _weekCalculationService;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeAttendanceWeekQueryHandler(
        IAttendanceReadRepository attendanceRepository,
        ITimePunchRepository timePunchRepository,
        IEmployeeRepository employeeRepository,
        ICompanyTimeZoneService timeZoneService,
        IAttendanceCalculationService dayCalculationService,
        IAttendanceWeekCalculationService weekCalculationService,
        ICurrentUserService currentUser)
    {
        _attendanceRepository = attendanceRepository;
        _timePunchRepository = timePunchRepository;
        _employeeRepository = employeeRepository;
        _timeZoneService = timeZoneService;
        _dayCalculationService = dayCalculationService;
        _weekCalculationService = weekCalculationService;
        _currentUser = currentUser;
    }

    public async Task<AttendanceWeekDto> Handle(
        GetEmployeeAttendanceWeekQuery request,
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

        var weekStart = GetMonday(request.Date);
        var weekEnd = weekStart.AddDays(6);

        var timeZone =
            await _timeZoneService.GetAsync(
                companyId,
                cancellationToken);

        var localNow =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                timeZone);

        var currentLocalDate =
            DateOnly.FromDateTime(localNow);

        /*
         * O nº de dias obrigatórios pertence ao horário semanal variável.
         * Usamos o horário que está atribuído na data pedida pelo utilizador.
         *
         * Se o funcionário trocar de horário a meio da semana,
         * os cálculos diários continuam a respeitar a atribuição efetiva
         * de cada dia. A regra semanal fica associada ao horário da data
         * consultada.
         */
        var anchorAssignment =
            await _attendanceRepository.GetEmployeeScheduleForDateAsync(
                request.EmployeeId,
                companyId,
                request.Date,
                cancellationToken);

        var requiredWorkingDays =
            anchorAssignment?.WorkSchedule.Type ==
                WorkScheduleType.WeeklyVariable
                ? anchorAssignment.WorkSchedule.RequiredWorkingDaysPerWeek ?? 0
                : 0;

        var weekDayInputs =
            new List<AttendanceWeekDayInput>();

        var dayDtos =
            new List<AttendanceWeekDayDto>();

        for (var offset = 0; offset < 7; offset++)
        {
            var date = weekStart.AddDays(offset);

            var assignment =
                await _attendanceRepository.GetEmployeeScheduleForDateAsync(
                    request.EmployeeId,
                    companyId,
                    date,
                    cancellationToken);

            /*
             * Se não existir horário atribuído nesse dia,
             * mantemos o dia visível no resumo semanal,
             * mas não inventamos uma obrigação de trabalho.
             */
            if (assignment is null)
            {
                var emptyDay =
                    new AttendanceDayResult
                    {
                        LocalDate = date,
                        Status = AttendanceDayStatus.NonWorkingDay
                    };

                weekDayInputs.Add(
                    new AttendanceWeekDayInput
                    {
                        Day = emptyDay,
                        HasWorked = false
                    });

                dayDtos.Add(
                    new AttendanceWeekDayDto
                    {
                        Date = date,
                        Status = emptyDay.Status.ToString(),
                        HasWorked = false
                    });

                continue;
            }

            var workSchedule =
                assignment.WorkSchedule;

            var scheduleDay =
                workSchedule.Days.FirstOrDefault(
                    x => x.DayOfWeek == date.DayOfWeek);

            var scheduleInput =
                BuildScheduleInput(
                    workSchedule.Type,
                    scheduleDay);

            var dayRange =
                _timeZoneService.GetUtcDayRange(
                    date,
                    timeZone);

            /*
             * Incluímos o dia seguinte para permitir que uma saída
             * posterior à meia-noite feche a entrada do dia anterior.
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

            var punchInputs =
                punches
                    .Where(x => !x.IsVoided)
                    .Select(x => new AttendancePunchInput
                    {
                        TimestampUtc = x.TimestampUtc,
                        Direction = x.Direction
                    })
                    .ToList();

            var dayResult =
                _dayCalculationService.Calculate(
                    date,
                    scheduleInput,
                    punchInputs,
                    timeZone,
                    DateTime.UtcNow);

            var hasWorked =
                dayResult.WorkedTime > TimeSpan.Zero ||
                dayResult.FirstEntryUtc.HasValue;

            weekDayInputs.Add(
                new AttendanceWeekDayInput
                {
                    Day = dayResult,
                    HasWorked = hasWorked
                });

            dayDtos.Add(
                new AttendanceWeekDayDto
                {
                    Date = date,

                    WorkScheduleId = workSchedule.Id,
                    WorkScheduleName = workSchedule.Name,
                    WorkScheduleType = workSchedule.Type.ToString(),

                    Status = dayResult.Status.ToString(),

                    ExpectedTime = FormatDuration(dayResult.ExpectedTime),
                    WorkedTime = FormatDuration(dayResult.WorkedTime),
                    BreakTime = FormatDuration(dayResult.BreakTime),
                    Balance = FormatSignedDuration(dayResult.Balance),

                    LateTime = FormatDuration(dayResult.LateTime),
                    EarlyLeaveTime = FormatDuration(dayResult.EarlyLeaveTime),
                    Overtime = FormatDuration(dayResult.Overtime),

                    HasWorked = hasWorked,

                    Alerts = dayResult.Alerts
                        .Select(x => new AttendanceAlertDto
                        {
                            Type = x.Type.ToString(),
                            Message = x.Message
                        })
                        .ToList()
                });
        }

        /*
         * Para horários não semanais variáveis, RequiredWorkingDays fica 0.
         * O serviço semanal continua útil para agregar horas e estados.
         */
        var weekResult =
            _weekCalculationService.Calculate(
                weekStart,
                weekDayInputs,
                requiredWorkingDays,
                currentLocalDate);

        return new AttendanceWeekDto
        {
            EmployeeId = request.EmployeeId,

            WeekStart = weekResult.WeekStart,
            WeekEnd = weekResult.WeekEnd,

            Status = weekResult.Status.ToString(),

            RequiredWorkingDays =
                weekResult.RequiredWorkingDays,

            WorkedDays =
                weekResult.WorkedDays,

            MissingWorkingDays =
                weekResult.MissingWorkingDays,

            ExpectedTime =
                FormatDuration(weekResult.ExpectedTime),

            WorkedTime =
                FormatDuration(weekResult.WorkedTime),

            BreakTime =
                FormatDuration(weekResult.BreakTime),

            Balance =
                FormatSignedDuration(weekResult.Balance),

            Overtime =
                FormatDuration(weekResult.Overtime),

            Alerts = weekResult.Alerts
                .Select(x => new AttendanceAlertDto
                {
                    Type = x.Type.ToString(),
                    Message = x.Message
                })
                .ToList(),

            Days = dayDtos
        };
    }

    private static AttendanceDaySchedule BuildScheduleInput(
        WorkScheduleType type,
        WorkScheduleDay? day)
    {
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

    private static DateOnly GetMonday(
        DateOnly date)
    {
        var difference =
            ((int)date.DayOfWeek -
             (int)DayOfWeek.Monday + 7) % 7;

        return date.AddDays(-difference);
    }

    private static string FormatDuration(
        TimeSpan value)
    {
        var totalMinutes =
            (long)Math.Round(
                Math.Abs(value.TotalMinutes),
                MidpointRounding.AwayFromZero);

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
