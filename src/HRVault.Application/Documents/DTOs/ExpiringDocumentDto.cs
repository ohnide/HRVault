namespace HRVault.Application.Documents.DTOs;

public class ExpiringDocumentDto
{
    public Guid DocumentId { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public Guid EmployeeDocumentTypeId { get; set; }

    public string DocumentTypeName { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }

    public int? DaysRemaining { get; set; }

    public string Status { get; set; } = string.Empty;
}