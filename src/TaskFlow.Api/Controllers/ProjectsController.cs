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
    public async Task<IActionResult> Get() =>
        Ok(await _mediator.Send(new GetProjectsQuery()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var list = await _mediator.Send(new GetProjectsQuery());
        var match = list.FirstOrDefault(p => p.Id == id);
        return match is null ? NotFound() : Ok(match);
    }
}
