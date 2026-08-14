namespace HRVault.Application.AuditLogs.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? UserId { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public Guid? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? IpAddress { get; set; }
}