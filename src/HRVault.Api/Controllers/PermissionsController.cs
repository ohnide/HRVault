using HRVault.Api.Authorization;
using HRVault.Application.Permissions.DTOs;
using HRVault.Application.Permissions.Queries.GetPermissionById;
using HRVault.Application.Permissions.Queries.GetPermissions;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : BaseApiController
{
    [HttpGet]
    [HasPermission("Roles.View")]
    public async Task<ActionResult<List<PermissionDto>>> GetAll()
    {
        var result = await Mediator.Send(
            new GetPermissionsQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Roles.View")]
    public async Task<ActionResult<PermissionDto>> GetById(
        Guid id)
    {
        var result = await Mediator.Send(
            new GetPermissionByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}