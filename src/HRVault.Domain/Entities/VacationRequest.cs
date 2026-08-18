using HRVault.Domain.Enums;

namespace HRVault.Domain.Entities;

public class VacationRequest
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Days { get; set; }

    public VacationRequestStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedBy { get; set; }

    public Company Company { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
	
}