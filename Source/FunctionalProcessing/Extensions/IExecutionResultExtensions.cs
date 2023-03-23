using FunctionalProcessing.Interfaces;

namespace FunctionalProcessing.Extensions;

public static class IExecutionResultExtensions
{
    public static ExecutionResult Reduce(this IExecutionResult? source) =>
        new() { Error = source?.Error };

    public static Task<T> AsTask<T>(this T source) where T : IExecutionResult =>
        Task.FromResult(source);
}