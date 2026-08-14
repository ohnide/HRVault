using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeAddressRepository
{
    Task<EmployeeAddress?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeAddress address,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        EmployeeAddress address,
        CancellationToken cancellationToken = default);
}