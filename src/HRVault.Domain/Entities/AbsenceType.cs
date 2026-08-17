using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class AbsenceType : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool RequiresApproval { get; set; } = true;

    public bool RequiresDocument { get; set; }

    public bool IsPaid { get; set; }

    public ICollection<EmployeeAbsence> EmployeeAbsences { get; set; }
        = new List<EmployeeAbsence>();
}