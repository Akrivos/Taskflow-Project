using MediatR;
namespace TaskFlow.Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(Guid TaskItemId, string Content) : IRequest<Guid>;
    

