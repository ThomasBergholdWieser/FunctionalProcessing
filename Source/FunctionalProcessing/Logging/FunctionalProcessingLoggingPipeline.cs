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

        if (response is not IExecutionResult {ExecutionFailed: true} executionResult ||
            executionResult.Error is null)
        {
            return response;
        }

        Action<ILogger, string, object[]> logFunc = executionResult.Error.LogLevel switch
        {
            LogLevel.Error => LoggerExtensions.LogError,
            LogLevel.Trace => LoggerExtensions.LogTrace,
            LogLevel.Debug => LoggerExtensions.LogDebug,
            LogLevel.Information => LoggerExtensions.LogInformation,
            LogLevel.Warning => LoggerExtensions.LogWarning,
            LogLevel.Critical => LoggerExtensions.LogCritical,
            _ => (_,_,_) => {  }
        };

        logFunc(this.logger, executionResult.Error.Message, Array.Empty<object>());

        return response;
    }
}
