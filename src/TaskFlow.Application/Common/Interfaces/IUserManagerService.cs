
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IUserManagerService
{
    Task<CreatedUserResult?> CreateAsync(string userName, string email, string password, CancellationToken ct = default);
    Task<AddToRoleResult?> AddToRoleAsync(string userId, string roleName, CancellationToken ct = default);
    Task<UserSummaryResult?> FindByNameAsync(string userName, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken ct = default);
}
