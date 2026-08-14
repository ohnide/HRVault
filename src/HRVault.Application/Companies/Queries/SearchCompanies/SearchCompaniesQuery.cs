using HRVault.Application.Common.Models;
using HRVault.Application.Companies.DTOs;
using MediatR;

namespace HRVault.Application.Companies.Queries.SearchCompanies;

public class SearchCompaniesQuery
    : IRequest<PagedResult<CompanyDto>>
{
    public string? Search { get; set; }

    public string? VatNumber { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}