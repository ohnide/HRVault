using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class Document : SoftDeleteEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid EmployeeDocumentTypeId { get; set; }

    public EmployeeDocumentType EmployeeDocumentType { get; set; } = null!;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Notes { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string StorageName { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long Size { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}