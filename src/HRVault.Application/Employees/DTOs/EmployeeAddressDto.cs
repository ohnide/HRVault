namespace HRVault.Application.Employees.DTOs;

public class EmployeeAddressDto
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? District { get; set; }

    public string Country { get; set; } = string.Empty;
}