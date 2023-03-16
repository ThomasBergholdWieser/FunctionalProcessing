namespace FunctionalProcessing.Interfaces;

public interface IExecutionResult
{
    ExecutionError? Error { get; }

    bool ExecutionSucceeded { get; }

    bool ExecutionFailed { get; }

    ExecutionError CheckedError => this.Error ?? throw new NullReferenceException();

    void ThrowIfFailed(string? exceptionMessage = null)
    {
        if (!this.ExecutionFailed)
        {
            return;
        }

        string BuildInternalMessage() =>
            this.Error is null
                ? "Unknown Error"
                : this.Error.Message;

        throw new ExecutionException(exceptionMessage is null
            ? BuildInternalMessage()
            : exceptionMessage + ": " + BuildInternalMessage());
    }
}