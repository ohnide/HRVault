using HRVault.Api.Authorization;
using HRVault.Application.WorkSchedules.Commands.CreateWorkSchedule;
using HRVault.Application.WorkSchedules.Commands.DeleteWorkSchedule;
using HRVault.Application.WorkSchedules.Commands.SetWorkScheduleActive;
using HRVault.Application.WorkSchedules.Commands.UpdateWorkSchedule;
using HRVault.Application.WorkSchedules.DTOs;
using HRVault.Application.WorkSchedules.Queries.GetWorkScheduleById;
using HRVault.Application.WorkSchedules.Queries.GetWorkSchedules;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkSchedulesController : BaseApiController
{
    [HttpGet]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<WorkScheduleDto>>> GetAll()
        => Ok(await Mediator.Send(new GetWorkSchedulesQuery()));

    [HttpGet("{id:guid}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<WorkScheduleDto>> GetById(Guid id)
    {
        var result =
            await Mediator.Send(
                new GetWorkScheduleByIdQuery(id));

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpPost]
    [HasPermission("Employees.Update")]
    public async Task<ActionResult<Guid>> Create(
        CreateWorkScheduleCommand command)
    {
        var id =
            await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Employees.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateWorkScheduleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "The route id must match the command id.");
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpPut("{id:guid}/active")]
    [HasPermission("Employees.Update")]
    public async Task<IActionResult> SetActive(
        Guid id,
        SetWorkScheduleActiveRequest request)
    {
        await Mediator.Send(
            new SetWorkScheduleActiveCommand(
                id,
                request.IsActive));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Employees.Update")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await Mediator.Send(
            new DeleteWorkScheduleCommand(id));

        return NoContent();
    }
}

public record SetWorkScheduleActiveRequest(
    bool IsActive);
