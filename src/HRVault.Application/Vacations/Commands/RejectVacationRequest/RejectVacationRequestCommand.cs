using MediatR;

namespace HRVault.Application.Vacations.Commands.RejectVacationRequest;

public record RejectVacationRequestCommand(
    Guid Id
) : IRequest;