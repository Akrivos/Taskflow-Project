using FluentValidation;

namespace TaskFlow.Application.Auth.Commands.RefreshTokenCommand;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token must not be empty.")
            .MaximumLength(256).WithMessage("Refresh token is too long.");
    }
}