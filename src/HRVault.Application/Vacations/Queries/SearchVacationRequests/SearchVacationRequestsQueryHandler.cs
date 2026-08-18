using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Vacations.DTOs;
using MediatR;

namespace HRVault.Application.Vacations.Queries.SearchVacationRequests;

public class SearchVacationRequestsQueryHandler
    : IRequestHandler<
        SearchVacationRequestsQuery,
        PagedResult<VacationRequestDto>>
{
    private readonly IVacationRequestRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchVacationRequestsQueryHandler(
        IVacationRequestRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<VacationRequestDto>> Handle(
        SearchVacationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        return await _repository.SearchAsync(
            request.Filter,
            _currentUser.CompanyId.Value,
            cancellationToken);
    }
}