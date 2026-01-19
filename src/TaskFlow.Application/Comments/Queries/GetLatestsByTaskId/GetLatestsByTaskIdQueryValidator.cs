using FluentValidation;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public sealed class GetLatestsByTaskIdQueryValidator
    : AbstractValidator<GetLatestsByTaskIdQuery>
{
    public GetLatestsByTaskIdQueryValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .When(x => x.Limit.HasValue)
            .WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(10)
            .When(x => x.Limit.HasValue)
            .WithMessage("Limit must be less than or equal to 10.");

        RuleFor(x => x.SortBy)
            .IsInEnum()
            .When(x => x.SortBy.HasValue)
            .WithMessage("Invalid SortBy value.");

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .When(x => x.SortDirection.HasValue)
            .WithMessage("Invalid SortDirection value.");
    }
}
