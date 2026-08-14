using HRVault.Application.Common.Interfaces;
using HRVault.Application.Positions.DTOs;
using MediatR;

namespace HRVault.Application.Positions.Queries.GetPositionById;

public class GetPositionByIdQueryHandler
    : IRequestHandler<GetPositionByIdQuery, PositionDto?>
{
    private readonly IPositionRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetPositionByIdQueryHandler(
        IPositionRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PositionDto?> Handle(
        GetPositionByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var position = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (position is null)
            return null;

        return new PositionDto
        {
            Id = position.Id,
            CompanyId = position.CompanyId,
            Code = position.Code,
            Name = position.Name,
            Description = position.Description,
            IsActive = position.IsActive
        };
    }
}