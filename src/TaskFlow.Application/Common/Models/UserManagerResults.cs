namespace TaskFlow.Application.Common.Models;

public record CreatedUserResult(
    string Id,
    string UserName,
    string Email
);

public record UserSummaryResult(
    string Id,
    string UserName,
    string Email
);

public record AddToRoleResult(
    string Id,
    string UserName,
    string Email,
    string Role
);
