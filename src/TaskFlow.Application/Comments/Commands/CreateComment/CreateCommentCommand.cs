using MediatR;
namespace TaskFlow.Application.Comments.Commands.CreateComment;

public sealed record CreateCommentCommand(Guid TaskItemId, string Content) : IRequest<Guid>;
    

