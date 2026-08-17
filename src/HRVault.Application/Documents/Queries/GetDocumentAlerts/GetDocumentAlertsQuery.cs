using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.GetDocumentAlerts;

public record GetDocumentAlertsQuery
    : IRequest<List<DocumentAlertDto>>;