using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ICommentRepository _repo;
    private readonly ICurrentUser _currentUser;
    public DeleteCommentCommandHandler(ICommentRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        if (_currentUser.UserId == null || hasAllowedRole)
        {
            throw new ForbiddenAccessException("You dont have access!");
        }
        var comment = await _repo.GetByIdAsync(request.Id, ct);
        if (comment == null)
        {
            throw new NotFoundException("Comment", request.Id);
        }
        await _repo.DeleteAsync(comment, ct);
        await _repo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
