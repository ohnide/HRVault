using MediatR;

namespace HRVault.Application.Employees.Commands.UploadEmployeeDocument;

public record UploadEmployeeDocumentCommand(
    Guid EmployeeId,
    string Category,
    string FileName,
    string ContentType,
    long Size,
    Stream Content
) : IRequest<Guid>;