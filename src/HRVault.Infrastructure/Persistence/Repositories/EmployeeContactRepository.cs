using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeContactRepository
    : IEmployeeContactRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeContactRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeContact?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeContacts
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmployeeContact contact,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeContacts.AddAsync(
            contact,
            cancellationToken);
    }

    public Task DeleteAsync(
        EmployeeContact contact,
        CancellationToken cancellationToken = default)
    {
        _context.EmployeeContacts.Remove(contact);

        return Task.CompletedTask;
    }
}