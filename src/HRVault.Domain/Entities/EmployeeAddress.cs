using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeAddress : SoftDeleteEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string Type { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? District { get; set; }

    public string Country { get; set; } = "Portugal";
}