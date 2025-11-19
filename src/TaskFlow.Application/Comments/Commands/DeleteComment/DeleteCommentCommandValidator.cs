using FluentValidation;
namespace TaskFlow.Application.Comments.Commands.DeleteComment;

public sealed class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Comment ID cannot be empty.");
    }
}