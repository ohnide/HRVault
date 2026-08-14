using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeRepository
    : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<Employee>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Employees
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAndCompanyAsync(
		Guid id,
		Guid companyId,
		CancellationToken cancellationToken = default)
	{
		return await Context.Employees
			.FirstOrDefaultAsync(
				x => x.Id == id &&
					 x.CompanyId == companyId,
				cancellationToken);
	}

    public async Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid companyId,
        Guid? excludeEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Employees
            .AsNoTracking()
            .Where(x =>
                x.CompanyId == companyId &&
                x.EmployeeNumber == employeeNumber);

        if (excludeEmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.Id != excludeEmployeeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

	public async Task<Employee?> GetDetailsByIdAndCompanyAsync(
		Guid id,
		Guid companyId,
		CancellationToken cancellationToken = default)
	{
		return await Context.Employees
			.AsNoTracking()
			.Include(x => x.Department)
			.Include(x => x.Position)
			.Include(x => x.Profile)
			.Include(x => x.Addresses)
			.Include(x => x.Contacts)
			.Include(x => x.EmergencyContact)
			.FirstOrDefaultAsync(
				x => x.Id == id &&
					 x.CompanyId == companyId,
				cancellationToken);
	}

    public async Task<PagedResult<EmployeeListDto>> SearchAsync(
        EmployeeFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Position)
            .Where(x => x.CompanyId == companyId)
            .AsQueryable();

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.DepartmentId == filter.DepartmentId);
        }

        if (filter.PositionId.HasValue)
        {
            query = query.Where(x =>
                x.PositionId == filter.PositionId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.FirstName.Contains(filter.Search) ||
                x.LastName.Contains(filter.Search) ||
                x.EmployeeNumber.Contains(filter.Search));
        }

        var total = await query.CountAsync(
            cancellationToken);

        var employees = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new EmployeeListDto
            {
                Id = x.Id,
                EmployeeNumber = x.EmployeeNumber,
                FullName = x.FirstName + " " + x.LastName,
                Department = x.Department != null
                    ? x.Department.Name
                    : null,
                Position = x.Position != null
                    ? x.Position.Name
                    : null,
                Status = x.Status.ToString(),
                HireDate = x.HireDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeListDto>
        {
            Items = employees,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}