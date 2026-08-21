using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeDeviceIdentity : AuditableEntity
{
    public Guid CompanyId { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public Guid AttendanceDeviceId { get; set; }
    public AttendanceDevice AttendanceDevice { get; set; } = null!;

    public string ExternalUserId { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public bool IsActive { get; set; } = true;
}
