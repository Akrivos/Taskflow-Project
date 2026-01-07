using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Projects.Commands;
using TaskFlow.Application.Projects.Queries;
using TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
using TaskFlow.Api.Controllers.Requests.Projects;

namespace TaskFlow.Api.Controllers;
[ApiController]
[Authorize(Policy = "Projects.Read")]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    } 

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken ct,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "name",
        [FromQuery] string? sortDirection = "asc"
      ) =>
        Ok(await _mediator.Send(new GetProjectsQuery(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Search: search,
            SortBy: sortBy,
            SortDirection: sortDirection
         ), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var project = await _mediator.Send(new GetProjectQuery(id), ct);
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req, CancellationToken ct)
    {
        var cmd = new CreateProjectCommand(
            Name: req.Name,
            Description: req.Description
        );

        var id = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Create), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProjectRequest req, CancellationToken ct)
    {
        var cmd = new UpdateProjectCommand(
            Id: id,
            Name: req.Name,
            Description: req.Description
        );

        await _mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] PatchProjectRequest req, CancellationToken ct)
    {
        var cmd = new PatchProjectCommand(
            Id: id,
            Name: req.Name,
            Description: req.Description
        );

        await _mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpGet("with-members")]
    public async Task<IActionResult> GetProjectsWithMembers(
        CancellationToken ct,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDirection = "asc"
     )
    {
        var projects = await _mediator.Send(new GetProjectsWithMembersQuery(
            PageNumber: pageNumber,
            PageSize: pageSize,
            Search: search,
            SortBy: sortBy,
            SortDirection: sortDirection
          ), ct);
        return Ok(projects);
    }
}
