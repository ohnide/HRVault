using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.GetDocumentSummary;

public record GetDocumentSummaryQuery
    : IRequest<DocumentSummaryDto>;