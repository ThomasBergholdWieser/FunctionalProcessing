﻿using Microsoft.Extensions.Logging;

namespace FunctionalProcessing;

public record ExecutionError
{
    public ExecutionError(IEnumerable<string> messages)
    {
        this.Messages = messages.ToArray();
    }

    public ExecutionError(params string[] messages)
        : this(messages.AsEnumerable())
    {
    }

    public string Message => string.Join("; ", this.Messages);

    public IList<string> Messages { get; set; } = new List<string>();

    public int? ErrorCode { get; set; }

    public LogLevel LogLevel { get; set; } = LogLevel.Error;

    public bool Logged { get; internal set; } = false;
}