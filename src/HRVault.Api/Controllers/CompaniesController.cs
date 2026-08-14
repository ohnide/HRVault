using HRVault.Application.Common.Models;
using HRVault.Application.Companies.Commands.CreateCompany;
using HRVault.Application.Companies.Commands.DeleteCompany;
using HRVault.Application.Companies.Commands.UpdateCompany;
using HRVault.Application.Companies.DTOs;
using HRVault.Application.Companies.Queries.GetCompanies;
using HRVault.Application.Companies.Queries.GetCompanyById;
using HRVault.Application.Companies.Queries.SearchCompanies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CompaniesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAll()
    {
        var result = await Mediator.Send(
            new GetCompaniesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(
            new GetCompanyByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<CompanyDto>>> Search(
        [FromQuery] SearchCompaniesQuery query)
    {
        var result = await Mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        CreateCompanyCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCompanyCommand command)
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
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeleteCompanyCommand(id));

        return NoContent();
    }
}