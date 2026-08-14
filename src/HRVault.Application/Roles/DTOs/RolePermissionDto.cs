namespace HRVault.Application.Roles.DTOs;

public class RolePermissionDto
{
    public Guid PermissionId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}