using MediatR;
using System.ComponentModel.DataAnnotations;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

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
    public async Task<string> Handle(RegisterCommand request, CancellationToken ct = default)
    {

        if (!await _roleService.RoleExistsAsync(request.Role))
        {
            throw new ValidationException($"Role '{request.Role}' does not exist.");
        }

        var userBody = new UserDto
        {
            UserName = request.Username,
            Email = request.Email,
            Password = request.Password
        };
        var user = await _userService.CreateAsync(userBody);
        if(user is null)
        {
            throw new ValidationException("User registration failed.");
        }

        var userWithRole = await _userService.AddToRoleAsync(user.Id!, request.Role);
        if(userWithRole is null) {
            throw  new ValidationException("Assigning role to user failed.");
        }

        return user.Id;
    }
}
