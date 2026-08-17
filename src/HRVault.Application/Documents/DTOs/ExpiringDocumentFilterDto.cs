namespace HRVault.Application.Documents.DTOs;

public class ExpiringDocumentFilterDto
{
    public string? Status { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? EmployeeDocumentTypeId { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}