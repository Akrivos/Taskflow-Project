using FluentValidation;

namespace TaskFlow.Application.Projects.Commands;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required!")
            .MaximumLength(200).WithMessage("The maximum length of chars are 200!");
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
