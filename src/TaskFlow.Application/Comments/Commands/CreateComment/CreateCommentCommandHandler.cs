using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler: IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICurrentUser _user;
    private readonly ICommentRepository _repo;
    public CreateCommentCommandHandler(ICurrentUser user, ICommentRepository repo)
    {
        _repo = repo;
        _user = user;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated to create a comment.");
        }
        var comment = new Comment(request.TaskItemId, request.Content, _user.UserId);
        comment.Validate();
        await _repo.AddAsync(comment, ct);
        await _repo.SaveChangesAsync(ct);
        return comment.Id;
    }
}
