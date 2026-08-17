using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeDocumentType;

public record CreateEmployeeDocumentTypeCommand(
    string Name,
    string? Description,
    bool HasExpiration,
    int? ExpirationWarningDays
) : IRequest<Guid>;