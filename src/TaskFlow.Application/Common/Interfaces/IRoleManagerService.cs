namespace TaskFlow.Application.Common.Interfaces;

public interface IRoleManagerService
{
    Task<bool> RoleExistsAsync(string roleName);
}