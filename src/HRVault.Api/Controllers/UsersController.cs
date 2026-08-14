using HRVault.Application.Roles.DTOs;
using HRVault.Application.Users.Commands.AssignRoleToUser;
using HRVault.Application.Users.Queries.GetUserRoles;
using HRVault.Application.Users.Commands.RemoveRoleFromUser;
using HRVault.Application.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Mvc;
using HRVault.Application.Users.Queries.GetUserById;
using HRVault.Application.Users.DTOs;
using HRVault.Application.Users.Queries.GetUsers;
using HRVault.Application.Users.Commands.UpdateUser;
using HRVault.Application.Users.Commands.DeleteUser;
using HRVault.Api.Authorization;
using HRVault.Application.Users.Commands.ChangePassword;
using HRVault.Application.Users.Commands.ResetPassword;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
	[HasPermission("Users.Update")]
    public async Task<IActionResult> AssignRole(
        Guid userId,
        Guid roleId)
    {
        await Mediator.Send(
            new AssignRoleToUserCommand(
                userId,
                roleId));

        return Ok(new
        {
            Message = "Role atribuído ao utilizador com sucesso."
        });
    }

    [HttpGet("{userId:guid}/roles")]
	[HasPermission("Users.View")]
    public async Task<ActionResult<List<RoleDto>>> GetRoles(
        Guid userId)
    {
        var roles = await Mediator.Send(
            new GetUserRolesQuery(userId));

        return Ok(roles);
    }
	
	[HttpDelete("{userId:guid}/roles/{roleId:guid}")]
	[HasPermission("Users.Update")]
	public async Task<IActionResult> RemoveRole(
		Guid userId,
		Guid roleId)
	{
		await Mediator.Send(
			new RemoveRoleFromUserCommand(
				userId,
				roleId));

		return NoContent();
	}
	
	[HttpPost]
	[HasPermission("Users.Create")]
	public async Task<ActionResult<Guid>> Create(
		CreateUserCommand command)
	{
		var id = await Mediator.Send(command);

		return CreatedAtAction(
			nameof(GetById),
			new { id },
			id);
	}
	
	[HttpGet("{id:guid}")]
	[HasPermission("Users.View")]
	public async Task<ActionResult<UserDto>> GetById(Guid id)
	{
		var user = await Mediator.Send(
			new GetUserByIdQuery(id));

		if (user is null)
			return NotFound();

		return Ok(user);
	}
	
	[HttpGet]
	[HasPermission("Users.View")]
	public async Task<ActionResult<List<UserDto>>> GetAll()
	{
		var users = await Mediator.Send(
			new GetUsersQuery());

		return Ok(users);
	}
	
	[HttpPut("{id:guid}")]
	[HasPermission("Users.Update")]
	public async Task<ActionResult<Guid>> Update(
		Guid id,
		UpdateUserCommand command)
	{
		if (id != command.Id)
			return BadRequest(
				"The route id must match the command id.");

		var result = await Mediator.Send(command);

		return Ok(result);
	}
	
	[HttpDelete("{id:guid}")]
	[HasPermission("Users.Delete")]
	public async Task<IActionResult> Delete(Guid id)
	{
		await Mediator.Send(
			new DeleteUserCommand(id));

		return NoContent();
	}
	
	[HttpPut("{id:guid}/password")]
	public async Task<IActionResult> ChangePassword(
		Guid id,
		ChangePasswordCommand command)
	{
		if (id != command.UserId)
			return BadRequest(
				"The route id must match the command user id.");

		await Mediator.Send(command);

		return NoContent();
	}
	
	[HttpPut("{id:guid}/password/reset")]
	[HasPermission("Users.ResetPassword")]
	public async Task<IActionResult> ResetPassword(
		Guid id,
		ResetPasswordCommand command)
	{
		if (id != command.UserId)
			return BadRequest(
				"The route id must match the command user id.");

		await Mediator.Send(command);

		return NoContent();
	}
}