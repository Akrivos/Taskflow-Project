using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler: IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICurrentUser _user;
    private readonly ICommentRepository _commentRepo;
    private readonly ITaskRepository _taskRepo;
    public CreateCommentCommandHandler(ICurrentUser user, ICommentRepository commentRepo, ITaskRepository taskRepo)
    {
        _user = user;
        _commentRepo = commentRepo;
        _taskRepo = taskRepo;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        var taskId = request.TaskItemId;
        var content = request.Content;
        var userId = _user.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("User must be authenticated to create a comment.");
        }

        var taskItem = await _taskRepo.GetByIdAsync(taskId, ct);
        if (taskItem == null)
        {
            throw new NotFoundException("Task", taskId);
        }

        var comment = new Comment(taskId, content, userId);
        comment.Validate();
        await _commentRepo.AddAsync(comment, ct);
        await _commentRepo.SaveChangesAsync(ct);
        return comment.Id;
    }
}
