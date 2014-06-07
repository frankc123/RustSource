using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ReflectionExtensions
{
    [CompilerGenerated]
    private static Func<FieldInfo, bool> <>f__am$cache0;

    public static FieldInfo[] GetAllFields(this Type type)
    {
        if (type == null)
        {
            return new FieldInfo[0];
        }
        BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = f => !f.IsDefined(typeof(HideInInspector), true);
        }
        return Enumerable.Where<FieldInfo>(type.GetFields(bindingAttr).Concat<FieldInfo>(type.BaseType.GetAllFields()), <>f__am$cache0).ToArray<FieldInfo>();
    }

    public static object GetProperty(this object target, string property)
    {
        if (target == null)
        {
            throw new NullReferenceException("Target is null");
        }
        MemberInfo[] member = target.GetType().GetMember(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if ((member == null) || (member.Length == 0))
        {
            throw new IndexOutOfRangeException("Property not found: " + property);
        }
        MemberInfo info = member[0];
        if (info is FieldInfo)
        {
            return ((FieldInfo) info).GetValue(target);
        }
        if (!(info is PropertyInfo))
        {
            throw new InvalidOperationException("Member type not supported: " + info.MemberType);
        }
        return ((PropertyInfo) info).GetValue(target, null);
    }

    public static void SetProperty(this object target, string property, object value)
    {
        if (target == null)
        {
            throw new NullReferenceException("Target is null");
        }
        MemberInfo[] member = target.GetType().GetMember(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if ((member == null) || (member.Length == 0))
        {
            throw new IndexOutOfRangeException("Property not found: " + property);
        }
        MemberInfo info = member[0];
        if (info is FieldInfo)
        {
            ((FieldInfo) info).SetValue(target, value);
        }
        else
        {
            if (!(info is PropertyInfo))
            {
                throw new InvalidOperationException("Member type not supported: " + info.MemberType);
            }
            ((PropertyInfo) info).SetValue(target, value, null);
        }
    }
}

