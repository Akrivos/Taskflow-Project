using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Controllers.Requests.Auth;
using TaskFlow.Application.Auth.Commands.Login;
using TaskFlow.Application.Auth.Commands.RefreshTokenCommand;
using TaskFlow.Application.Auth.Commands.RegisterCommand;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var result = await _mediator.Send(new RegisterCommand(req.Username, req.Email, req.Password, req.Role));
        return CreatedAtAction(nameof(Register), new { Id = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _mediator.Send(new LoginCommand(req.UserName, req.Password));
        return Ok(new { 
            access_token = result.AccessToken, 
            refresh_token = result.RefreshToken, 
            token_type = "Bearer", 
            roles = result.Roles 
       });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(refreshToken.RefreshToken));
        return Ok(new
        {
            access_token = result.AccessToken,
            refresh_token = result.RefreshToken,
            token_type = "Bearer",
            roles = result.Roles
        });
    }
}
