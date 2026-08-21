using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeWorkScheduleRepository
{
    Task<List<EmployeeWorkSchedule>> GetHistoryAsync(
        Guid employeeId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<EmployeeWorkSchedule?> GetAssignmentForDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<EmployeeWorkSchedule?> GetByStartDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly startDate,
        CancellationToken cancellationToken = default);

    Task<EmployeeWorkSchedule?> GetNextAssignmentAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly afterDate,
        CancellationToken cancellationToken = default);

    Task<WorkSchedule?> GetWorkScheduleAsync(
        Guid workScheduleId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeWorkSchedule assignment,
        CancellationToken cancellationToken = default);

    void Update(EmployeeWorkSchedule assignment);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
