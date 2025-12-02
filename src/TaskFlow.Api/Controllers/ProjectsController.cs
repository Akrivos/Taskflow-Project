using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Projects.Commands;
using TaskFlow.Application.Projects.Queries;

namespace TaskFlow.Api.Controllers;
[ApiController]
[Authorize(Policy = "Projects.Read")]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProjectsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(
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
         )));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _mediator.Send(new GetProjectQuery(id));
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Create), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProjectCommand cmd)
    {
        var command = cmd with { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] PatchProjectCommand cmd)
    {
        var command = cmd with { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
