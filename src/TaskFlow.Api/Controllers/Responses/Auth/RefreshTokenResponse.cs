namespace TaskFlow.Api.Controllers.Responses.Auth;

public sealed record RefreshTokenResponse(
    string AccessToken, 
    string RefreshToken,
    string TokenType,
    IReadOnlyList<string> Roles
);