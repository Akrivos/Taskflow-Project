namespace TaskFlow.Api.Controllers.Requests.Auth;

public sealed record LoginRequest (string UserName, string Password);