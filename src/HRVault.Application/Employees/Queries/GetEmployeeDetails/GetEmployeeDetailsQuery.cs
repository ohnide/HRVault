using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDetails;

public record GetEmployeeDetailsQuery(
    Guid Id)
    : IRequest<EmployeeDetailsDto?>;