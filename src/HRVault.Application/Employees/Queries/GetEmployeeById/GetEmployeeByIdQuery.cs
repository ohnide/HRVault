using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id)
    : IRequest<EmployeeDto?>;