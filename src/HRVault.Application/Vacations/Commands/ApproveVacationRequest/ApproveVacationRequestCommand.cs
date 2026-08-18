using MediatR;

namespace HRVault.Application.Vacations.Commands.ApproveVacationRequest;

public record ApproveVacationRequestCommand(
    Guid Id
) : IRequest;