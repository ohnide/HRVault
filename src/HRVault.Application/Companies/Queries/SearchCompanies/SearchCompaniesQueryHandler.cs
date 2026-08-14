using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Companies.DTOs;
using HRVault.Application.Common.Exceptions;
using MediatR;

namespace HRVault.Application.Companies.Queries.SearchCompanies;

public class SearchCompaniesQueryHandler
    : IRequestHandler<SearchCompaniesQuery, PagedResult<CompanyDto>>
{
    private readonly ICompanyRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchCompaniesQueryHandler(
        ICompanyRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<CompanyDto>> Handle(
        SearchCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (!_currentUser.IsPlatformAdministrator)
        {
            throw new ForbiddenException(
                "Platform administrator access is required.");
        }

        var filter = new CompanyFilterDto
        {
            Search = request.Search,
            VatNumber = request.VatNumber,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return await _repository.SearchAsync(
            filter,
            cancellationToken);
    }
}