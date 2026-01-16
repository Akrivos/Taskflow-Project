using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TaskFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUser _user;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUser user)
    {
        _logger = logger;
        _user = user;
    }

    public Task Process(TRequest request, CancellationToken ct)
    {
        _logger.LogInformation(
            "Handling {RequestName} | UserId={UserId} | UserName={UserName}",
            typeof(TRequest).Name,
            _user.UserId,
            _user.UserName);

        return Task.CompletedTask;
    }
}

