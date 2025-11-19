using FluentValidation;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Comments.Commands.CreateComment;

public sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TaskItemId)
            .NotEmpty().WithMessage("TaskId is required.")
            .Must(id => id != Guid.Empty).WithMessage("TaskItemId must be a valid UUID.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(Comment.MaxContentLength).WithMessage("Content cannot exceed 2000 characters.");
    }
}
