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
	
	public string Color { get; set; } = "#3B82F6";

    public ICollection<EmployeeAbsence> EmployeeAbsences { get; set; }
        = new List<EmployeeAbsence>();
}