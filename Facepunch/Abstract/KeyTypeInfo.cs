namespace Facepunch.Abstract
{
    using System;

    internal static class KeyTypeInfo
    {
        public static int ForcedDifCompareValue(Type x, Type y)
        {
            int num = x.GetHashCode().CompareTo(y.GetHashCode());
            if (num == 0)
            {
                num = x.AssemblyQualifiedName.CompareTo(y.AssemblyQualifiedName);
                if (num == 0)
                {
                    num = x.TypeHandle.Value.ToInt64().CompareTo(y.TypeHandle.Value);
                    if (num == 0)
                    {
                        throw new InvalidProgramException();
                    }
                }
            }
            return num;
        }
    }
}

