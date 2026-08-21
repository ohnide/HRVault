using HRVault.Application.Employees.WorkSchedules.DTOs;
using MediatR;

namespace HRVault.Application.Employees.WorkSchedules.Commands.AssignEmployeeWorkSchedule;

public record AssignEmployeeWorkScheduleCommand(
    Guid EmployeeId,
    Guid WorkScheduleId,
    DateOnly StartDate
) : IRequest<EmployeeWorkScheduleDto>;
