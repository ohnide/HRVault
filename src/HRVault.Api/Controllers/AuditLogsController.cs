using HRVault.Application.AuditLogs.DTOs;
using HRVault.Application.AuditLogs.Queries.SearchAuditLogs;
using HRVault.Application.AuditLogs.Queries.GetAuditLogById;
using HRVault.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuditLogsController : BaseApiController
{
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> Search(
        [FromQuery] AuditLogFilterDto filter)
    {
        var result = await Mediator.Send(
            new SearchAuditLogsQuery(filter));

        return Ok(result);
    }
	
	[HttpGet("{id:guid}")]
	public async Task<ActionResult<AuditLogDto>> GetById(
		Guid id)
	{
		var result = await Mediator.Send(
			new GetAuditLogByIdQuery(id));

		if (result is null)
			return NotFound();

		return Ok(result);
	}
}