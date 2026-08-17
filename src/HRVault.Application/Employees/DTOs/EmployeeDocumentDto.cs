namespace HRVault.Application.Employees.DTOs;

public class EmployeeDocumentDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid EmployeeDocumentTypeId { get; set; }

	public string EmployeeDocumentTypeName { get; set; } = string.Empty;

	public DateOnly? IssueDate { get; set; }

	public DateOnly? ExpirationDate { get; set; }

	public string? Notes { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long Size { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }
	
	public string Status { get; set; } = string.Empty;
}