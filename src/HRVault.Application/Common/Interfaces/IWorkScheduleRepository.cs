using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IWorkScheduleRepository
{
    Task<List<WorkSchedule>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<WorkSchedule?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsAssignedAsync(
        Guid workScheduleId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        WorkSchedule workSchedule,
        CancellationToken cancellationToken = default);

    Task AddPeriodAsync(
        WorkSchedulePeriod period,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        WorkSchedule workSchedule,
        CancellationToken cancellationToken = default);

    Task DeletePeriodsAsync(
        IEnumerable<WorkSchedulePeriod> periods,
        CancellationToken cancellationToken = default);
}
