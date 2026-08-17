using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.SearchExpiringDocuments;

public class SearchExpiringDocumentsQueryHandler
    : IRequestHandler<
        SearchExpiringDocumentsQuery,
        PagedResult<ExpiringDocumentDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;

    public SearchExpiringDocumentsQueryHandler(
        IDocumentRepository documentRepository,
        ICurrentUserService currentUser)
    {
        _documentRepository = documentRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<ExpiringDocumentDto>> Handle(
        SearchExpiringDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        return await _documentRepository.SearchByCompanyAsync(
            _currentUser.CompanyId.Value,
            request.Filter,
            cancellationToken);
    }
}