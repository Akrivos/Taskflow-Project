namespace TaskFlow.Api.Controllers.Requests.Projects;
public sealed record CreateProjectRequest(
    string Name,
    string? Description
);