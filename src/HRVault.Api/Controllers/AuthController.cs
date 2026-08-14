using HRVault.Application.Authentication.Commands.Login;
using HRVault.Application.Authentication.Commands.RefreshToken;
using HRVault.Application.Authentication.DTOs;
using HRVault.Application.Authentication.Commands.Logout;
using HRVault.Application.Authentication.Commands.LogoutAll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }
	
	[HttpPost("refresh")]
	public async Task<ActionResult<LoginResponse>> Refresh(
		RefreshTokenCommand command)
	{
		var result = await Mediator.Send(command);

		return Ok(result);
	}
	
	[HttpPost("logout")]
	public async Task<IActionResult> Logout(
		LogoutCommand command)
	{
		await Mediator.Send(command);

		return NoContent();
	}
	
	[Authorize]
	[HttpPost("logout-all")]
	public async Task<IActionResult> LogoutAll()
	{
		await Mediator.Send(new LogoutAllCommand());

		return NoContent();
	}
}