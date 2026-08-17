namespace HRVault.Application.Employees.DTOs;

public class EmployeeDocumentDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long Size { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }
}