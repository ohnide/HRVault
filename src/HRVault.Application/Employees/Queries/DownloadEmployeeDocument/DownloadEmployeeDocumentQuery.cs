using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.DownloadEmployeeDocument;

public record DownloadEmployeeDocumentQuery(
    Guid EmployeeId,
    Guid DocumentId
) : IRequest<EmployeeDocumentDownloadDto>;