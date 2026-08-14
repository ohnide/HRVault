namespace HRVault.Application.Common.Models;

public abstract class PagedRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}