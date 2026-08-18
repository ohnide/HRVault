using HRVault.Application.Vacations.Commands.ApproveVacationRequest;
using HRVault.Application.Vacations.Commands.CreateVacationRequest;
using HRVault.Application.Vacations.Commands.RejectVacationRequest;
using HRVault.Application.Vacations.DTOs;
using HRVault.Application.Vacations.Queries.SearchVacationRequests;
using HRVault.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacationRequestsController
    : BaseApiController
{
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<VacationRequestDto>>> Search(
        [FromQuery] VacationRequestFilterDto filter)
    {
        var result =
            await Mediator.Send(
                new SearchVacationRequestsQuery(
                    filter));

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        CreateVacationRequestCommand command)
    {
        var id =
            await Mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id)
    {
        await Mediator.Send(
            new ApproveVacationRequestCommand(
                id));

        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id)
    {
        await Mediator.Send(
            new RejectVacationRequestCommand(
                id));

        return NoContent();
    }
}