using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

public class LogginBehavior<TRequest, TResponse>(ILogger<LogginBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[Start] Handle request={Request} - Reponse={respone} - RequestData={request}",
            typeof(TRequest).Name,
            typeof(TResponse).Name, request);

        var timer = Stopwatch.StartNew();

        var response = await next();

        timer.Stop();

        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
            logger.LogWarning("[Performance] The request {Request} took {TimeTaken} seconds", typeof(TRequest).Name,
                timeTaken.Seconds);

        logger.LogInformation("[End] Handled request={Request} - Reponse={respone}",
            typeof(TRequest).Name,
            typeof(TResponse).Name);
        return response;
    }
}