using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class dfComponentMemberInfo
{
    public UnityEngine.Component Component;
    public string MemberName;

    public Type GetMemberType()
    {
        Type type = this.Component.GetType();
        MemberInfo info = type.GetMember(this.MemberName).FirstOrDefault<MemberInfo>();
        if (info == null)
        {
            throw new MissingMemberException("Member not found: " + type.Name + "." + this.MemberName);
        }
        if (info is FieldInfo)
        {
            return ((FieldInfo) info).FieldType;
        }
        if (info is PropertyInfo)
        {
            return ((PropertyInfo) info).PropertyType;
        }
        if (info is MethodInfo)
        {
            return ((MethodInfo) info).ReturnType;
        }
        if (!(info is EventInfo))
        {
            throw new InvalidCastException("Invalid member type: " + info.MemberType);
        }
        return ((EventInfo) info).EventHandlerType;
    }

    public MethodInfo GetMethod()
    {
        return (this.Component.GetType().GetMember(this.MemberName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault<MemberInfo>() as MethodInfo);
    }

    public dfObservableProperty GetProperty()
    {
        Type type = this.Component.GetType();
        MemberInfo member = this.Component.GetType().GetMember(this.MemberName).FirstOrDefault<MemberInfo>();
        if (member == null)
        {
            throw new MissingMemberException("Member not found: " + type.Name + "." + this.MemberName);
        }
        if (!(member is FieldInfo) && !(member is PropertyInfo))
        {
            throw new InvalidCastException("Member " + this.MemberName + " is not an observable field or property");
        }
        return new dfObservableProperty(this.Component, member);
    }

    public override string ToString()
    {
        string str = (this.Component == null) ? "[Missing ComponentType]" : this.Component.GetType().Name;
        string str2 = string.IsNullOrEmpty(this.MemberName) ? "[Missing MemberName]" : this.MemberName;
        return string.Format("{0}.{1}", str, str2);
    }

    public bool IsValid
    {
        get
        {
            if ((this.Component == null) || string.IsNullOrEmpty(this.MemberName))
            {
                return false;
            }
            if (this.Component.GetType().GetMember(this.MemberName).FirstOrDefault<MemberInfo>() == null)
            {
                return false;
            }
            return true;
        }
    }
}

