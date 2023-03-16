using System.Net;
using System.Text.RegularExpressions;
using FunctionalProcessing.Interfaces;

namespace FunctionalProcessing;

public static class Execution
{
    private static readonly ExecutionResult VoidSuccess = new();

    public static ExecutionResult<TResult> Success<TResult>(TResult value) => value;

    public static ExecutionResult Success() =>
        VoidSuccess;

    public static ExecutionResult<TResult> Failure<TResult>(IEnumerable<string> messages, int? errorCode = null) =>
        new(new ExecutionError(messages) { ErrorCode = errorCode });

    public static ExecutionResult Failure(IEnumerable<string> messages, int? errorCode = null) =>
        new(new ExecutionError(messages) { ErrorCode = errorCode });

    public static ExecutionResult<TResult> Failure<TResult>(Exception exception) =>
        Failure<TResult>(GetExceptionMessages(exception));

    public static ExecutionResult<TResult> Failure<TResult>(string message, Exception ex) =>
        Failure<TResult>(new[] { message }.Concat(GetExceptionMessages(ex)));

    public static ExecutionResult<TResult> Failure<TResult>(string message, int? errorCode = null) =>
        Failure<TResult>(new[] { message }, errorCode);

    public static ExecutionResult<TResult> Failure<TResult>(IExecutionResult result, int? errorCode = null) =>
        Failure<TResult>(result.CheckedError.Messages, errorCode ?? result.CheckedError.ErrorCode);

    public static ExecutionResult Failure(IExecutionResult result, int? errorCode = null) =>
        Failure(result.CheckedError.Messages, errorCode ?? result.CheckedError.ErrorCode);

    public static ExecutionResult Failure(string message, int? errorCode = null) =>
        Failure(new[] { message }, errorCode);

    public static ExecutionResult Failure(string message, Exception ex, int? errorCode = null) =>
        Failure(new[] { message }.Concat(GetExceptionMessages(ex)), errorCode);

    public static ExecutionResult Failure(Exception ex, int? errorCode = null) =>
        Failure(GetExceptionMessages(ex).ToArray(), errorCode);

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