
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IUserManagerService
{
    //Task<UserResponseDto> FindByEmailAsync(string email);
    Task<UserResponseDto> CreateAsync(UserDto? user);
    Task<UserResponseDto> AddToRoleAsync(string userId, string roleName);
    Task<UserResponseDto> FindByNameAsync(string userName);
    Task<IList<string>> GetRolesAsync(string userId);
}
