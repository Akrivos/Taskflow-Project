
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Comments.Commands.CreateComment;
using TaskFlow.Application.Comments.Commands.DeleteComment;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize(Policy = "Comments.Read")]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator) { _mediator = mediator;}

    //[HttpGet("{id:guid}")]
    //public async Task<IActionResult> GetById([FromRoute] Guid id)
    //{
    //    var comment = await _mediator.Send(new GetCommentQuery(id));
    //    return Ok(comment);
    //}

    [HttpPost]
    [Authorize(Policy = "Comments.Create")]
    public async Task<IActionResult> Create([FromBody] CreateCommentCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(Create), new { id });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Comments.Delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteCommentCommand(id));
        return NoContent();
    }
}
