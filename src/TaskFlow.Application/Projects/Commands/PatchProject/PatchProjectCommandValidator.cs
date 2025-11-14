using FluentValidation;

namespace TaskFlow.Application.Projects.Commands;

public sealed class PatchProjectCommandValidator : AbstractValidator<PatchProjectCommand>
{
    public PatchProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project Id is required!");
        RuleFor(x => x.Name)
            .MinimumLength(1).WithMessage("The minimum length of chars is 1!")
            .MaximumLength(200).WithMessage("The maximum length of chars are 200!")
            .When(x => x.Name != null);
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("The maximum length of chars are 2000!")
            .When(x => x.Description != null);
    }
}
