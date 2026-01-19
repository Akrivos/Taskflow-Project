using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Entities;
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
    private readonly DateTime now = DateTime.UtcNow;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenWriteRepository refreshTokenWriteRepository,
        IRefreshTokenReadRepository refreshTokenReadRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _refreshTokenReadRepository = refreshTokenReadRepository;
    }

    public async Task<AuthTokenResult?> LoginAsync(
        string username,
        string password,
        CancellationToken ct = default)
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

        var authResponse = await IssueTokensAsync(user, ct);
        await _refreshTokenWriteRepository.SaveChangesAsync(ct);

        return authResponse;
    }

    public async Task<AuthTokenResult?> RefreshTokenAsync(
        string refreshTokenValue,
        CancellationToken ct = default)
    {
        var storedToken = await _refreshTokenReadRepository
            .GetByTokenAsync(refreshTokenValue, ct);

        if (storedToken is null)
        {
            return null;
        }

        bool isActive =
            storedToken.RevokedAt is null &&
            storedToken.ExpiresAt > DateTime.UtcNow;

        if (!isActive)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId!);
        if (user is null)
        {
            return null;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        var authResponse = await IssueTokensAsync(user, ct);

        storedToken.ReplacedByToken = authResponse.RefreshToken;

        await _refreshTokenWriteRepository.SaveChangesAsync(ct);

        return authResponse;
    }

    private async Task<AuthTokenResult> IssueTokensAsync(
        ApplicationUser user,
        CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtTokenService.GenerateToken(
            user.Id,
            user.UserName!,
            roles);

        var refreshTokenLifetime = TimeSpan.FromDays(RefreshTokenDaysValid);


        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = RefreshTokenGenerator.Generate(),
            CreatedAt = now,
            ExpiresAt = now.AddDays(RefreshTokenDaysValid)
        };

        await _refreshTokenWriteRepository.AddAsync(refreshToken, ct);

        return new AuthTokenResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            Roles: roles.ToList());
    }
}
