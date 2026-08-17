using MediatR;

namespace HRVault.Application.Documents.Commands.DismissDocumentAlert;

public record DismissDocumentAlertCommand(
    Guid Id
) : IRequest;