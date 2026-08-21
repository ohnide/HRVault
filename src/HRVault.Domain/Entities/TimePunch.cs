using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class TimePunch : AuditableEntity
{
    public Guid CompanyId { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public DateTime TimestampUtc { get; set; }

    public TimePunchSource Source { get; set; }
    public AttendanceEventDirection Direction { get; set; } = AttendanceEventDirection.Unknown;

    public Guid? AttendanceDeviceId { get; set; }
    public AttendanceDevice? AttendanceDevice { get; set; }

    public Guid? AttendanceEventId { get; set; }
    public AttendanceEvent? AttendanceEvent { get; set; }

    public string? AdjustmentReason { get; set; }

    public bool IsVoided { get; set; }
    public string? VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public Guid? VoidedBy { get; set; }
}
