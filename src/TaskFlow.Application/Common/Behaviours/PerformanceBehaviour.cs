using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TaskFlow.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private ICurrentUser _user;
    private ILogger<TRequest> _logger;
    public PerformanceBehaviour(ICurrentUser user, ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _user = user;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest req, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 300)
        {
            if(!string.IsNullOrEmpty(_user.UserId))
            {
                _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) UserId: {UserId}, Username: {UserName}",
                    typeof(TRequest).Name, elapsedMilliseconds, _user.UserId, _user.UserName);
            }
            else
            {
                _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    typeof(TRequest).Name, elapsedMilliseconds, req);
            }
        }

        return response;
    }
}