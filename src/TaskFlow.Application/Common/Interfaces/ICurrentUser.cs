public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string>? Roles { get; }
    bool IsInRole(string role);
}