using Microsoft.AspNetCore.Http;

namespace HRVault.Api.Models.Employees;

public class UploadEmployeeDocumentRequest
{
    public string Category { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}