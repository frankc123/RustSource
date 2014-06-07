using System;
using System.Runtime.Serialization;

[Serializable]
public class NetMainPrefabNameException : ArgumentOutOfRangeException
{
    public NetMainPrefabNameException()
    {
    }

    public NetMainPrefabNameException(string parameter) : base(parameter)
    {
    }

    protected NetMainPrefabNameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NetMainPrefabNameException(string message, Exception inner) : base(message, inner)
    {
    }

    public NetMainPrefabNameException(string parameter, string message) : base(parameter, message)
    {
    }

    public NetMainPrefabNameException(string parameter, string value, string message) : base(parameter, value, message)
    {
    }
}

