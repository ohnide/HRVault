using MediatR;

namespace HRVault.Application.Positions.Commands.DeletePosition;

public record DeletePositionCommand(Guid Id) : IRequest;