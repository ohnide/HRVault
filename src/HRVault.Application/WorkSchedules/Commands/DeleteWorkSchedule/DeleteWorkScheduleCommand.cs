using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.DeleteWorkSchedule;

public record DeleteWorkScheduleCommand(
    Guid Id
) : IRequest;
