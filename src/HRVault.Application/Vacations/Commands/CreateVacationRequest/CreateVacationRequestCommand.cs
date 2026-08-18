using MediatR;

namespace HRVault.Application.Vacations.Commands.CreateVacationRequest;

public record CreateVacationRequestCommand(
    Guid EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string? Notes
) : IRequest<Guid>;