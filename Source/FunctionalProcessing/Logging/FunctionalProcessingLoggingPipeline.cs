using FunctionalProcessing.Extensions;
using FunctionalProcessing.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FunctionalProcessing.Logging;

public sealed class FunctionalProcessingLoggingPipeline<TRequest, TResponse>(ILogger<TRequest> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (response is not IExecutionResult {ExecutionFailed: true} executionResult ||
            executionResult.Error is null ||
            executionResult.Error.Logged)
        {
            return response;
        }

        executionResult.Log(logger);

        return response;
    }
}
