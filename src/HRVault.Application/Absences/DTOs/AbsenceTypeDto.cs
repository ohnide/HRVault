namespace HRVault.Application.Absences.DTOs;

public class AbsenceTypeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public string? Description { get; set; }

    public bool RequiresApproval { get; set; }

    public bool RequiresDocument { get; set; }

    public bool IsPaid { get; set; }
	
	public string Color { get; set; } = "#3B82F6";
}