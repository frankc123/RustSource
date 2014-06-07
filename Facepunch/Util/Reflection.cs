namespace Facepunch.Util
{
    using System;
    using System.Reflection;

    public class Reflection
    {
        public static bool HasAttribute(MemberInfo method, Type attribute)
        {
            return (method.GetCustomAttributes(attribute, true).Length > 0);
        }
    }
}

