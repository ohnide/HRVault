using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeDocumentType;

public record DeleteEmployeeDocumentTypeCommand(
    Guid Id
) : IRequest;