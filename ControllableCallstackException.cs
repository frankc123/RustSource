using System;
using System.Runtime.Serialization;

[Serializable]
public class ControllableCallstackException : InvalidOperationException
{
    public ControllableCallstackException()
    {
    }

    public ControllableCallstackException(string message) : base(message)
    {
    }

    protected ControllableCallstackException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ControllableCallstackException(string message, Exception inner) : base(message, inner)
    {
    }
}

