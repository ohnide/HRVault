namespace HRVault.Application.Vacations.DTOs;

public class VacationRequestFilterDto
{
    public Guid? EmployeeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? Status { get; set; }

    public int? Year { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}