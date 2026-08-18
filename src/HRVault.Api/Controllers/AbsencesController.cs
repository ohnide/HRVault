using HRVault.Api.Authorization;
using HRVault.Application.Absences.Commands.CreateEmployeeAbsence;
using HRVault.Application.Absences.Commands.DeleteEmployeeAbsence;
using HRVault.Application.Absences.Commands.UpdateEmployeeAbsence;
using HRVault.Application.Absences.DTOs;
using HRVault.Application.Absences.Queries.GetEmployeeAbsenceById;
using HRVault.Application.Absences.Queries.SearchEmployeeAbsences;
using HRVault.Application.Absences.Commands.ApproveEmployeeAbsence;
using HRVault.Application.Absences.Commands.RejectEmployeeAbsence;
using HRVault.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AbsencesController
    : BaseApiController
{
    [HttpGet("search")]
    [HasPermission("Absences.View")]
    public async Task<ActionResult<PagedResult<EmployeeAbsenceDto>>> Search(
        [FromQuery] EmployeeAbsenceFilterDto filter)
    {
        var result =
            await Mediator.Send(
                new SearchEmployeeAbsencesQuery(
                    filter));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Absences.View")]
    public async Task<ActionResult<EmployeeAbsenceDto>> GetById(
        Guid id)
    {
        var result =
            await Mediator.Send(
                new GetEmployeeAbsenceByIdQuery(
                    id));

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Absences.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreateEmployeeAbsenceCommand command)
    {
        var id =
            await Mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Absences.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateEmployeeAbsenceCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "The route id must match the command id.");
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Absences.Delete")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await Mediator.Send(
            new DeleteEmployeeAbsenceCommand(
                id));

        return NoContent();
    }
	
	[HttpPut("{id:guid}/approve")]
	[HasPermission("Absences.Approve")]
	public async Task<IActionResult> Approve(
		Guid id)
	{
		await Mediator.Send(
			new ApproveEmployeeAbsenceCommand(
				id));

		return NoContent();
	}

	[HttpPut("{id:guid}/reject")]
	[HasPermission("Absences.Approve")]
	public async Task<IActionResult> Reject(
		Guid id)
	{
		await Mediator.Send(
			new RejectEmployeeAbsenceCommand(
				id));

		return NoContent();
	}
}