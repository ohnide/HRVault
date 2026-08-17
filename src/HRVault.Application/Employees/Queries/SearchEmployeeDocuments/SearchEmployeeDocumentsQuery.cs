using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.SearchEmployeeDocuments;

public record SearchEmployeeDocumentsQuery(
    Guid EmployeeId,
    EmployeeDocumentFilterDto Filter
) : IRequest<PagedResult<EmployeeDocumentDto>>;