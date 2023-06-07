using FunctionalProcessing.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FunctionalProcessing.Logging;

public sealed class FunctionalProcessingLoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> logger;

    public FunctionalProcessingLoggingPipeline(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (response is IExecutionResult { ExecutionFailed: true, Error.Handled: not true } executionResult)
        {
            this.logger.LogError(executionResult.CheckedError.Message);
        }

        return response;
    }
}
