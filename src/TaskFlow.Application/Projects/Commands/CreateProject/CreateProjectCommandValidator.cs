using FluentValidation;

namespace TaskFlow.Application.Projects.Commands;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Το όνομα είναι υποχρεωτικό.")
            .MaximumLength(200).WithMessage("Μέγιστο 200 χαρακτήρες.");
        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
