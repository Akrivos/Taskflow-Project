using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Services;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Api.Controllers;

public record RegisterRequest(string Username, string Email, string Password, string Role);
public record LoginRequest(string Username, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly IJwtTokenService _tokens;

    public AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, RoleManager<IdentityRole> roles, IJwtTokenService tokens)
    {
        _users = users; _signIn = signIn; _roles = roles; _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!await _roles.RoleExistsAsync(req.Role))
            return BadRequest($"Role '{req.Role}' does not exist.");

        var user = new ApplicationUser { UserName = req.Username, Email = req.Email };
        var result = await _users.CreateAsync(user, req.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await _users.AddToRoleAsync(user, req.Role);
        return Ok(new { message = "User created", user = user.UserName, role = req.Role });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _users.FindByNameAsync(req.Username);
        if (user is null) return Unauthorized();

        var pass = await _signIn.CheckPasswordSignInAsync(user, req.Password, false);
        if (!pass.Succeeded) return Unauthorized();

        var roles = await _users.GetRolesAsync(user);
        var token = _tokens.GenerateToken(user.Id, user.UserName!, roles);
        return Ok(new { access_token = token, token_type = "Bearer", roles });
    }
}
