using HRVault.Application.Common.Models;
using HRVault.Application.Documents.DTOs;
using HRVault.Application.Documents.Queries.SearchExpiringDocuments;
using HRVault.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : BaseApiController
{
    [HttpGet("search")]
	[HasPermission("Documents.View")]
    public async Task<ActionResult<PagedResult<ExpiringDocumentDto>>> Search(
        [FromQuery] ExpiringDocumentFilterDto filter)
    {
        var result = await Mediator.Send(
            new SearchExpiringDocumentsQuery(filter));

        return Ok(result);
    }
}