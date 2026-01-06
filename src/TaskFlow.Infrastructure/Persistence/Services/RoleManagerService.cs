using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Persistence.Services;

public class RoleManagerService : IRoleManagerService
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleManagerService(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName,CancellationToken ct = default)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }
}