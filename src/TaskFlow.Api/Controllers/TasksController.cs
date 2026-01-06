using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands;
using TaskFlow.Api.Controllers.Requests.Tasks;

namespace TaskFlow.Api.Controllers;
[ApiController]
[Authorize(Policy = "Tasks.Read")]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    public TasksController(IMediator mediator) 
    { 
        _mediator = mediator;  
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest req, CancellationToken ct)
    {
        var cmd = new CreateTaskCommand(req.Title, req.Description, req.ProjectId);
        var id = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Create), new { id }, null);
    }
}
