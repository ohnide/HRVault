using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeEmergencyContactRepository
    : IEmployeeEmergencyContactRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeEmergencyContactRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeEmergencyContact?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeEmergencyContacts
            .FirstOrDefaultAsync(
                x => x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task AddAsync(
        EmployeeEmergencyContact contact,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeEmergencyContacts.AddAsync(
            contact,
            cancellationToken);
    }

    public Task DeleteAsync(
        EmployeeEmergencyContact contact,
        CancellationToken cancellationToken = default)
    {
        _context.EmployeeEmergencyContacts.Remove(contact);

        return Task.CompletedTask;
    }
}