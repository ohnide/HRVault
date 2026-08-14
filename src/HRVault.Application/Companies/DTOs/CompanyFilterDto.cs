using HRVault.Application.Common.Models;

namespace HRVault.Application.Companies.DTOs;

public class CompanyFilterDto : PagedFilter
{
    public string? VatNumber { get; set; }
}