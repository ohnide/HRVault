namespace HRVault.Application.Common.Models;

public abstract class PagedFilter
{
    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}