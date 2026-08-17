using Microsoft.AspNetCore.Http;

namespace HRVault.Api.Models.Employees;

public class UploadEmployeeDocumentRequest
{
	public Guid EmployeeDocumentTypeId { get; set; }

	public DateOnly? IssueDate { get; set; }

	public DateOnly? ExpirationDate { get; set; }

	public string? Notes { get; set; }

	public IFormFile File { get; set; } = null!;
}