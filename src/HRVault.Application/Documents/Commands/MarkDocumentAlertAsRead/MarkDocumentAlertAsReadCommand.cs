using MediatR;

namespace HRVault.Application.Documents.Commands.MarkDocumentAlertAsRead;

public record MarkDocumentAlertAsReadCommand(
    Guid Id
) : IRequest;