using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeContactRepository
{
    Task<EmployeeContact?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeContact contact,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        EmployeeContact contact,
        CancellationToken cancellationToken = default);
}