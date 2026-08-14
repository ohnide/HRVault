using HRVault.Application.Positions.DTOs;
using MediatR;

namespace HRVault.Application.Positions.Queries.GetPositions;

public class GetPositionsQuery : IRequest<List<PositionDto>>
{
}