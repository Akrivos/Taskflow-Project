using FluentValidation;
using MediatR;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Auth.Commands.RegisterCommand;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IUserManagerService _userService;
    private readonly IRoleManagerService _roleService;
    public RegisterCommandHandler(
        IUserManagerService userService,
        IRoleManagerService roleService
        )
    {
        _userService = userService;
        _roleService = roleService;
    }
    public async Task<string> Handle(RegisterCommand request, CancellationToken ct)
    {
        var role = request.Role?.Trim();

        var allowed =
            string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "ProjectManager", StringComparison.OrdinalIgnoreCase);

        if (!allowed)
        {
            throw new ValidationException("Invalid role. Allowed roles: User, ProjectManager.");
        }

        if (!await _roleService.RoleExistsAsync(role!, ct))
        {
            throw new ValidationException($"Role '{role}' is not configured.");
        }

        var createdUser = await _userService.CreateAsync(
            userName: request.Username,
            email: request.Email,
            password: request.Password,
            ct: ct);

        if (createdUser is null)
        {
            throw new ValidationException("User registration failed.");
        }

        var userWithRole = await _userService.AddToRoleAsync(createdUser.Id!, request.Role!, ct);
        if(userWithRole is null) {
            throw  new ValidationException("Assigning role to user failed.");
        }

        return createdUser.Id;
    }
}
