using MediatR;

namespace TaskFlow.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest<Unit>;
