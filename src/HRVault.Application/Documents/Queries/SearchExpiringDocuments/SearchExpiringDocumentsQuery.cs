using HRVault.Application.Common.Models;
using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.SearchExpiringDocuments;

public record SearchExpiringDocumentsQuery(
    ExpiringDocumentFilterDto Filter
) : IRequest<PagedResult<ExpiringDocumentDto>>;