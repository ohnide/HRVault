using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeDocumentType : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool HasExpiration { get; set; }

    public int? ExpirationWarningDays { get; set; }

    public ICollection<Document> Documents { get; set; }
        = new List<Document>();
}