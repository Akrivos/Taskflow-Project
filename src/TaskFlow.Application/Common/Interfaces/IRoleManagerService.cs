namespace TaskFlow.Application.Common.Interfaces;

public interface IRoleManagerService
{
    Task<bool> RoleExistsAsync(string roleName);
    //Task<bool> CreateRoleAsync(string roleName);
    //Task<bool> AssignRoleAsync(T user, string roleName);
}