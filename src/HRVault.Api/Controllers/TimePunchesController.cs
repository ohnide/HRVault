using HRVault.Api.Authorization;
using HRVault.Application.TimePunches.Commands.CreateTimePunch;
using HRVault.Application.TimePunches.DTOs;
using HRVault.Application.TimePunches.Queries.GetEmployeeTimePunches;
using HRVault.Application.TimePunches.Queries.GetTodayTimePunches;
using HRVault.Application.TimePunches.Commands.CreateManualTimePunch;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimePunchesController : BaseApiController
{
    [HttpPost]
    [HasPermission("Employees.Update")]
    public async Task<ActionResult<Guid>> Create(
        CreateTimePunchCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(id);
    }
	
	[HttpPost("manual")]
	[HasPermission("Employees.Update")]
	public async Task<ActionResult<Guid>> CreateManual(
		CreateManualTimePunchCommand command)
	{
		var id = await Mediator.Send(command);
		return Ok(id);
	}

    [HttpGet("today")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<TimePunchDto>>> GetToday(
        [FromQuery] Guid? employeeId = null)
    {
        var result =
            await Mediator.Send(
                new GetTodayTimePunchesQuery(
                    employeeId));

        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<TimePunchDto>>> GetEmployee(
        Guid employeeId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        if (fromUtc.Kind != DateTimeKind.Utc ||
            toUtc.Kind != DateTimeKind.Utc)
        {
            return BadRequest(
                "fromUtc e toUtc devem ser enviados em UTC.");
        }

        if (toUtc <= fromUtc)
        {
            return BadRequest(
                "toUtc deve ser posterior a fromUtc.");
        }

        var result =
            await Mediator.Send(
                new GetEmployeeTimePunchesQuery(
                    employeeId,
                    fromUtc,
                    toUtc));

        return Ok(result);
    }
}
