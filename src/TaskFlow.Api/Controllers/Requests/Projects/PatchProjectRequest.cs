namespace TaskFlow.Api.Controllers.Requests.Projects;

public sealed record PatchProjectRequest(
    string? Name,
    string? Description
);