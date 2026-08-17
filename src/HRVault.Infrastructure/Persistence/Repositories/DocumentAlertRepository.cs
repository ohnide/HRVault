using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class DocumentAlertRepository
    : IDocumentAlertRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentAlertRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
		Guid documentId,
		CancellationToken cancellationToken = default)
	{
		return await _context.DocumentAlerts
			.AsNoTracking()
			.AnyAsync(
				x => x.DocumentId == documentId,
				cancellationToken);
	}

    public async Task AddAsync(
        DocumentAlert alert,
        CancellationToken cancellationToken = default)
    {
        await _context.DocumentAlerts.AddAsync(
            alert,
            cancellationToken);
    }

    public async Task<List<DocumentAlert>> GetPendingByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DocumentAlerts
            .AsNoTracking()
            .Include(x => x.Document)
                .ThenInclude(x => x.EmployeeDocumentType)
            .Include(x => x.Employee)
            .Where(x =>
                x.CompanyId == companyId &&
                x.Status == DocumentAlertStatus.Pending)
            .OrderBy(x => x.AlertDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<DocumentAlert?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DocumentAlerts
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id &&
                    x.CompanyId == companyId,
                cancellationToken);
    }
	
	public async Task<List<DocumentAlert>> GetUnsentByCompanyAsync(
		Guid companyId,
		CancellationToken cancellationToken = default)
	{
		return await _context.DocumentAlerts
			.Include(x => x.Document)
				.ThenInclude(x => x.EmployeeDocumentType)
			.Include(x => x.Employee)
			.Where(x =>
				x.CompanyId == companyId &&
				!x.EmailSent)
			.OrderBy(x => x.Document.ExpirationDate)
			.ToListAsync(cancellationToken);
	}
}