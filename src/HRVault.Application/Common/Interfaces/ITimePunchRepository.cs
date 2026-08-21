using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface ITimePunchRepository
{
    Task<bool> EmployeeExistsInCompanyAsync(
        Guid employeeId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> HasRecentPunchAsync(
        Guid employeeId,
        Guid companyId,
        DateTime timestampUtc,
        TimeSpan tolerance,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TimePunch punch,
        CancellationToken cancellationToken = default);

    Task<List<TimePunch>> GetEmployeePunchesAsync(
        Guid employeeId,
        Guid companyId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);

    Task<List<TimePunch>> GetCompanyPunchesAsync(
        Guid companyId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);
}
