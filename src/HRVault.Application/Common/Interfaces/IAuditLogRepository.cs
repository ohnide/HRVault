using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.Common.Models;

namespace HRVault.Application.Common.Interfaces;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogFilterDto filter,
        Guid? companyId,
        CancellationToken cancellationToken = default);

    Task<AuditLogDto?> GetByIdAsync(
        Guid id,
        Guid? companyId,
        CancellationToken cancellationToken = default);
}