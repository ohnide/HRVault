using HRVault.Application.Common.Models;
using HRVault.Application.Documents.DTOs;
using HRVault.Application.Documents.Queries.SearchExpiringDocuments;
using HRVault.Application.Documents.Queries.GetDocumentSummary;
using HRVault.Application.Documents.Commands.GenerateDocumentAlerts;
using HRVault.Application.Documents.Queries.GetDocumentAlerts;
using HRVault.Application.Documents.Commands.MarkDocumentAlertAsRead;
using HRVault.Application.Documents.Commands.DismissDocumentAlert;
using HRVault.Application.Documents.Services;
using HRVault.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

using HRVault.Application.Common.Interfaces;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : BaseApiController
{
	
	private readonly IEmailService _emailService;
	private readonly IDocumentAlertEmailService _documentAlertEmailService;

public DocumentsController(
    IEmailService emailService,
    IDocumentAlertEmailService documentAlertEmailService)
{
    _emailService = emailService;
    _documentAlertEmailService = documentAlertEmailService;
}
	
    [HttpGet("search")]
	[HasPermission("Documents.View")]
    public async Task<ActionResult<PagedResult<ExpiringDocumentDto>>> Search(
        [FromQuery] ExpiringDocumentFilterDto filter)
    {
        var result = await Mediator.Send(
            new SearchExpiringDocumentsQuery(filter));

        return Ok(result);
    }
	
	[HttpGet("summary")]
	[HasPermission("Documents.View")]
	public async Task<ActionResult<DocumentSummaryDto>> GetSummary()
	{
		var result = await Mediator.Send(
			new GetDocumentSummaryQuery());

		return Ok(result);
	}
	
	[HttpPost("alerts/generate")]
	[HasPermission("Documents.View")]
	public async Task<ActionResult<int>> GenerateAlerts()
	{
		var created = await Mediator.Send(
			new GenerateDocumentAlertsCommand());

		return Ok(created);
	}
	
	[HttpGet("alerts")]
	[HasPermission("Documents.View")]
	public async Task<ActionResult<List<DocumentAlertDto>>> GetAlerts()
	{
		var result = await Mediator.Send(
			new GetDocumentAlertsQuery());

		return Ok(result);
	}
	
	[HttpPost("alerts/{id:guid}/read")]
	[HasPermission("Documents.View")]
	public async Task<IActionResult> MarkAlertAsRead(
		Guid id)
	{
		await Mediator.Send(
			new MarkDocumentAlertAsReadCommand(id));

		return NoContent();
	}

	[HttpPost("alerts/{id:guid}/dismiss")]
	[HasPermission("Documents.View")]
	public async Task<IActionResult> DismissAlert(
		Guid id)
	{
		await Mediator.Send(
			new DismissDocumentAlertCommand(id));

		return NoContent();
	}
}