using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.WorkSchedules.DTOs;
using MediatR;

namespace HRVault.Application.Employees.WorkSchedules.Queries.GetEmployeeWorkSchedules;

public class GetEmployeeWorkSchedulesQueryHandler
    : IRequestHandler<GetEmployeeWorkSchedulesQuery, List<EmployeeWorkScheduleDto>>
{
    private readonly IEmployeeWorkScheduleRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeWorkSchedulesQueryHandler(
        IEmployeeWorkScheduleRepository repository,
        IEmployeeRepository employeeRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _currentUser = currentUser;
    }

    public async Task<List<EmployeeWorkScheduleDto>> Handle(
        GetEmployeeWorkSchedulesQuery request,
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
            throw new NotFoundException("Funcionário não encontrado.");

        var assignments =
            await _repository.GetHistoryAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        return assignments
            .Select(x => new EmployeeWorkScheduleDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,

                WorkScheduleId = x.WorkScheduleId,
                WorkScheduleName = x.WorkSchedule.Name,
                WorkScheduleType = x.WorkSchedule.Type.ToString(),

                StartDate = x.StartDate,
                EndDate = x.EndDate,

                IsCurrent =
                    x.StartDate <= today &&
                    (!x.EndDate.HasValue ||
                     x.EndDate.Value >= today)
            })
            .ToList();
    }
}
