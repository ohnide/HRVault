namespace HRVault.Domain.Entities;

public class VacationBalance
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal EntitledDays { get; set; }

    public decimal CarriedOverDays { get; set; }

    public decimal AdjustmentDays { get; set; }

    public string? Notes { get; set; }

    public Company Company { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}