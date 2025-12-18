namespace TaskFlow.Api.Controllers.Requests.Auth;

public sealed record RegisterRequest(string Username, string Email, string Password, string Role);