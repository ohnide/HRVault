using HRVault.Application.Authentication.Commands.Login;
using HRVault.Application.Authentication.DTOs;
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
}