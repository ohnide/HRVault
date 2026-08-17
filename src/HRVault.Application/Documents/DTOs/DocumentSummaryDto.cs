namespace HRVault.Application.Documents.DTOs;

public class DocumentSummaryDto
{
    public int Total { get; set; }

    public int Valid { get; set; }

    public int Expiring { get; set; }

    public int Expired { get; set; }
}