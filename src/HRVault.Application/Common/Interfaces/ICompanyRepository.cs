using HRVault.Application.Common.Models;
using HRVault.Application.Companies.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<PagedResult<CompanyDto>> SearchAsync(
        CompanyFilterDto filter,
        CancellationToken cancellationToken = default);
}