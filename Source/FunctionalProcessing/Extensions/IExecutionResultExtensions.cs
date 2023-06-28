using FunctionalProcessing.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunctionalProcessing.Extensions;

public static class IExecutionResultExtensions
{
    public static ExecutionResult Reduce(this IExecutionResult? source) =>
        new() { Error = source?.Error };

    public static Task<T> AsTask<T>(this T source) where T : IExecutionResult =>
        Task.FromResult(source);

    public static T Log<T>(this T result, ILogger logger)
        where T : IExecutionResult
    {
        if (result.ExecutionSucceeded)
        {
            return result;
        }

        if (result.Error is null ||
            result.Error.Logged)
        {
            return result;
        }

        Action<ILogger, string, object[]> logFunc = result.Error.LogLevel switch
        {
            LogLevel.Error => LoggerExtensions.LogError,
            LogLevel.Trace => LoggerExtensions.LogTrace,
            LogLevel.Debug => LoggerExtensions.LogDebug,
            LogLevel.Information => LoggerExtensions.LogInformation,
            LogLevel.Warning => LoggerExtensions.LogWarning,
            LogLevel.Critical => LoggerExtensions.LogCritical,
            _ => (_, _, _) => { }
        };

        logFunc(logger, result.Error.Message, Array.Empty<object>());

        result.CheckedError.Logged = true;

        return result;
    }
}