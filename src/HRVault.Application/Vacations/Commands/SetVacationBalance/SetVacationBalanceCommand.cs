using MediatR;

namespace HRVault.Application.Vacations.Commands.SetVacationBalance;

public record SetVacationBalanceCommand(
    Guid EmployeeId,
    int Year,
    decimal AdjustmentDays,
    string? Notes
) : IRequest<Guid>;