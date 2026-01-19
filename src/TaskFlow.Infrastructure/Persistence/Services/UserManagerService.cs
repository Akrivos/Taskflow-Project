using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence.Services;

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreatedUserResult?> CreateAsync(string userName, string email, string password, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password!);
        if (!result.Succeeded)
        {
            return null;
        }

        return new CreatedUserResult(
            Id: user.Id,
            UserName: user.UserName!,
            Email: user.Email!
        );
    }

    public async Task<AddToRoleResult?> AddToRoleAsync(string userId, string roleName, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
        {
            return null;
        }
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return null;
        }

        return new AddToRoleResult(
            Id: user.Id,
            UserName: user.UserName!,
            Email: user.Email!,
            Role: roleName
        );
    }

    public async Task<UserSummaryResult> FindByNameAsync(string username, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return null;
        }

        return new UserSummaryResult(
            Id: user.Id,
            UserName: user.UserName!,
            Email: user.Email!
        );
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken ct = default)
    {
        if(userId is null)
        {
            return new List<string>();
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new List<string>();
        }
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
