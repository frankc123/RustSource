using System;
using System.Runtime.Serialization;

[Serializable]
public class NonPlayerRootControllableException : InstantiateControllableException
{
    public NonPlayerRootControllableException()
    {
    }

    public NonPlayerRootControllableException(string message) : base(message)
    {
    }

    protected NonPlayerRootControllableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NonPlayerRootControllableException(string message, Exception inner) : base(message, inner)
    {
    }
}

