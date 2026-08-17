using MediatR;

namespace HRVault.Application.Documents.Commands.GenerateDocumentAlerts;

public record GenerateDocumentAlertsCommand
    : IRequest<int>;