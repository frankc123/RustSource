using System;
using System.Runtime.Serialization;

[Serializable]
public class NonBindlessVesselControllableException : InstantiateControllableException
{
    public NonBindlessVesselControllableException()
    {
    }

    public NonBindlessVesselControllableException(string message) : base(message)
    {
    }

    protected NonBindlessVesselControllableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NonBindlessVesselControllableException(string message, Exception inner) : base(message, inner)
    {
    }
}

