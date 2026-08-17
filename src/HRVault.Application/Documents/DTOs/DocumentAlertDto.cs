namespace HRVault.Application.Documents.DTOs;

public class DocumentAlertDto
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string DocumentTypeName { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }

    public int? DaysRemaining { get; set; }

    public DateOnly AlertDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool EmailSent { get; set; }

    public DateTime? EmailSentAt { get; set; }
}