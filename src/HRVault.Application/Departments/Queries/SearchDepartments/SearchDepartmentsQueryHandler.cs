using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.SearchDepartments;

public class SearchDepartmentsQueryHandler
    : IRequestHandler<SearchDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly IDepartmentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchDepartmentsQueryHandler(
        IDepartmentRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<DepartmentDto>> Handle(
        SearchDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var filter = new DepartmentFilterDto
        {
            Search = request.Search,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return await _repository.SearchAsync(
            filter,
            _currentUser.CompanyId.Value,
            cancellationToken);
    }
}