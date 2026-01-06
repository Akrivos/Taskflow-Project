using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence.Services;

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponseDto> CreateAsync(UserDto userDto)
    {
        var user = new ApplicationUser
        {
            UserName = userDto.UserName,
            Email = userDto.Email
        };
        var result = await _userManager.CreateAsync(user, userDto.Password!);
        if (!result.Succeeded)
        {
            return null;
        }
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<UserResponseDto> AddToRoleAsync(string userId, string roleName)
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
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = roleName
        };
    }

    public async Task<UserResponseDto> FindByNameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return null;
        }
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
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
        return await _userManager.GetRolesAsync(user);
    }
}
