using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class TimePunchRepository
    : ITimePunchRepository
{
    private readonly ApplicationDbContext _context;

    public TimePunchRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmployeeExistsInCompanyAsync(
        Guid employeeId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == employeeId &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<bool> HasRecentPunchAsync(
        Guid employeeId,
        Guid companyId,
        DateTime timestampUtc,
        TimeSpan tolerance,
        CancellationToken cancellationToken = default)
    {
        var fromUtc =
            timestampUtc.Subtract(tolerance);

        var toUtc =
            timestampUtc.Add(tolerance);

        return await _context.TimePunches
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.EmployeeId == employeeId &&
                    !x.IsVoided &&
                    x.TimestampUtc >= fromUtc &&
                    x.TimestampUtc <= toUtc,
                cancellationToken);
    }

    public async Task AddAsync(
        TimePunch punch,
        CancellationToken cancellationToken = default)
    {
        await _context.TimePunches.AddAsync(
            punch,
            cancellationToken);
    }

    public async Task<List<TimePunch>> GetEmployeePunchesAsync(
        Guid employeeId,
        Guid companyId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.TimePunches
            .AsNoTracking()
            .Include(x => x.Employee)
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeId == employeeId &&
                x.TimestampUtc >= fromUtc &&
                x.TimestampUtc < toUtc)
            .OrderByDescending(x => x.TimestampUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TimePunch>> GetCompanyPunchesAsync(
        Guid companyId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.TimePunches
            .AsNoTracking()
            .Include(x => x.Employee)
            .Where(x =>
                x.CompanyId == companyId &&
                x.TimestampUtc >= fromUtc &&
                x.TimestampUtc < toUtc)
            .OrderByDescending(x => x.TimestampUtc)
            .ToListAsync(cancellationToken);
    }
}
