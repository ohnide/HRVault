using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class VacationBalanceRepository
    : IVacationBalanceRepository
{
    private readonly ApplicationDbContext _context;

    public VacationBalanceRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacationBalance?> GetByEmployeeAndYearAsync(
        Guid employeeId,
        int year,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VacationBalances
            .FirstOrDefaultAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.EmployeeId == employeeId &&
                    x.Year == year,
                cancellationToken);
    }

    public async Task AddAsync(
        VacationBalance balance,
        CancellationToken cancellationToken = default)
    {
        await _context.VacationBalances.AddAsync(
            balance,
            cancellationToken);
    }
}