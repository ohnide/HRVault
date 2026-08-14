using HRVault.Application.Positions.DTOs;
using MediatR;

namespace HRVault.Application.Positions.Queries.GetPositionById;

public class GetPositionByIdQuery : IRequest<PositionDto?>
{
    public Guid Id { get; }

    public GetPositionByIdQuery(Guid id)
    {
        Id = id;
    }
}