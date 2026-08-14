using HRVault.Application.Common.Models;
using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.SearchDepartments;

public class SearchDepartmentsQuery
    : IRequest<PagedResult<DepartmentDto>>
{
    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}