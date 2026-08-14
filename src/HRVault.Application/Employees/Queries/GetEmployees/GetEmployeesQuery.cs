using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery()
    : IRequest<List<EmployeeDto>>;