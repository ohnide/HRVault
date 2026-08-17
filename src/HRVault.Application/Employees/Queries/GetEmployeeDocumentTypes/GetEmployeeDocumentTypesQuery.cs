using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDocumentTypes;

public record GetEmployeeDocumentTypesQuery
    : IRequest<List<EmployeeDocumentTypeDto>>;