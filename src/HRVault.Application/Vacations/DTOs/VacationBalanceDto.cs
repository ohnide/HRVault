namespace HRVault.Application.Vacations.DTOs;

public class VacationBalanceDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal EntitledDays { get; set; }

    public decimal CarriedOverDays { get; set; }

    public decimal AdjustmentDays { get; set; }

    public string? Notes { get; set; }
	
	public decimal TotalDays { get; set; }

	public decimal ApprovedDays { get; set; }

	public decimal RemainingDays { get; set; }
}