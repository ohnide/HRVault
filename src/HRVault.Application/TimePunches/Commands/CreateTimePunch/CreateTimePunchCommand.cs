using MediatR;

namespace HRVault.Application.TimePunches.Commands.CreateTimePunch;

public record CreateTimePunchCommand(
    Guid EmployeeId,
    int Direction = 0
) : IRequest<Guid>;
