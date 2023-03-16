using System;
using FluentAssertions;
using Xunit;

namespace FunctionalProcessing.Test;

public class ExecutionTests
{
    [Fact]
    public void It_Should_Combine_Execution_Correctly()
    {
        Execution.Combine(Execution.Success(true), Execution.Success(true))
            .Should()
            .BeEquivalentTo(Execution.Success(), x => x
                .Excluding(y => y.CheckedError));

        Execution.Combine(Execution.Success(true), Execution.Failure<bool>("Error"))
            .Should()
            .BeEquivalentTo(Execution.Failure("Error"), x => x
                .Excluding(y => y.CheckedError));

        Execution.Combine(Execution.Failure<bool>("Error"), Execution.Failure<bool>("Error"))
            .Should()
            .BeEquivalentTo(new ExecutionResult(new ExecutionError("Error", "Error")), x => x
                .Excluding(y => y.CheckedError));

        Execution.Combine(Execution.Failure<bool>("Error"), Execution.Success(true))
            .Should()
            .BeEquivalentTo(Execution.Failure("Error"), x => x
                .Excluding(y => y.CheckedError));

        Execution.Combine(Execution.Failure<bool>("Error", 50), Execution.Failure<bool>("Error"))
            .Should()
            .BeEquivalentTo(new ExecutionResult(new ExecutionError("Error", "Error") { ErrorCode = 50 }), x => x
                .Excluding(y => y.CheckedError));
    }

    [Fact]
    public void It_Should_Produce_Successful_Void()
    {
        var result = Execution.Success();
        result.Error.Should().BeNull();
        result.Invoking(x => x.CheckedError).Should().Throw<NullReferenceException>();
        result.ExecutionSucceeded.Should().BeTrue();
        result.ExecutionFailed.Should().BeFalse();
    }

    [Fact]
    public void It_Should_Produce_Successful_ValueType()
    {
        var result = Execution.Success(true);
        result.Error.Should().BeNull();
        result.Invoking(x => x.CheckedError).Should().Throw<NullReferenceException>();
        result.ExecutionSucceeded.Should().BeTrue();
        result.ExecutionFailed.Should().BeFalse();
        result.CheckedValue.Should().BeTrue();
    }

    [Fact]
    public void It_Should_Produce_Successful_ReferenceType()
    {
        var result = Execution.Success(new object());
        result.Error.Should().BeNull();
        result.Invoking(x => x.CheckedError).Should().Throw<NullReferenceException>();
        result.ExecutionSucceeded.Should().BeTrue();
        result.ExecutionFailed.Should().BeFalse();
        result.CheckedValue.Should().NotBeNull();
    }

    [Fact]
    public void It_Should_Produce_Failure_Void()
    {
        var result = Execution.Failure("Error");
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
    }

    [Fact]
    public void It_Should_Produce_Failure_Void_ErrorCode()
    {
        var result = Execution.Failure("Error", 12);
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().Be(12);
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
    }

    [Fact]
    public void It_Should_Produce_Failure_Void_Exception()
    {
        var result = Execution.Failure("Error", new Exception("ExceptionMessage"));
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Messages.Should().BeEquivalentTo("Error", "ExceptionMessage");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
    }

    [Fact]
    public void It_Should_Produce_Failure_Void_Exception_ErrorCode()
    {
        var result = Execution.Failure("Error", new Exception("ExceptionMessage"), 15);
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().Be(15);
        result.CheckedError.Messages.Should().BeEquivalentTo("Error", "ExceptionMessage");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
    }

    [Fact]
    public void It_Should_Produce_Failure_ValueType()
    {
        var result = Execution.Failure<bool>("Error");
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void It_Should_Produce_Failure_ValueType_ErrorCode()
    {
        var result = Execution.Failure<bool>("Error", 12);
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().Be(12);
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void It_Should_Produce_Failure_ValueType_Exception()
    {
        var result = Execution.Failure<bool>("Error", new Exception("ExceptionMessage"));
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Messages.Should().BeEquivalentTo("Error", "ExceptionMessage");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void It_Should_Produce_Failure_ReferenceType()
    {
        var result = Execution.Failure<object>("Error");
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void It_Should_Produce_Failure_ReferenceType_ErrorCode()
    {
        var result = Execution.Failure<object>("Error", 12);
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().Be(12);
        result.CheckedError.Message.Should().Be("Error");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void It_Should_Produce_Failure_ReferenceType_Exception()
    {
        var result = Execution.Failure<object>("Error", new Exception("ExceptionMessage"));
        result.Error.Should().NotBeNull();
        result.CheckedError.ErrorCode.Should().BeNull();
        result.CheckedError.Messages.Should().BeEquivalentTo("Error", "ExceptionMessage");
        result.ExecutionSucceeded.Should().BeFalse();
        result.ExecutionFailed.Should().BeTrue();
        result.Invoking(x => x.CheckedValue).Should().Throw<NullReferenceException>();
    }
}
