using MediatR;

namespace HRVault.Application.Employees.Commands.UploadEmployeeDocument;

public record UploadEmployeeDocumentCommand(
    Guid EmployeeId,
    Guid EmployeeDocumentTypeId,
    DateOnly? IssueDate,
    DateOnly? ExpirationDate,
    string? Notes,
    string FileName,
    string ContentType,
    long Size,
    Stream Content
) : IRequest<Guid>;