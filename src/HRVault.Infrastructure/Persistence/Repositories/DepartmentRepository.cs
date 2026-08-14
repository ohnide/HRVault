using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Departments.DTOs;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class DepartmentRepository
    : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PagedResult<DepartmentDto>> SearchAsync(
        DepartmentFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Departments
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.Name.Contains(filter.Search));
        }

        var total = await query.CountAsync(
            cancellationToken);

        var departments = await query
            .OrderBy(x => x.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new DepartmentDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Name = x.Name,
                Description = x.Description,
                ParentDepartmentId = x.ParentDepartmentId
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<DepartmentDto>
        {
            Items = departments,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Department?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<List<Department>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Departments
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> WouldCreateCycleAsync(
        Guid departmentId,
        Guid parentDepartmentId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        if (departmentId == parentDepartmentId)
            return true;

        var currentParentId = parentDepartmentId;

        while (true)
        {
            var parent = await Context.Departments
                .AsNoTracking()
                .Where(x =>
                    x.Id == currentParentId &&
                    x.CompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.ParentDepartmentId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (parent is null)
                return false;

            if (parent.Id == departmentId)
                return true;

            if (!parent.ParentDepartmentId.HasValue)
                return false;

            currentParentId = parent.ParentDepartmentId.Value;
        }
    }
}