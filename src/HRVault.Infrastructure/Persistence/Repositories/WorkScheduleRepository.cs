using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class WorkScheduleRepository
    : IWorkScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public WorkScheduleRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkSchedule>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .AsNoTracking()
            .Include(x => x.Days)
                .ThenInclude(x => x.Periods)
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkSchedule?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .Include(x => x.Days)
                .ThenInclude(x => x.Periods)
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
        var normalizedName = name.Trim();

        var query = _context.WorkSchedules
            .AsNoTracking()
            .Where(x =>
                x.CompanyId == companyId &&
                x.Name == normalizedName);

        if (excludeId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(
            cancellationToken);
    }

    public async Task<bool> IsAssignedAsync(
        Guid workScheduleId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.WorkScheduleId == workScheduleId &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task AddAsync(
        WorkSchedule workSchedule,
        CancellationToken cancellationToken = default)
    {
        await _context.WorkSchedules.AddAsync(
            workSchedule,
            cancellationToken);
    }

    public async Task AddPeriodAsync(
        WorkSchedulePeriod period,
        CancellationToken cancellationToken = default)
    {
        // Adição explícita ao DbSet para garantir EntityState.Added.
        await _context.WorkSchedulePeriods.AddAsync(
            period,
            cancellationToken);
    }

    public Task DeleteAsync(
        WorkSchedule workSchedule,
        CancellationToken cancellationToken = default)
    {
        _context.WorkSchedules.Remove(workSchedule);
        return Task.CompletedTask;
    }

    public Task DeletePeriodsAsync(
        IEnumerable<WorkSchedulePeriod> periods,
        CancellationToken cancellationToken = default)
    {
        _context.WorkSchedulePeriods.RemoveRange(periods);
        return Task.CompletedTask;
    }
}
