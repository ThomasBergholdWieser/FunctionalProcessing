using System.Runtime.Serialization;

namespace FunctionalProcessing;

[Serializable]
public class ExecutionException : Exception
{
    public ExecutionException(string message)
        : base(message)
    {
    }

    protected ExecutionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}