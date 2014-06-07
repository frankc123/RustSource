using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class InterfaceDriverComponentAttribute : Attribute
{
    private Type _minimumType = typeof(MonoBehaviour);
    public readonly Type Interface;
    public readonly string RuntimeFieldName;
    private InterfaceSearchRoute searchRoute = InterfaceSearchRoute.GameObject;
    public readonly string SerializedFieldName;

    public InterfaceDriverComponentAttribute(Type interfaceType, string serializedFieldName, string runtimeFieldName)
    {
        this.Interface = interfaceType;
        this.SerializedFieldName = serializedFieldName;
        this.RuntimeFieldName = runtimeFieldName;
    }

    public string AdditionalProperties { get; set; }

    public bool AlwaysSaveDisabled { get; set; }

    public InterfaceSearchRoute SearchRoute
    {
        get
        {
            return this.searchRoute;
        }
        set
        {
            if (value == 0)
            {
                value = InterfaceSearchRoute.GameObject;
            }
            this.searchRoute = value;
        }
    }

    public Type UnityType
    {
        get
        {
            return this._minimumType;
        }
        set
        {
            Type expressionStack_13_0;
            InterfaceDriverComponentAttribute expressionStack_13_1;
            InterfaceDriverComponentAttribute expressionStack_8_1;
            if (value != null)
            {
                expressionStack_13_1 = this;
                expressionStack_13_0 = value;
                goto Label_0013;
            }
            else
            {
                expressionStack_8_1 = this;
                Type expressionStack_8_0 = value;
            }
            expressionStack_13_1 = expressionStack_8_1;
            expressionStack_13_0 = typeof(MonoBehaviour);
        Label_0013:
            expressionStack_13_1._minimumType = expressionStack_13_0;
        }
    }
}

