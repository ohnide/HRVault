using HRVault.Application.AuditLogs.DTOs;
using MediatR;

namespace HRVault.Application.AuditLogs.Queries.GetAuditLogById;

public record GetAuditLogByIdQuery(
    Guid Id)
    : IRequest<AuditLogDto?>;