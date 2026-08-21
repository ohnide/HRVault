using MediatR;
namespace HRVault.Application.WorkSchedules.Commands.SetWorkScheduleActive;
public record SetWorkScheduleActiveCommand(Guid Id, bool IsActive) : IRequest;
