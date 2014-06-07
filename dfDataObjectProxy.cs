using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Data Binding/Proxy Data Object")]
public class dfDataObjectProxy : MonoBehaviour, IDataBindingComponent
{
    private object data;
    [SerializeField]
    protected string typeName;

    public event DataObjectChangedHandler DataChanged;

    public void Bind()
    {
    }

    public dfObservableProperty GetProperty(string PropertyName)
    {
        if (this.data == null)
        {
            return null;
        }
        return new dfObservableProperty(this.data, PropertyName);
    }

    public Type GetPropertyType(string PropertyName)
    {
        Type dataType = this.DataType;
        if (dataType != null)
        {
            MemberInfo info = dataType.GetMember(PropertyName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault<MemberInfo>();
            if (info is FieldInfo)
            {
                return ((FieldInfo) info).FieldType;
            }
            if (info is PropertyInfo)
            {
                return ((PropertyInfo) info).PropertyType;
            }
        }
        return null;
    }

    private Type getTypeFromName(string typeName)
    {
        <getTypeFromName>c__AnonStorey5A storeya = new <getTypeFromName>c__AnonStorey5A {
            typeName = typeName
        };
        return Enumerable.Where<Type>(base.GetType().Assembly.GetTypes(), new Func<Type, bool>(storeya.<>m__2D)).FirstOrDefault<Type>();
    }

    private static Type getTypeFromQualifiedName(string typeName)
    {
        Type type = Type.GetType(typeName);
        if (type != null)
        {
            return type;
        }
        if (typeName.IndexOf('.') == -1)
        {
            return null;
        }
        Assembly assembly = Assembly.Load(typeName.Substring(0, typeName.IndexOf('.')));
        if (assembly == null)
        {
            return null;
        }
        return assembly.GetType(typeName);
    }

    public void Start()
    {
        if (this.DataType == null)
        {
            Debug.LogError("Unable to retrieve System.Type reference for type: " + this.TypeName);
        }
    }

    public void Unbind()
    {
    }

    public object Data
    {
        get
        {
            return this.data;
        }
        set
        {
            if (!object.ReferenceEquals(value, this.data))
            {
                this.data = value;
                if (value != null)
                {
                    this.typeName = value.GetType().Name;
                }
                if (this.DataChanged != null)
                {
                    this.DataChanged(value);
                }
            }
        }
    }

    public Type DataType
    {
        get
        {
            return this.getTypeFromName(this.typeName);
        }
    }

    public string TypeName
    {
        get
        {
            return this.typeName;
        }
        set
        {
            if (this.typeName != value)
            {
                this.typeName = value;
                this.Data = null;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <getTypeFromName>c__AnonStorey5A
    {
        internal string typeName;

        internal bool <>m__2D(Type t)
        {
            return (t.Name == this.typeName);
        }
    }

    [dfEventCategory("Data Changed")]
    public delegate void DataObjectChangedHandler(object data);
}

