using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeDocumentType;

public record UpdateEmployeeDocumentTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    bool HasExpiration,
    int? ExpirationWarningDays
) : IRequest;