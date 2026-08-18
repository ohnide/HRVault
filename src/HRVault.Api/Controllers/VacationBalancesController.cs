using HRVault.Application.Vacations.Commands.SetVacationBalance;
using HRVault.Application.Vacations.DTOs;
using HRVault.Application.Vacations.Queries.GetVacationBalance;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacationBalancesController
    : BaseApiController
{
    [HttpGet("{employeeId:guid}/{year:int}")]
    public async Task<ActionResult<VacationBalanceDto>> Get(
        Guid employeeId,
        int year)
    {
        var result =
            await Mediator.Send(
                new GetVacationBalanceQuery(
                    employeeId,
                    year));

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{employeeId:guid}/{year:int}")]
    public async Task<ActionResult<Guid>> Set(
        Guid employeeId,
        int year,
        SetVacationBalanceCommand command)
    {
        if (employeeId != command.EmployeeId ||
            year != command.Year)
        {
            return BadRequest(
                "Route values must match the command.");
        }

        var id =
            await Mediator.Send(command);

        return Ok(id);
    }
}