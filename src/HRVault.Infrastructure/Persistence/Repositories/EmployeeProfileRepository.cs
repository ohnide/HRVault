using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeProfileRepository
    : IEmployeeProfileRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeProfileRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeProfile?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeProfiles
            .FirstOrDefaultAsync(
                x => x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmployeeProfile profile,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeProfiles.AddAsync(
            profile,
            cancellationToken);
    }
}