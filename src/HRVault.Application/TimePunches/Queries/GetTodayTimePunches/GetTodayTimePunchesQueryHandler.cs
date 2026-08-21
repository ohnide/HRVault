using HRVault.Application.Common.Interfaces;
using HRVault.Application.TimePunches.DTOs;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.TimePunches.Queries.GetTodayTimePunches;

public class GetTodayTimePunchesQueryHandler
    : IRequestHandler<GetTodayTimePunchesQuery, List<TimePunchDto>>
{
    private readonly ITimePunchRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly ICompanyTimeZoneService _timeZoneService;

    public GetTodayTimePunchesQueryHandler(
        ITimePunchRepository repository,
        ICurrentUserService currentUser,
        ICompanyTimeZoneService timeZoneService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _timeZoneService = timeZoneService;
    }

    public async Task<List<TimePunchDto>> Handle(
        GetTodayTimePunchesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var timeZone =
            await _timeZoneService.GetAsync(
                companyId,
                cancellationToken);

        var nowUtc = DateTime.UtcNow;

        var localNow =
            _timeZoneService.ConvertUtcToLocal(
                nowUtc,
                timeZone);

        var localDate =
            DateOnly.FromDateTime(localNow);

        var range =
            _timeZoneService.GetUtcDayRange(
                localDate,
                timeZone);

        var punches = request.EmployeeId.HasValue
            ? await _repository.GetEmployeePunchesAsync(
                request.EmployeeId.Value,
                companyId,
                range.FromUtc,
                range.ToUtc,
                cancellationToken)
            : await _repository.GetCompanyPunchesAsync(
                companyId,
                range.FromUtc,
                range.ToUtc,
                cancellationToken);

        return punches
            .OrderByDescending(x => x.TimestampUtc)
            .Select(x => new TimePunchDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName =
                    x.Employee.FirstName + " " + x.Employee.LastName,
                TimestampUtc = x.TimestampUtc,
                Source = (int)x.Source,
                SourceName = x.Source switch
                {
                    TimePunchSource.HRVault => "HRVault",
                    TimePunchSource.Device => "Dispositivo",
                    TimePunchSource.ManualAdjustment => "Ajuste manual",
                    TimePunchSource.Import => "Importação",
                    _ => x.Source.ToString()
                },
                Direction = (int)x.Direction,
                DirectionName = x.Direction switch
                {
                    AttendanceEventDirection.Entry => "Entrada",
                    AttendanceEventDirection.Exit => "Saída",
                    _ => "Não definido"
                },
				AdjustmentReason = x.AdjustmentReason,
                AttendanceDeviceId = x.AttendanceDeviceId,
                IsVoided = x.IsVoided,
                VoidReason = x.VoidReason,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}
