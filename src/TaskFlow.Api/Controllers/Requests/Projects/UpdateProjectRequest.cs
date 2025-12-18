namespace TaskFlow.Api.Controllers.Requests.Projects;

public sealed record UpdateProjectRequest(
    string Name,
    string? Description
);