namespace TaskFlow.Api.Controllers.Requests.Tasks;

public sealed record CreateTaskRequest(string Title, string? Description, Guid ProjectId);