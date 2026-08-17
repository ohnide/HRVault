using HRVault.Application.Common.Interfaces;
using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.GetDocumentSummary;

public class GetDocumentSummaryQueryHandler
    : IRequestHandler<GetDocumentSummaryQuery, DocumentSummaryDto>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;

    public GetDocumentSummaryQueryHandler(
        IDocumentRepository documentRepository,
        ICurrentUserService currentUser)
    {
        _documentRepository = documentRepository;
        _currentUser = currentUser;
    }

    public async Task<DocumentSummaryDto> Handle(
        GetDocumentSummaryQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        return await _documentRepository.GetSummaryByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);
    }
}