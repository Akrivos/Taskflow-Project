using FluentValidation;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public sealed class GetLatestsByTaskIdQueryValidator : AbstractValidator<GetLatestsByTaskIdQuery>
{
    private static readonly HashSet<string> AllowedSortByValues = new() { "createdAt" };
    private static readonly HashSet<string> AllowedSortDirectionValues = new() { "asc", "desc" };
    public GetLatestsByTaskIdQueryValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("TaskId is required.");
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Limit must be less than or equal to 10.");
        RuleFor(x => x.SortBy)
            .Must(value => AllowedSortByValues.Contains(value))
            .WithMessage($"SortBy must be one of the following values: {string.Join(", ", AllowedSortByValues)}.");
        RuleFor(x => x.SortDirection)
            .Must(value => AllowedSortDirectionValues.Contains(value))
            .WithMessage($"SortDirection must be one of the following values: {string.Join(", ", AllowedSortDirectionValues)}.");
    }
}