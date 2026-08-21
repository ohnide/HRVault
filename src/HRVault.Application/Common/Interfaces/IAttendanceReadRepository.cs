using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IAttendanceReadRepository
{
    Task<EmployeeWorkSchedule?> GetEmployeeScheduleForDateAsync(
        Guid employeeId,
        Guid companyId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
