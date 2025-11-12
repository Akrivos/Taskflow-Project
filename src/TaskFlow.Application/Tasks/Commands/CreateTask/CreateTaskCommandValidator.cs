using FluentValidation;

namespace TaskFlow.Application.Tasks.Commands;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Ο τίτλος είναι υποχρεωτικός.")
            .MaximumLength(200);
        RuleFor(x => x.Description)
            .MaximumLength(4000);
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Απαιτείται ProjectId.");
    }
}
