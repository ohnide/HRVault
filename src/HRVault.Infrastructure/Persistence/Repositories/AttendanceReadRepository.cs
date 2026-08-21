using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class AttendanceReadRepository
    : IAttendanceReadRepository
{
    private readonly ApplicationDbContext _context;

    public AttendanceReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeWorkSchedule?> GetEmployeeScheduleForDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeWorkSchedules
            .AsNoTracking()
            .Include(x => x.WorkSchedule)
                .ThenInclude(x => x.Days)
                    .ThenInclude(x => x.Periods)
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeId == employeeId &&
                x.StartDate <= date &&
                (!x.EndDate.HasValue ||
                 x.EndDate.Value >= date))
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
