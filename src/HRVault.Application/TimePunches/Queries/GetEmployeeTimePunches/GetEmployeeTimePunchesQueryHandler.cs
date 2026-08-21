using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.TimePunches.DTOs;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.TimePunches.Queries.GetEmployeeTimePunches;

public class GetEmployeeTimePunchesQueryHandler
    : IRequestHandler<GetEmployeeTimePunchesQuery, List<TimePunchDto>>
{
    private readonly ITimePunchRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeTimePunchesQueryHandler(
        ITimePunchRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<TimePunchDto>> Handle(
        GetEmployeeTimePunchesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var employeeExists =
            await _repository.EmployeeExistsInCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (!employeeExists)
            throw new NotFoundException(
                "Funcionário não encontrado.");

        var punches =
            await _repository.GetEmployeePunchesAsync(
                request.EmployeeId,
                companyId,
                request.FromUtc,
                request.ToUtc,
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
