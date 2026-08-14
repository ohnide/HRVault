using HRVault.Application.Common.Models;

namespace HRVault.Application.Positions.DTOs;

public class PositionFilterDto : PagedRequest
{
    public Guid? CompanyId { get; set; }

    public string? Search { get; set; }

    public bool? IsActive { get; set; }
}