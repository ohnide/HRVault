using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class DocumentRepository
    : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task<List<Document>> GetAllByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AsNoTracking()
			.Include(x => x.EmployeeDocumentType)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(
            document,
            cancellationToken);
    }

    public Task DeleteAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        _context.Documents.Remove(document);

        return Task.CompletedTask;
    }
	
	public async Task<PagedResult<EmployeeDocumentDto>> SearchByEmployeeAsync(
		Guid employeeId,
		EmployeeDocumentFilterDto filter,
		CancellationToken cancellationToken = default)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);

		var query = _context.Documents
			.AsNoTracking()
			.Include(x => x.EmployeeDocumentType)
			.Where(x => x.EmployeeId == employeeId)
			.AsQueryable();

		if (filter.EmployeeDocumentTypeId.HasValue)
		{
			query = query.Where(
				x => x.EmployeeDocumentTypeId ==
					 filter.EmployeeDocumentTypeId.Value);
		}

		if (filter.DateFrom.HasValue)
		{
			query = query.Where(
				x => x.ExpirationDate >= filter.DateFrom.Value);
		}

		if (filter.DateTo.HasValue)
		{
			query = query.Where(
				x => x.ExpirationDate <= filter.DateTo.Value);
		}

		if (!string.IsNullOrWhiteSpace(filter.Status))
		{
			var status = filter.Status.Trim().ToLower();

			switch (status)
			{
				case "expired":
					query = query.Where(x =>
						x.ExpirationDate.HasValue &&
						x.ExpirationDate.Value < today);
					break;

				case "expiring":
					query = query.Where(x =>
						x.ExpirationDate.HasValue &&
						x.ExpirationDate.Value >= today &&
						x.EmployeeDocumentType
							.ExpirationWarningDays.HasValue &&
						x.ExpirationDate.Value <=
							today.AddDays(
								x.EmployeeDocumentType
									.ExpirationWarningDays.Value));
					break;

				case "valid":
					query = query.Where(x =>
						!x.ExpirationDate.HasValue ||
						!x.EmployeeDocumentType
							.ExpirationWarningDays.HasValue ||
						x.ExpirationDate.Value >
							today.AddDays(
								x.EmployeeDocumentType
									.ExpirationWarningDays.Value));
					break;
			}
		}

		var totalCount =
			await query.CountAsync(cancellationToken);

		var items = await query
			.OrderBy(x => x.ExpirationDate)
			.ThenByDescending(x => x.UploadedAt)
			.Skip(
				(filter.Page - 1) *
				filter.PageSize)
			.Take(filter.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedResult<EmployeeDocumentDto>
		{
			Items = items
				.Select(x =>
				{
					var status = "Valid";

					if (x.ExpirationDate.HasValue)
					{
						if (x.ExpirationDate.Value < today)
						{
							status = "Expired";
						}
						else if (
							x.EmployeeDocumentType
								.ExpirationWarningDays.HasValue &&
							x.ExpirationDate.Value <=
								today.AddDays(
									x.EmployeeDocumentType
										.ExpirationWarningDays.Value))
						{
							status = "Expiring";
						}
					}

					return new EmployeeDocumentDto
					{
						Id = x.Id,
						EmployeeId = x.EmployeeId,
						EmployeeDocumentTypeId =
							x.EmployeeDocumentTypeId,
						EmployeeDocumentTypeName =
							x.EmployeeDocumentType.Name,
						IssueDate = x.IssueDate,
						ExpirationDate = x.ExpirationDate,
						Notes = x.Notes,
						FileName = x.FileName,
						MimeType = x.MimeType,
						Size = x.Size,
						UploadedByUserId =
							x.UploadedByUserId,
						UploadedAt = x.UploadedAt,
						Status = status
					};
				})
				.ToList(),

			TotalCount = totalCount,
			Page = filter.Page,
			PageSize = filter.PageSize
		};
	}
}