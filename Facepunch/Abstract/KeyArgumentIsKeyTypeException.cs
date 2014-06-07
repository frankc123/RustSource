namespace Facepunch.Abstract
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class KeyArgumentIsKeyTypeException : ArgumentOutOfRangeException
    {
        public KeyArgumentIsKeyTypeException()
        {
        }

        public KeyArgumentIsKeyTypeException(string parameterName) : base(parameterName)
        {
        }

        protected KeyArgumentIsKeyTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public KeyArgumentIsKeyTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        public KeyArgumentIsKeyTypeException(string parameterName, string message) : base(parameterName, message)
        {
        }
    }
}

