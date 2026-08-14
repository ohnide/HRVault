using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class AuditLogRepository
    : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }
	
	public async Task<AuditLogDto?> GetByIdAsync(
		Guid id,
		Guid? companyId,
		CancellationToken cancellationToken = default)
	{
		var query = _context.AuditLogs
			.AsNoTracking()
			.Where(x => x.Id == id);

		if (companyId.HasValue)
		{
			query = query.Where(
				x => x.CompanyId == companyId.Value);
		}

		return await query
			.Select(x => new AuditLogDto
			{
				Id = x.Id,
				CompanyId = x.CompanyId,
				UserId = x.UserId,
				UserName = x.UserName,
				Action = x.Action,
				EntityName = x.EntityName,
				EntityId = x.EntityId,
				OldValues = x.OldValues,
				NewValues = x.NewValues,
				CreatedAt = x.CreatedAt,
				IpAddress = x.IpAddress
			})
			.FirstOrDefaultAsync(
				cancellationToken);
	}
	
    public async Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogFilterDto filter,
        Guid? companyId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId == companyId.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(
                x => x.UserId == filter.UserId.Value);
        }

        if (filter.EntityId.HasValue)
        {
            query = query.Where(
                x => x.EntityId == filter.EntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                filter.EntityName))
        {
            query = query.Where(
                x => x.EntityName == filter.EntityName);
        }

        if (!string.IsNullOrWhiteSpace(
                filter.Action))
        {
            query = query.Where(
                x => x.Action == filter.Action);
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(
                x => x.CreatedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(
                x => x.CreatedAt <= filter.DateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                filter.Search))
        {
            query = query.Where(x =>
                (x.UserName != null &&
                 x.UserName.Contains(filter.Search)) ||
                x.EntityName.Contains(filter.Search) ||
                x.Action.Contains(filter.Search));
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(
                (filter.Page - 1) *
                filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                UserId = x.UserId,
                UserName = x.UserName,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                CreatedAt = x.CreatedAt,
                IpAddress = x.IpAddress
            })
            .ToListAsync(
                cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}