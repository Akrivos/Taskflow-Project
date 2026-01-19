namespace TaskFlow.Application.Common.Models;

public record AuthTokenResult(
    string AccessToken,
    string RefreshToken,
    IReadOnlyList<string> Roles
);
