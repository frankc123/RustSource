using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReferenceTypeHelper
{
    private static readonly Dictionary<Type, bool> cache = new Dictionary<Type, bool>();

    public static bool TreatAsReferenceHolder(Type type)
    {
        bool flag;
        if (!cache.TryGetValue(type, out flag))
        {
            if (type.IsByRef)
            {
                flag = true;
            }
            else if (type.IsEnum)
            {
                flag = false;
            }
            else
            {
                foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    Type fieldType = info.FieldType;
                    if (fieldType.IsByRef || !TreatAsReferenceHolder(fieldType))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            cache[type] = flag;
        }
        return flag;
    }
}

