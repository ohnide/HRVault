using HRVault.Api.Authorization;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.Commands.CreateEmployee;
using HRVault.Application.Employees.Commands.DeleteEmployee;
using HRVault.Application.Employees.Commands.UpdateEmployee;
using HRVault.Application.Employees.DTOs;
using HRVault.Application.Employees.Queries.GetEmployeeById;
using HRVault.Application.Employees.Queries.GetEmployees;
using HRVault.Application.Employees.Queries.SearchEmployees;
using HRVault.Application.Employees.Queries.GetEmployeeDetails;
using HRVault.Application.Employees.Commands.UpsertEmployeeProfile;
using HRVault.Application.Employees.Commands.CreateEmployeeAddress;
using HRVault.Application.Employees.Commands.UpdateEmployeeAddress;
using HRVault.Application.Employees.Commands.DeleteEmployeeAddress;
using HRVault.Application.Employees.Commands.CreateEmployeeContact;
using HRVault.Application.Employees.Commands.UpdateEmployeeContact;
using HRVault.Application.Employees.Commands.DeleteEmployeeContact;
using HRVault.Application.Employees.Commands.UpsertEmployeeEmergencyContact;
using HRVault.Application.Employees.Commands.DeleteEmployeeEmergencyContact;
using HRVault.Application.Employees.Commands.UploadEmployeeDocument;
using HRVault.Application.Employees.Queries.GetEmployeeDocuments;
using HRVault.Application.Employees.Queries.DownloadEmployeeDocument;
using HRVault.Application.Employees.Commands.DeleteEmployeeDocument;
using HRVault.Application.Employees.Commands.CreateEmployeeDocumentType;
using HRVault.Application.Employees.Queries.GetEmployeeDocumentTypes;
using HRVault.Application.Employees.Commands.UpdateEmployeeDocumentType;
using HRVault.Application.Employees.Commands.DeleteEmployeeDocumentType;
using HRVault.Application.Employees.Queries.SearchEmployeeDocuments;
using HRVault.Api.Models.Employees;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : BaseApiController
{
    [HttpGet]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll()
    {
        var result = await Mediator.Send(
            new GetEmployeesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(
            new GetEmployeeByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("search")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> Search(
        [FromQuery] EmployeeFilterDto filter)
    {
        var result = await Mediator.Send(
            new SearchEmployeesQuery(filter));

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Employees.Create")]
    public async Task<ActionResult<Guid>> Create(
        CreateEmployeeCommand command)
    {
        var id = await Mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Employees.Update")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        UpdateEmployeeCommand command)
    {
        if (id != command.Id)
            return BadRequest(
                "The route id must match the command id.");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Employees.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(
            new DeleteEmployeeCommand(id));

        return NoContent();
    }
	
	[HttpGet("{id:guid}/details")]
	public async Task<ActionResult<EmployeeDetailsDto>> GetDetails(
		Guid id)
	{
		var result = await Mediator.Send(
			new GetEmployeeDetailsQuery(id));

		if (result is null)
			return NotFound();

		return Ok(result);
	}
	
	[HttpPut("{id:guid}/profile")]
	public async Task<IActionResult> UpsertProfile(
		Guid id,
		UpsertEmployeeProfileCommand command)
	{
		if (id != command.EmployeeId)
		{
			return BadRequest(
				"The route id must match the employee id.");
		}

		await Mediator.Send(command);

		return NoContent();
	}
	
	[HttpPost("{employeeId:guid}/addresses")]
	public async Task<ActionResult<Guid>> CreateAddress(
		Guid employeeId,
		CreateEmployeeAddressCommand command)
	{
		if (employeeId != command.EmployeeId)
		{
			return BadRequest(
				"The route employee id must match the command employee id.");
		}

		var id = await Mediator.Send(command);

		return Ok(id);
	}
	
	[HttpPut("{employeeId:guid}/addresses/{addressId:guid}")]
	public async Task<IActionResult> UpdateAddress(
		Guid employeeId,
		Guid addressId,
		UpdateEmployeeAddressCommand command)
	{
		if (employeeId != command.EmployeeId)
		{
			return BadRequest(
				"The route employee id must match the command employee id.");
		}

		if (addressId != command.AddressId)
		{
			return BadRequest(
				"The route address id must match the command address id.");
		}

		await Mediator.Send(command);

		return NoContent();
	}
	
	[HttpDelete("{employeeId:guid}/addresses/{addressId:guid}")]
	public async Task<IActionResult> DeleteAddress(
		Guid employeeId,
		Guid addressId)
	{
		await Mediator.Send(
			new DeleteEmployeeAddressCommand(
				employeeId,
				addressId));

		return NoContent();
	}
	
	[HttpPost("{employeeId:guid}/contacts")]
	public async Task<ActionResult<Guid>> CreateContact(
		Guid employeeId,
		CreateEmployeeContactCommand command)
	{
		if (employeeId != command.EmployeeId)
		{
			return BadRequest(
				"The route employee id must match the command employee id.");
		}

		var id = await Mediator.Send(command);

		return Ok(id);
	}
	
	[HttpPut("{employeeId:guid}/contacts/{contactId:guid}")]
	public async Task<IActionResult> UpdateContact(
		Guid employeeId,
		Guid contactId,
		UpdateEmployeeContactCommand command)
	{
		if (employeeId != command.EmployeeId)
		{
			return BadRequest(
				"The route employee id must match the command employee id.");
		}

		if (contactId != command.ContactId)
		{
			return BadRequest(
				"The route contact id must match the command contact id.");
		}

		await Mediator.Send(command);

		return NoContent();
	}
	
	[HttpDelete("{employeeId:guid}/contacts/{contactId:guid}")]
	public async Task<IActionResult> DeleteContact(
		Guid employeeId,
		Guid contactId)
	{
		await Mediator.Send(
			new DeleteEmployeeContactCommand(
				employeeId,
				contactId));

		return NoContent();
	}
	
	[HttpPut("{employeeId:guid}/emergency-contact")]
	public async Task<IActionResult> UpsertEmergencyContact(
		Guid employeeId,
		UpsertEmployeeEmergencyContactCommand command)
	{
		if (employeeId != command.EmployeeId)
		{
			return BadRequest(
				"The route employee id must match the command employee id.");
		}

		await Mediator.Send(command);

		return NoContent();
	}
	
	[HttpDelete("{employeeId:guid}/emergency-contact")]
	public async Task<IActionResult> DeleteEmergencyContact(
		Guid employeeId)
	{
		await Mediator.Send(
			new DeleteEmployeeEmergencyContactCommand(
				employeeId));

		return NoContent();
	}
	
	[HttpPost("{employeeId:guid}/documents")]
	[Consumes("multipart/form-data")]
	[RequestSizeLimit(20_000_000)]
	public async Task<ActionResult<Guid>> UploadDocument(
		Guid employeeId,
		[FromForm] UploadEmployeeDocumentRequest request)
	{
		if (request.File is null)
		{
			return BadRequest(
				"No file was received.");
		}

		if (request.File.Length == 0)
		{
			return BadRequest(
				"The uploaded file is empty.");
		}

		if (request.EmployeeDocumentTypeId == Guid.Empty)
		{
			return BadRequest(
				"EmployeeDocumentTypeId is required.");
		}

		await using var stream =
			request.File.OpenReadStream();

		var command =
			new UploadEmployeeDocumentCommand(
				EmployeeId: employeeId,
				EmployeeDocumentTypeId:
					request.EmployeeDocumentTypeId,
				IssueDate: request.IssueDate,
				ExpirationDate: request.ExpirationDate,
				Notes: request.Notes,
				FileName: request.File.FileName,
				ContentType: request.File.ContentType,
				Size: request.File.Length,
				Content: stream);

		var id = await Mediator.Send(command);

		return Ok(id);
	}
	
	[HttpGet("{employeeId:guid}/documents")]
	public async Task<ActionResult<List<EmployeeDocumentDto>>> GetDocuments(
		Guid employeeId)
	{
		var result = await Mediator.Send(
			new GetEmployeeDocumentsQuery(
				employeeId));

		return Ok(result);
	}
	
	[HttpGet("{employeeId:guid}/documents/{documentId:guid}/download")]
	public async Task<IActionResult> DownloadDocument(
		Guid employeeId,
		Guid documentId)
	{
		var result = await Mediator.Send(
			new DownloadEmployeeDocumentQuery(
				employeeId,
				documentId));

		return File(
			result.Content,
			result.MimeType,
			result.FileName);
	}
	
	[HttpDelete("{employeeId:guid}/documents/{documentId:guid}")]
	public async Task<IActionResult> DeleteDocument(
		Guid employeeId,
		Guid documentId)
	{
		await Mediator.Send(
			new DeleteEmployeeDocumentCommand(
				employeeId,
				documentId));

		return NoContent();
	}
	
	[HttpGet("document-types")]
	public async Task<ActionResult<List<EmployeeDocumentTypeDto>>> GetDocumentTypes()
	{
		var result = await Mediator.Send(
			new GetEmployeeDocumentTypesQuery());

		return Ok(result);
	}

	[HttpPost("document-types")]
	public async Task<ActionResult<Guid>> CreateDocumentType(
		CreateEmployeeDocumentTypeCommand command)
	{
		var id = await Mediator.Send(command);

		return Ok(id);
	}
	
	[HttpPut("document-types/{id:guid}")]
	public async Task<IActionResult> UpdateDocumentType(
		Guid id,
		UpdateEmployeeDocumentTypeCommand command)
	{
		if (id != command.Id)
		{
			return BadRequest(
				"The route id must match the command id.");
		}

		await Mediator.Send(command);

		return NoContent();
	}

	[HttpDelete("document-types/{id:guid}")]
	public async Task<IActionResult> DeleteDocumentType(
		Guid id)
	{
		await Mediator.Send(
			new DeleteEmployeeDocumentTypeCommand(id));

		return NoContent();
	}
	
	[HttpGet("{employeeId:guid}/documents/search")]
	public async Task<ActionResult<PagedResult<EmployeeDocumentDto>>> SearchDocuments(
		Guid employeeId,
		[FromQuery] EmployeeDocumentFilterDto filter)
	{
		var result = await Mediator.Send(
			new SearchEmployeeDocumentsQuery(
				employeeId,
				filter));

		return Ok(result);
	}
}