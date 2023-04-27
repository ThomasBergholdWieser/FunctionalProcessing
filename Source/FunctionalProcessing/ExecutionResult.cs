using FunctionalProcessing.Interfaces;

namespace FunctionalProcessing;

public record ExecutionResult<T>(ExecutionError? Error = null) : IExecutionResult
    where T : notnull

{
    protected T? Value { get; set; }

    public bool ExecutionSucceeded => this.Error is null && this.Value is not null;

    public bool ExecutionFailed => this.Error is not null || this.Value is null;

    public T CheckedValue => this.ExecutionSucceeded ? this.Value! : throw new NullReferenceException();

    public ExecutionError CheckedError => this.Error ?? throw new NullReferenceException();

    public static implicit operator ExecutionResult<T>(T value) => new() { Value = value };

    public T ThrowIfFailed(string? exceptionMessage = null)
    {
        ((IExecutionResult)this).ThrowIfFailed(exceptionMessage);
        return this.CheckedValue;
    }

    public override string ToString() =>
        this.ExecutionSucceeded ? nameof(this.ExecutionSucceeded) : nameof(this.ExecutionFailed) + ": " + this.Error;
}

public record ExecutionResult(ExecutionError? Error = null) : IExecutionResult
{
    public bool ExecutionSucceeded => this.Error is null;

    public bool ExecutionFailed => this.Error is not null;

    public ExecutionError CheckedError => this.Error ?? throw new NullReferenceException();

    public override string ToString() =>
        this.ExecutionSucceeded ? nameof(this.ExecutionSucceeded) : nameof(this.ExecutionFailed) + ": " + this.Error;
}