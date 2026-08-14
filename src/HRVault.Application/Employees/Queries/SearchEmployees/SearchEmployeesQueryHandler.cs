using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.SearchEmployees;

public class SearchEmployeesQueryHandler
    : IRequestHandler<SearchEmployeesQuery, PagedResult<EmployeeListDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchEmployeesQueryHandler(
        IEmployeeRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<EmployeeListDto>> Handle(
        SearchEmployeesQuery request,
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