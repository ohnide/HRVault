using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class AuditLog : SoftDeleteEntity
{
    public Guid UserId { get; set; }

    public string TableName { get; set; } = string.Empty;

    public string RecordId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IPAddress { get; set; }
}