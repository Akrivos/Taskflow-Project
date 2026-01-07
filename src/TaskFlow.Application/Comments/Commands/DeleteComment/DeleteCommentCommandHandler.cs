using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ICommentReadRepository _commentReadRepo;
    private readonly ICommentWriteRepository _commentWriteRepo;
    private readonly ICurrentUser _currentUser;
    public DeleteCommentCommandHandler(
        ICommentReadRepository commentReadRepo,
        ICommentWriteRepository commentWriteRepo,
        ICurrentUser currentUser)
    {
        _commentReadRepo = commentReadRepo;
        _commentWriteRepo = commentWriteRepo;
        _currentUser = currentUser;
    }
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        if (_currentUser.UserId is null || hasAllowedRole)
        {
            throw new ForbiddenException("You dont have access!");
        }
        var comment = await _commentReadRepo.GetByIdAsync(request.Id, ct);
        if (comment == null)
        {
            throw new NotFoundException("Comment", request.Id);
        }
        await _commentWriteRepo.DeleteAsync(comment, ct);
        await _commentWriteRepo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
