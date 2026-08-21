using HRVault.Application.WorkSchedules.DTOs;
using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.CreateWorkSchedule;

public record CreateWorkScheduleCommand(
    string Name,
    string? Description,
    int Type,
    int? RequiredWorkingDaysPerWeek,
    List<WorkScheduleDayInputDto> Days
) : IRequest<Guid>;
