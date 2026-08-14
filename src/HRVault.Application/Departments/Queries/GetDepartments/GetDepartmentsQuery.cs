using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery()
    : IRequest<List<DepartmentDto>>;