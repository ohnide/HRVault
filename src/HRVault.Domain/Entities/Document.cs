using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class Document : SoftDeleteEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string Category { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string StorageName { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long Size { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}