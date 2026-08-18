using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IVacationBalanceRepository
{
    Task<VacationBalance?> GetByEmployeeAndYearAsync(
        Guid employeeId,
        int year,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        VacationBalance balance,
        CancellationToken cancellationToken = default);
}