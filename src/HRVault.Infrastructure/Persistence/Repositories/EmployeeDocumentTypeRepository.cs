using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeDocumentTypeRepository
    : IEmployeeDocumentTypeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeDocumentTypeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDocumentType>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DocumentTypes
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeDocumentType?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DocumentTypes
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.DocumentTypes
            .AsNoTracking()
            .Where(x =>
                x.CompanyId == companyId &&
                x.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(
            cancellationToken);
    }

    public async Task AddAsync(
        EmployeeDocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        await _context.DocumentTypes.AddAsync(
            documentType,
            cancellationToken);
    }

    public Task DeleteAsync(
        EmployeeDocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        _context.DocumentTypes.Remove(
            documentType);

        return Task.CompletedTask;
    }
}