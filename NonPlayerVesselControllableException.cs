using System;
using System.Runtime.Serialization;

[Serializable]
public class NonPlayerVesselControllableException : InstantiateControllableException
{
    public NonPlayerVesselControllableException()
    {
    }

    public NonPlayerVesselControllableException(string message) : base(message)
    {
    }

    protected NonPlayerVesselControllableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NonPlayerVesselControllableException(string message, Exception inner) : base(message, inner)
    {
    }
}

