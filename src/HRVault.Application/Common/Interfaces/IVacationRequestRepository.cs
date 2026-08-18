using HRVault.Application.Common.Models;
using HRVault.Application.Vacations.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IVacationRequestRepository
{
    Task<VacationRequest?> GetByIdAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        VacationRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<VacationRequestDto>> SearchAsync(
        VacationRequestFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default);
		
	Task<bool> HasOverlapAsync(
		Guid employeeId,
		DateTime startDate,
		DateTime endDate,
		Guid companyId,
		CancellationToken cancellationToken = default);
		
	Task<decimal> GetApprovedDaysForYearAsync(
		Guid employeeId,
		int year,
		Guid companyId,
		Guid? excludeRequestId = null,
		CancellationToken cancellationToken = default);
}