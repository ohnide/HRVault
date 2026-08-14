using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeEmergencyContactRepository
{
    Task<EmployeeEmergencyContact?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeEmergencyContact contact,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        EmployeeEmergencyContact contact,
        CancellationToken cancellationToken = default);
}