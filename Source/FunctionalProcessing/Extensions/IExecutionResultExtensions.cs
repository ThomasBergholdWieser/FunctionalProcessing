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

        Action<ILogger, string> logFunc = result.Error.LogLevel switch
        {
            LogLevel.Error => LogExtensions.Error,
            LogLevel.Trace => LogExtensions.Trace,
            LogLevel.Debug => LogExtensions.Debug,
            LogLevel.Information => LogExtensions.Information,
            LogLevel.Warning => LogExtensions.Warning,
            LogLevel.Critical => LogExtensions.Critical,
            LogLevel.None => (_,_) => {  },
            _ => throw new ArgumentOutOfRangeException()
        };

        logFunc(logger, result.Error.Message);

        result.CheckedError.Logged = true;

        return result;
    }
}

static partial class LogExtensions
{
    [LoggerMessage(LogLevel.Information, "ExecutionResult: {Message}")]
    public static partial void Information(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Error, "ExecutionResult: {Message}")]
    public static partial void Error(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Debug, "ExecutionResult: {Message}")]
    public static partial void Debug(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Warning, "ExecutionResult: {Message}")]
    public static partial void Warning(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Critical, "ExecutionResult: {Message}")]
    public static partial void Critical(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Trace, "ExecutionResult: {Message}")]
    public static partial void Trace(this ILogger logger, string message);

}