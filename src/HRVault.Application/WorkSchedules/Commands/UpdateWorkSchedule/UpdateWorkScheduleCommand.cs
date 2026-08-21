using HRVault.Application.WorkSchedules.DTOs;
using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.UpdateWorkSchedule;

public record UpdateWorkScheduleCommand(
    Guid Id,
    string Name,
    string? Description,
    int Type,
    int? RequiredWorkingDaysPerWeek,
    List<WorkScheduleDayInputDto> Days
) : IRequest;
