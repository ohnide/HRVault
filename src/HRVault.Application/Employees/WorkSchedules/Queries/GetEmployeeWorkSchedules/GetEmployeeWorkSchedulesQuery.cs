using HRVault.Application.Employees.WorkSchedules.DTOs;
using MediatR;

namespace HRVault.Application.Employees.WorkSchedules.Queries.GetEmployeeWorkSchedules;

public record GetEmployeeWorkSchedulesQuery(
    Guid EmployeeId
) : IRequest<List<EmployeeWorkScheduleDto>>;
