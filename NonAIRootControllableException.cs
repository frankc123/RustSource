using System;
using System.Runtime.Serialization;

[Serializable]
public class NonAIRootControllableException : InstantiateControllableException
{
    public NonAIRootControllableException()
    {
    }

    public NonAIRootControllableException(string message) : base(message)
    {
    }

    protected NonAIRootControllableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NonAIRootControllableException(string message, Exception inner) : base(message, inner)
    {
    }
}

