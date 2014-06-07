using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

public class dfObservableProperty : IObservableValue
{
    private FieldInfo fieldInfo;
    private bool hasChanged;
    private object lastValue;
    private MethodInfo propertyGetter;
    private PropertyInfo propertyInfo;
    private object target;

    internal dfObservableProperty(object target, FieldInfo field)
    {
        this.initField(target, field);
    }

    internal dfObservableProperty(object target, MemberInfo member)
    {
        this.initMember(target, member);
    }

    internal dfObservableProperty(object target, PropertyInfo property)
    {
        this.initProperty(target, property);
    }

    internal dfObservableProperty(object target, string memberName)
    {
        MemberInfo member = target.GetType().GetMember(memberName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault<MemberInfo>();
        if (member == null)
        {
            throw new ArgumentException("Invalid property or field name: " + memberName, "memberName");
        }
        this.initMember(target, member);
    }

    public void ClearChangedFlag()
    {
        this.hasChanged = false;
        this.lastValue = this.getter();
    }

    private object getFieldValue()
    {
        return this.fieldInfo.GetValue(this.target);
    }

    private object getPropertyValue()
    {
        if (this.propertyGetter == null)
        {
            this.propertyGetter = this.propertyInfo.GetGetMethod();
            if (this.propertyGetter == null)
            {
                throw new InvalidOperationException("Cannot read property: " + this.propertyInfo);
            }
        }
        return this.propertyGetter.Invoke(this.target, null);
    }

    private object getter()
    {
        if (this.propertyInfo != null)
        {
            return this.getPropertyValue();
        }
        return this.getFieldValue();
    }

    private void initField(object target, FieldInfo field)
    {
        this.target = target;
        this.fieldInfo = field;
        this.Value = this.getter();
    }

    private void initMember(object target, MemberInfo member)
    {
        if (member is FieldInfo)
        {
            this.initField(target, (FieldInfo) member);
        }
        else
        {
            this.initProperty(target, (PropertyInfo) member);
        }
    }

    private void initProperty(object target, PropertyInfo property)
    {
        this.target = target;
        this.propertyInfo = property;
        this.Value = this.getter();
    }

    private void setFieldValue(object value)
    {
        if (!this.fieldInfo.IsLiteral)
        {
            Type fieldType = this.fieldInfo.FieldType;
            if ((value == null) || fieldType.IsAssignableFrom(value.GetType()))
            {
                this.fieldInfo.SetValue(this.target, value);
            }
            else
            {
                object obj2 = Convert.ChangeType(value, fieldType);
                this.fieldInfo.SetValue(this.target, obj2);
            }
        }
    }

    private void setFieldValueNOP(object value)
    {
    }

    private void setPropertyValue(object value)
    {
        MethodInfo setMethod = this.propertyInfo.GetSetMethod();
        if (this.propertyInfo.CanWrite && (setMethod != null))
        {
            Type propertyType = this.propertyInfo.PropertyType;
            if ((value == null) || propertyType.IsAssignableFrom(value.GetType()))
            {
                this.propertyInfo.SetValue(this.target, value, null);
            }
            else
            {
                object obj2 = Convert.ChangeType(value, propertyType);
                this.propertyInfo.SetValue(this.target, obj2, null);
            }
        }
    }

    private void setter(object value)
    {
        if (this.propertyInfo != null)
        {
            this.setPropertyValue(value);
        }
        else
        {
            this.setFieldValue(value);
        }
    }

    public bool HasChanged
    {
        get
        {
            if (this.hasChanged)
            {
                return true;
            }
            object objA = this.getter();
            if (object.ReferenceEquals(objA, this.lastValue))
            {
                this.hasChanged = false;
            }
            else if ((objA == null) || (this.lastValue == null))
            {
                this.hasChanged = true;
            }
            else
            {
                this.hasChanged = !objA.Equals(this.lastValue);
            }
            return this.hasChanged;
        }
    }

    public object Value
    {
        get
        {
            return this.getter();
        }
        set
        {
            this.lastValue = value;
            this.setter(value);
            this.hasChanged = false;
        }
    }

    private delegate object ValueGetter();

    private delegate void ValueSetter(object value);
}

