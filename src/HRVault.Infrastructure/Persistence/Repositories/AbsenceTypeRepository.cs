using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class AbsenceTypeRepository
    : IAbsenceTypeRepository
{
    private readonly ApplicationDbContext _context;

    public AbsenceTypeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AbsenceType>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AbsenceTypes
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AbsenceType?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AbsenceTypes
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.AbsenceTypes
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(
                x =>
                    x.Id != excludeId.Value);
        }

        return await query.AnyAsync(
            cancellationToken);
    }

    public async Task AddAsync(
        AbsenceType absenceType,
        CancellationToken cancellationToken = default)
    {
        await _context.AbsenceTypes.AddAsync(
            absenceType,
            cancellationToken);
    }

    public Task DeleteAsync(
        AbsenceType absenceType,
        CancellationToken cancellationToken = default)
    {
        _context.AbsenceTypes.Remove(
            absenceType);

        return Task.CompletedTask;
    }
}