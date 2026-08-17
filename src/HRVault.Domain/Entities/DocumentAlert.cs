using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class DocumentAlert : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }

    public Guid DocumentId { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly AlertDate { get; set; }

    public DocumentAlertStatus Status { get; set; }
        = DocumentAlertStatus.Pending;

    public DateTime? ReadAt { get; set; }

    public DateTime? DismissedAt { get; set; }

    public bool EmailSent { get; set; }

    public DateTime? EmailSentAt { get; set; }

    public Document Document { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}