using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HRVault.Application.Common.Models;
using HRVault.Application.Vacations.DTOs;
using HRVault.Domain.Enums;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class VacationRequestRepository
    : IVacationRequestRepository
{
    private readonly ApplicationDbContext _context;

    public VacationRequestRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacationRequest?> GetByIdAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VacationRequests
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task AddAsync(
        VacationRequest request,
        CancellationToken cancellationToken = default)
    {
        await _context.VacationRequests.AddAsync(
            request,
            cancellationToken);
    }
	
	public async Task<PagedResult<VacationRequestDto>> SearchAsync(
		VacationRequestFilterDto filter,
		Guid companyId,
		CancellationToken cancellationToken = default)
	{
		var query = _context.VacationRequests
			.AsNoTracking()
			.Include(x => x.Employee)
			.Where(x => x.CompanyId == companyId)
			.AsQueryable();

		if (filter.EmployeeId.HasValue)
		{
			query = query.Where(
				x => x.EmployeeId == filter.EmployeeId.Value);
		}

		if (filter.DepartmentId.HasValue)
		{
			query = query.Where(
				x => x.Employee.DepartmentId == filter.DepartmentId.Value);
		}

		if (!string.IsNullOrWhiteSpace(filter.Status))
		{
			if (Enum.TryParse<VacationRequestStatus>(
					filter.Status,
					true,
					out var status))
			{
				query = query.Where(
					x => x.Status == status);
			}
			else
			{
				query = query.Where(x => false);
			}
		}

		if (filter.Year.HasValue)
		{
			var yearStart = new DateTime(
				filter.Year.Value,
				1,
				1,
				0,
				0,
				0,
				DateTimeKind.Utc);

			var yearEnd = new DateTime(
				filter.Year.Value,
				12,
				31,
				23,
				59,
				59,
				DateTimeKind.Utc);

			query = query.Where(
				x =>
					x.StartDate <= yearEnd &&
					x.EndDate >= yearStart);
		}

		var totalCount =
			await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(x => x.StartDate)
			.ThenBy(x => x.Employee.LastName)
			.ThenBy(x => x.Employee.FirstName)
			.Skip((filter.Page - 1) * filter.PageSize)
			.Take(filter.PageSize)
			.Select(x => new VacationRequestDto
			{
				Id = x.Id,
				EmployeeId = x.EmployeeId,
				EmployeeName =
					x.Employee.FirstName + " " +
					x.Employee.LastName,
				StartDate = x.StartDate,
				EndDate = x.EndDate,
				Days = x.Days,
				Status = x.Status.ToString(),
				Notes = x.Notes,
				ApprovedAt = x.ApprovedAt,
				ApprovedBy = x.ApprovedBy
			})
			.ToListAsync(cancellationToken);

		return new PagedResult<VacationRequestDto>
		{
			Items = items,
			TotalCount = totalCount,
			Page = filter.Page,
			PageSize = filter.PageSize
		};
	}
	
	public async Task<bool> HasOverlapAsync(
		Guid employeeId,
		DateTime startDate,
		DateTime endDate,
		Guid companyId,
		CancellationToken cancellationToken = default)
	{
		return await _context.VacationRequests
			.AnyAsync(
				x =>
					x.CompanyId == companyId &&
					x.EmployeeId == employeeId &&
					x.Status != VacationRequestStatus.Rejected &&
					x.Status != VacationRequestStatus.Cancelled &&
					x.StartDate <= endDate &&
					x.EndDate >= startDate,
				cancellationToken);
	}
	
	public async Task<decimal> GetApprovedDaysForYearAsync(
		Guid employeeId,
		int year,
		Guid companyId,
		Guid? excludeRequestId = null,
		CancellationToken cancellationToken = default)
	{
		var yearStart = new DateTime(
			year,
			1,
			1,
			0,
			0,
			0,
			DateTimeKind.Utc);

		var yearEnd = new DateTime(
			year,
			12,
			31,
			23,
			59,
			59,
			DateTimeKind.Utc);

		var query = _context.VacationRequests
			.AsNoTracking()
			.Where(x =>
				x.CompanyId == companyId &&
				x.EmployeeId == employeeId &&
				x.Status == VacationRequestStatus.Approved &&
				x.StartDate <= yearEnd &&
				x.EndDate >= yearStart);

		if (excludeRequestId.HasValue)
		{
			query = query.Where(
				x => x.Id != excludeRequestId.Value);
		}

		return await query.SumAsync(
			x => (decimal?)x.Days,
			cancellationToken) ?? 0;
	}
}