using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class AttendanceEvent : AuditableEntity
{
    public Guid CompanyId { get; set; }

    public Guid AttendanceDeviceId { get; set; }
    public AttendanceDevice AttendanceDevice { get; set; } = null!;

    public string ExternalEventId { get; set; } = string.Empty;
    public string ExternalUserId { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; }

    public AttendanceEventDirection Direction { get; set; } = AttendanceEventDirection.Unknown;

    public string? ReaderCode { get; set; }
    public string? RawPayload { get; set; }

    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessingError { get; set; }

    public ICollection<TimePunch> TimePunches { get; set; } = new List<TimePunch>();
}
