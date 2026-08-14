using HRVault.Api.Authorization;
using HRVault.Application.Common.Models;
using HRVault.Application.Departments.Commands.CreateDepartment;
using HRVault.Application.Departments.Commands.DeleteDepartment;
using HRVault.Application.Departments.Commands.UpdateDepartment;
using HRVault.Application.Departments.DTOs;
using HRVault.Application.Departments.Queries.GetDepartmentById;
using HRVault.Application.Departments.Queries.GetDepartments;
using HRVault.Application.Departments.Queries.SearchDepartments;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : BaseApiController
{
    [HttpGet]
    [HasPermission("Departments.View")]
    public async Task<ActionResult<List<DepartmentDto>>> GetAll()
    {
        var result = await Mediator.Send(
            new GetDepartmentsQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Departments.View")]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(
            new GetDepartmentByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    [HasPermission("Departments.View")]
    public async Task<ActionResult<PagedResult<DepartmentDto>>> Search(
        [FromQuery] SearchDepartmentsQuery query)
    {
        var result = await Mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Departments.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreateDepartmentCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Departments.Update")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        UpdateDepartmentCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                "The route id must match the command id.");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Departments.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeleteDepartmentCommand(id));

        return NoContent();
    }
}