using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeWorkScheduleRepository
    : IEmployeeWorkScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeWorkScheduleRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeWorkSchedule>> GetHistoryAsync(
        Guid employeeId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .AsNoTracking()
            .Include(x => x.WorkSchedule)
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeWorkSchedule?> GetAssignmentForDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeId == employeeId &&
                x.StartDate <= date &&
                (!x.EndDate.HasValue ||
                 x.EndDate.Value >= date))
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeWorkSchedule?> GetByStartDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly startDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .FirstOrDefaultAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.EmployeeId == employeeId &&
                    x.StartDate == startDate,
                cancellationToken);
    }

    public async Task<EmployeeWorkSchedule?> GetNextAssignmentAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly afterDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeId == employeeId &&
                x.StartDate > afterDate)
            .OrderBy(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkSchedule?> GetWorkScheduleAsync(
        Guid workScheduleId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == workScheduleId &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmployeeWorkSchedule assignment,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeWorkSchedules
            .AddAsync(
                assignment,
                cancellationToken);
    }

    public void Update(
        EmployeeWorkSchedule assignment)
    {
        _context.EmployeeWorkSchedules.Update(
            assignment);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }
}
