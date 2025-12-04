using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands;
using TaskFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Api.Controllers;
[ApiController]
[Authorize(Policy = "Tasks.Read")]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TaskFlowDbContext _db;
    public TasksController(IMediator mediator, TaskFlowDbContext db) { _mediator = mediator; _db = db; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Create), new { id }, null);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _db.Tasks.Include(t=>t.Attachments).FirstOrDefaultAsync(t => t.Id == id);
        return task is null ? NotFound() : Ok(new { task.Id, task.Title, task.Description, task.ProjectId, task.Status });
    }
}
