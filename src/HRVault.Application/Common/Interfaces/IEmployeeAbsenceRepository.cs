using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Models;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeAbsenceRepository
{
    Task<EmployeeAbsence?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<EmployeeAbsenceDto>> SearchAsync(
        EmployeeAbsenceFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        Guid employeeId,
        DateTime startDateTime,
        DateTime endDateTime,
        Guid companyId,
        Guid? excludeAbsenceId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeAbsence absence,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        EmployeeAbsence absence,
        CancellationToken cancellationToken = default);
}