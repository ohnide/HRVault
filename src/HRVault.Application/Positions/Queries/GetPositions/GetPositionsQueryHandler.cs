using HRVault.Application.Common.Interfaces;
using HRVault.Application.Positions.DTOs;
using MediatR;

namespace HRVault.Application.Positions.Queries.GetPositions;

public class GetPositionsQueryHandler
    : IRequestHandler<GetPositionsQuery, List<PositionDto>>
{
    private readonly IPositionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetPositionsQueryHandler(
        IPositionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<PositionDto>> Handle(
        GetPositionsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var positions = await _repository.GetAllByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);

        return positions
            .Select(p => new PositionDto
            {
                Id = p.Id,
                CompanyId = p.CompanyId,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive
            })
            .ToList();
    }
}