using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.SearchEmployees;

public record SearchEmployeesQuery(
    EmployeeFilterDto Filter)
    : IRequest<PagedResult<EmployeeListDto>>;