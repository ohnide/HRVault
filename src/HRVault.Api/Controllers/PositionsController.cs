using HRVault.Api.Authorization;
using HRVault.Application.Positions.Commands.CreatePosition;
using HRVault.Application.Positions.Commands.DeletePosition;
using HRVault.Application.Positions.Commands.UpdatePosition;
using HRVault.Application.Positions.DTOs;
using HRVault.Application.Positions.Queries.GetPositionById;
using HRVault.Application.Positions.Queries.GetPositions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PositionsController : BaseApiController
{
    [HttpPost]
    [HasPermission("Positions.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreatePositionCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpGet]
    [HasPermission("Positions.View")]
    public async Task<ActionResult<List<PositionDto>>> GetAll()
    {
        var positions = await Mediator.Send(
            new GetPositionsQuery());

        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Positions.View")]
    public async Task<ActionResult<PositionDto>> GetById(Guid id)
    {
        var position = await Mediator.Send(
            new GetPositionByIdQuery(id));

        if (position is null)
            return NotFound();

        return Ok(position);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Positions.Update")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        UpdatePositionCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                "The route id must match the command id.");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Positions.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeletePositionCommand(id));

        return NoContent();
    }
}