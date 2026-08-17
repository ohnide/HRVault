using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Companies.DTOs;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class CompanyRepository
    : BaseRepository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PagedResult<CompanyDto>> SearchAsync(
        CompanyFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Companies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.Name.Contains(filter.Search));
        }

        if (!string.IsNullOrWhiteSpace(filter.VatNumber))
        {
            query = query.Where(x =>
                x.VatNumber.Contains(filter.VatNumber));
        }

        var total = await query.CountAsync(cancellationToken);

        var companies = await query
            .OrderBy(x => x.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                VatNumber = x.VatNumber,
                Address = x.Address,
                LogoUrl = x.LogoUrl
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<CompanyDto>
        {
            Items = companies,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
	
	public async Task<List<Company>> GetAllActiveAsync(
		CancellationToken cancellationToken = default)
	{
		return await Context.Companies
			.AsNoTracking()
			.OrderBy(x => x.Name)
			.ToListAsync(cancellationToken);
	}
}