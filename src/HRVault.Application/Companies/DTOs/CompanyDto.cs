namespace HRVault.Application.Companies.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string VatNumber { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }
	
	public string? HrNotificationEmail { get; set; }
}