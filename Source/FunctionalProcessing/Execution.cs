using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using FunctionalProcessing.Interfaces;

namespace FunctionalProcessing;

public static class Execution
{
    private static readonly ExecutionResult VoidSuccess = new();

    public static ExecutionResult<TResult> Success<TResult>(TResult value) where TResult : notnull => value;
        

    public static ExecutionResult Success() =>
        VoidSuccess;

    public static ExecutionResult<TResult> Failure<TResult>(IEnumerable<string> messages, int? errorCode = null, bool? suppressPipelineLogging = null) where TResult : notnull =>
        new(new ExecutionError(messages) { ErrorCode = errorCode, Handled = suppressPipelineLogging });
    
    public static ExecutionResult Failure(IEnumerable<string> messages, int? errorCode = null, bool? suppressPipelineLogging = null) =>
        new(new ExecutionError(messages) { ErrorCode = errorCode, Handled = suppressPipelineLogging });

    public static ExecutionResult<TResult> Failure<TResult>(Exception exception, bool? suppressPipelineLogging = null) where TResult : notnull =>
        Failure<TResult>(GetExceptionMessages(exception), suppressPipelineLogging: suppressPipelineLogging);

    public static ExecutionResult<TResult> Failure<TResult>(string message, Exception ex, bool? suppressPipelineLogging = null) where TResult : notnull =>
        Failure<TResult>(new[] { message }.Concat(GetExceptionMessages(ex)), suppressPipelineLogging: suppressPipelineLogging);

    public static ExecutionResult<TResult> Failure<TResult>(string message, int? errorCode = null, bool? suppressPipelineLogging = null) where TResult : notnull =>
        Failure<TResult>(new[] { message }, errorCode, suppressPipelineLogging);

    public static ExecutionResult<TResult> Failure<TResult>(IExecutionResult result, int? errorCode = null, bool? suppressPipelineLogging = null) where TResult : notnull =>
        Failure<TResult>(result.CheckedError.Messages, errorCode ?? result.CheckedError.ErrorCode, suppressPipelineLogging);

    public static ExecutionResult Failure(IExecutionResult result, int? errorCode = null, bool? suppressPipelineLogging = null) =>
        Failure(result.CheckedError.Messages, errorCode ?? result.CheckedError.ErrorCode, suppressPipelineLogging);

    public static ExecutionResult Failure(string message, int? errorCode = null, bool? suppressPipelineLogging = null) =>
        Failure(new[] { message }, errorCode, suppressPipelineLogging);

    public static ExecutionResult Failure(string message, Exception ex, int? errorCode = null, bool? suppressPipelineLogging = null) =>
        Failure(new[] { message }.Concat(GetExceptionMessages(ex)), errorCode, suppressPipelineLogging);

    public static ExecutionResult Failure(Exception ex, int? errorCode = null, bool? suppressPipelineLogging = null) =>
        Failure(GetExceptionMessages(ex).ToArray(), errorCode, suppressPipelineLogging);

    public static ExecutionResult Combine<T>(params T[] results)
        where T : IExecutionResult =>
        results.All(x => x.ExecutionSucceeded)
            ? Success()
            : Failure(ConcatMessages(results), ConcatErrorCode(results));

    public static string ToStatusCodeText(HttpStatusCode statusCode) =>
        Regex.Replace(statusCode.ToString(), "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private static int? ConcatErrorCode<T>(params T[] results)
        where T : IExecutionResult =>
        results.Select(x => x.Error?.ErrorCode).FirstOrDefault(x => x is not null);

    private static List<string> ConcatMessages<T>(params T[] results)
        where T : IExecutionResult =>
        results.SelectMany(x => x.Error?.Messages ?? new List<string>()).ToList();

    private static IEnumerable<string> GetExceptionMessages(Exception ex)
    {
        if (ex is AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                foreach (var innerMessage in GetExceptionMessages(innerException))
                {
                    yield return innerMessage;
                }
            }
        }
        else
        {
            yield return ex.Message;
        }

        if (ex.InnerException is null)
        {
            yield break;
        }

        foreach (var innerMessage in GetExceptionMessages(ex.InnerException))
        {
            yield return innerMessage;
        }
    }
}