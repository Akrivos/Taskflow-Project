using FluentValidation;

namespace TaskFlow.Application.Projects.Commands;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project Id is required!");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required!")
            .MaximumLength(200).WithMessage("The maximum length of chars are 200!");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required!")
            .MaximumLength(2000);
    }
}
