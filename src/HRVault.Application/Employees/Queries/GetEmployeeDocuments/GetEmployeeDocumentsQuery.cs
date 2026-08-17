using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDocuments;

public record GetEmployeeDocumentsQuery(
    Guid EmployeeId
) : IRequest<List<EmployeeDocumentDto>>;