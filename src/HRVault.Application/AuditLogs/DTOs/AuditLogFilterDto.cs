namespace HRVault.Application.AuditLogs.DTOs;

public class AuditLogFilterDto
{
    public Guid? CompanyId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? EntityId { get; set; }

    public string? EntityName { get; set; }

    public string? Action { get; set; }

    public string? Search { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}