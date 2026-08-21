using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class AttendanceDevice : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SettingsJson { get; set; }

    public ICollection<EmployeeDeviceIdentity> EmployeeIdentities { get; set; } = new List<EmployeeDeviceIdentity>();
    public ICollection<AttendanceEvent> Events { get; set; } = new List<AttendanceEvent>();
    public ICollection<TimePunch> TimePunches { get; set; } = new List<TimePunch>();
}
