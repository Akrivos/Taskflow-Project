using System.Security.Claims;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUser(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string? UserId =>
        _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email =>
        _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public IReadOnlyList<string>? Roles =>
        _contextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList();

    public bool IsInRole(string role)
    {
        return _contextAccessor.HttpContext?.User.IsInRole(role) ?? false;
    }
}
