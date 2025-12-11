namespace TaskFlow.Api.Controllers.Requests;

public record RegisterRequest(string Username, string Email, string Password, string Role);