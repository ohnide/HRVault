using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeDocument;

public record DeleteEmployeeDocumentCommand(
    Guid EmployeeId,
    Guid DocumentId
) : IRequest;