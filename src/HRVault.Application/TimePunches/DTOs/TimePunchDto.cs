namespace HRVault.Application.TimePunches.DTOs;

public class TimePunchDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; }

    public int Source { get; set; }
    public string SourceName { get; set; } = string.Empty;

    public int Direction { get; set; }
    public string DirectionName { get; set; } = string.Empty;

    public Guid? AttendanceDeviceId { get; set; }

    public bool IsVoided { get; set; }
    public string? VoidReason { get; set; }

    public DateTime CreatedAt { get; set; }
}
