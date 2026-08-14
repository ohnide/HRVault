using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeAddressRepository
    : IEmployeeAddressRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeAddressRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeAddress?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeAddresses
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmployeeAddress address,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeAddresses.AddAsync(
            address,
            cancellationToken);
    }

    public Task DeleteAsync(
        EmployeeAddress address,
        CancellationToken cancellationToken = default)
    {
        _context.EmployeeAddresses.Remove(address);

        return Task.CompletedTask;
    }
}