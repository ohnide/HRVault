using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using MediatR;

namespace HRVault.Application.AuditLogs.Queries.SearchAuditLogs;

public class SearchAuditLogsQueryHandler
    : IRequestHandler<SearchAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchAuditLogsQueryHandler(
        IAuditLogRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        SearchAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        Guid? companyId;

        if (_currentUser.IsPlatformAdministrator)
        {
            companyId = request.Filter.CompanyId;
        }
        else
        {
            if (_currentUser.CompanyId is null)
                throw new UnauthorizedAccessException();

            companyId = _currentUser.CompanyId.Value;
        }

        return await _repository.SearchAsync(
            request.Filter,
            companyId,
            cancellationToken);
    }
}