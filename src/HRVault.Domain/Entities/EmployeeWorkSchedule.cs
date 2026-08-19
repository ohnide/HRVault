using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class EmployeeWorkSchedule : AuditableEntity
{
    public Guid CompanyId { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public Guid WorkScheduleId { get; set; }
    public WorkSchedule WorkSchedule { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
