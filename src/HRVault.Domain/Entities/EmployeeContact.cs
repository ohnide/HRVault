using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeContact : SoftDeleteEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public ContactType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public string? Notes { get; set; }
}