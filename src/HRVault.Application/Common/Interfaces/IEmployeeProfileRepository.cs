using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeProfileRepository
{
    Task<EmployeeProfile?> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeProfile profile,
        CancellationToken cancellationToken = default);
}