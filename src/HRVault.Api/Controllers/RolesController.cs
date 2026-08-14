using HRVault.Api.Authorization;
using HRVault.Application.Roles.Commands.AssignPermissionToRole;
using HRVault.Application.Roles.Commands.CreateRole;
using HRVault.Application.Roles.Commands.DeleteRole;
using HRVault.Application.Roles.Commands.RemovePermissionFromRole;
using HRVault.Application.Roles.Commands.UpdateRole;
using HRVault.Application.Roles.DTOs;
using HRVault.Application.Roles.Queries.GetRoleById;
using HRVault.Application.Roles.Queries.GetRolePermissions;
using HRVault.Application.Roles.Queries.GetRoles;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : BaseApiController
{
    [HttpPost]
    [HasPermission("Roles.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreateRoleCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpGet]
    [HasPermission("Roles.View")]
    public async Task<ActionResult<List<RoleDto>>> GetAll()
    {
        var roles = await Mediator.Send(
            new GetRolesQuery());

        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Roles.View")]
    public async Task<ActionResult<RoleDto>> GetById(
        Guid id)
    {
        var role = await Mediator.Send(
            new GetRoleByIdQuery(id));

        if (role is null)
            return NotFound();

        return Ok(role);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Roles.Update")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        UpdateRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                "The route id must match the command id.");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Roles.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeleteRoleCommand(id));

        return NoContent();
    }

    // -------------------------------------------------
    // Role Permissions
    // -------------------------------------------------

    [HttpGet("{roleId:guid}/permissions")]
    [HasPermission("Roles.View")]
    public async Task<ActionResult<List<RolePermissionDto>>>
        GetPermissions(Guid roleId)
    {
        var permissions = await Mediator.Send(
            new GetRolePermissionsQuery(roleId));

        return Ok(permissions);
    }

    [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
    [HasPermission("Roles.Update")]
    public async Task<IActionResult> AssignPermission(
        Guid roleId,
        Guid permissionId)
    {
        await Mediator.Send(
            new AssignPermissionToRoleCommand(
                roleId,
                permissionId));

        return NoContent();
    }

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    [HasPermission("Roles.Update")]
    public async Task<IActionResult> RemovePermission(
        Guid roleId,
        Guid permissionId)
    {
        await Mediator.Send(
            new RemovePermissionFromRoleCommand(
                roleId,
                permissionId));

        return NoContent();
    }
}