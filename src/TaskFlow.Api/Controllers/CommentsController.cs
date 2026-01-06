
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Controllers.Requests.Comment;
using TaskFlow.Application.Comments.Commands.CreateComment;
using TaskFlow.Application.Comments.Commands.DeleteComment;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize(Policy = "Comments.Read")]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator) { _mediator = mediator;}

    [HttpPost]
    [Authorize(Policy = "Comments.Create")]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest req, CancellationToken ct)
    {
        var cmd = new CreateCommentCommand(req.TaskId, req.Content);
        var id = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Create), new { id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Comments.Delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCommentCommand(id), ct);
        return NoContent();
    }

    [HttpGet("task/{taskId:guid}/latests")]
    public async Task<IActionResult> GetLatestsByTaskId(
        [FromRoute] Guid taskId,
        CancellationToken ct,
        [FromQuery] int? limit = 10, 
        [FromQuery] string? sortDirection = "desc",
        [FromQuery] string? sortBy = "createdAt"
       )
    {
        var comments = await _mediator.Send(new GetLatestsByTaskIdQuery(
            TaskId: taskId,
            Limit: limit,
            SortDirection: sortDirection,
            SortBy: sortBy
         ), ct);
        return Ok(comments);
    }
}
