using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeAbsence : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid AbsenceTypeId { get; set; }

    public AbsenceType AbsenceType { get; set; } = null!;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public AbsenceStatus Status { get; set; }
        = AbsenceStatus.Pending;

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}