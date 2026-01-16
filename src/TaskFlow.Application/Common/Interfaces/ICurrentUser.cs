public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    string? UserName { get; }
    IReadOnlyList<string>? Roles { get; }
    bool IsInRole(string role);
}