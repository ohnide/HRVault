namespace HRVault.Application.Employees.DTOs;

public class EmployeeDocumentDownloadDto
{
    public Stream Content { get; set; } = Stream.Null;

    public string FileName { get; set; } = string.Empty;

    public string MimeType { get; set; } =
        "application/octet-stream";
}