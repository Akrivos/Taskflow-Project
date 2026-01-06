
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IUserManagerService
{
    Task<UserResponseDto> CreateAsync(UserDto? user, CancellationToken ct = default);
    Task<UserResponseDto> AddToRoleAsync(string userId, string roleName, CancellationToken ct = default);
    Task<UserResponseDto> FindByNameAsync(string userName, CancellationToken ct = default);
    Task<IList<string>> GetRolesAsync(string userId, CancellationToken ct = default);
}
