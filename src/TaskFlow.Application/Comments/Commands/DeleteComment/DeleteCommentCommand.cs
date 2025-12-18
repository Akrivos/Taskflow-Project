using MediatR;

namespace TaskFlow.Application.Comments.Commands.DeleteComment;

public sealed record DeleteCommentCommand(Guid Id) : IRequest<Unit>;
