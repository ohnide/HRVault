using MediatR;

namespace HRVault.Application.TimePunches.Commands.CreateManualTimePunch;

public record CreateManualTimePunchCommand(
    Guid EmployeeId,
    DateOnly LocalDate,
    TimeOnly LocalTime,
    int Direction,
    string Reason
) : IRequest<Guid>;
