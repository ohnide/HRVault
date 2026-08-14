using HRVault.Api.Authorization;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.Commands.CreateEmployee;
using HRVault.Application.Employees.Commands.DeleteEmployee;
using HRVault.Application.Employees.Commands.UpdateEmployee;
using HRVault.Application.Employees.DTOs;
using HRVault.Application.Employees.Queries.GetEmployeeById;
using HRVault.Application.Employees.Queries.GetEmployees;
using HRVault.Application.Employees.Queries.SearchEmployees;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : BaseApiController
{
    [HttpGet]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll()
    {
        var result = await Mediator.Send(
            new GetEmployeesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(
            new GetEmployeeByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> Search(
        [FromQuery] EmployeeFilterDto filter)
    {
        var result = await Mediator.Send(
            new SearchEmployeesQuery(filter));

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Employees.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreateEmployeeCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Employees.Update")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        UpdateEmployeeCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                "The route id must match the command id.");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Employees.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeleteEmployeeCommand(id));

        return NoContent();
    }
}