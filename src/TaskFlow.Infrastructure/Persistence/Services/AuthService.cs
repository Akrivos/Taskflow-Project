using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Common;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence.Services;

public class AuthService : IAuthService
{
    private const int RefreshTokenDaysValid = 7;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository;
    private readonly IRefreshTokenReadRepository _refreshTokenReadRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenWriteRepository refreshTokenWriteRepository,
        IRefreshTokenReadRepository refreshTokenReadRepository
     )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _refreshTokenReadRepository = refreshTokenReadRepository;
    }

    public async Task<AuthResponseDto?> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return null;
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user.Id, user.UserName!, roles);
        var refreshTokenLifetime = TimeSpan.FromDays(RefreshTokenDaysValid);

        var refreshTokenBody = new RefreshTokenBodyDto
        {
            UserId = user.Id,
            Token = RefreshTokenGenerator.Generate(),
            ExpiresAt = DateTime.UtcNow.Add(refreshTokenLifetime),
            CreatedAt = DateTime.UtcNow
        };
        var refreshToken = await _refreshTokenWriteRepository.CreateAsync(refreshTokenBody);

        await _refreshTokenWriteRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token!,
            Roles = roles
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshTokenValue, CancellationToken ct = default)
    {
        var storedToken = await _refreshTokenReadRepository.GetByTokenAsync(refreshTokenValue, ct);
        if (storedToken is null)
        {
            return null;
        }

        bool isActive = storedToken.RevokedAt is null && storedToken.ExpiresAt > DateTime.UtcNow;
        if (!isActive)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId!);
        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = _jwtTokenService.GenerateToken(user.Id, user.UserName!, roles);

        var lifetime = TimeSpan.FromDays(RefreshTokenDaysValid);
        var newRefreshTokenBody = new RefreshTokenBodyDto
        {
            UserId = user.Id,
            Token = RefreshTokenGenerator.Generate(),
            ExpiresAt = DateTime.UtcNow.Add(lifetime),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenWriteRepository.CreateAsync(newRefreshTokenBody, ct);

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshTokenBody.Token;

        await _refreshTokenWriteRepository.SaveChangesAsync(ct);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenBody.Token,
            Roles = roles
        };
    }

}
