using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.Common.Models;
using MediatR;

namespace HRVault.Application.AuditLogs.Queries.SearchAuditLogs;

public record SearchAuditLogsQuery(
    AuditLogFilterDto Filter)
    : IRequest<PagedResult<AuditLogDto>>;