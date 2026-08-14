using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.AuditLogs.Queries.GetAuditLogById;

public class GetAuditLogByIdQueryHandler
    : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IAuditLogRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetAuditLogByIdQueryHandler(
        IAuditLogRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<AuditLogDto?> Handle(
        GetAuditLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        Guid? companyId;

        if (_currentUser.IsPlatformAdministrator)
        {
            companyId = null;
        }
        else
        {
            if (_currentUser.CompanyId is null)
                throw new UnauthorizedAccessException();

            companyId = _currentUser.CompanyId.Value;
        }

        return await _repository.GetByIdAsync(
            request.Id,
            companyId,
            cancellationToken);
    }
}