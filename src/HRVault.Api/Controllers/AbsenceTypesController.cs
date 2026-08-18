using HRVault.Api.Authorization;
using HRVault.Application.Absences.Commands.CreateAbsenceType;
using HRVault.Application.Absences.Commands.DeleteAbsenceType;
using HRVault.Application.Absences.Commands.UpdateAbsenceType;
using HRVault.Application.Absences.DTOs;
using HRVault.Application.Absences.Queries.GetAbsenceTypes;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AbsenceTypesController
    : BaseApiController
{
    [HttpGet]
	[HasPermission("Absences.ManageTypes")]
    public async Task<ActionResult<List<AbsenceTypeDto>>> GetAll()
    {
        var result =
            await Mediator.Send(
                new GetAbsenceTypesQuery());

        return Ok(result);
    }

    [HttpPost]
	[HasPermission("Absences.ManageTypes")]
    public async Task<ActionResult<Guid>> Create(
        CreateAbsenceTypeCommand command)
    {
        var id =
            await Mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id:guid}")]
	[HasPermission("Absences.ManageTypes")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateAbsenceTypeCommand command)
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
	[HasPermission("Absences.ManageTypes")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await Mediator.Send(
            new DeleteAbsenceTypeCommand(id));

        return NoContent();
    }
}