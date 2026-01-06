using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler: IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICurrentUser _user;
    private readonly ICommentWriteRepository _commentWriteRepo;
    private readonly ITaskReadRepository _taskReadRepo;
    public CreateCommentCommandHandler(
        ICurrentUser user, 
        ICommentWriteRepository commentWriteRepo, 
        ITaskReadRepository taskReadRepo
      )
    {
        _user = user;
        _commentWriteRepo = commentWriteRepo;
        _taskReadRepo = taskReadRepo;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        var taskId = request.TaskItemId;
        var content = request.Content;
        var userId = _user.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("User must be authenticated to create a comment.");
        }

        var taskItem = await _taskReadRepo.GetByIdAsync(taskId, ct);
        if (taskItem is null)
        {
            throw new NotFoundException("Task", taskId);
        }

        var comment = new Comment(taskId, content, userId);
        comment.Validate();
        await _commentWriteRepo.AddAsync(comment, ct);
        await _commentWriteRepo.SaveChangesAsync(ct);
        return comment.Id;
    }
}
