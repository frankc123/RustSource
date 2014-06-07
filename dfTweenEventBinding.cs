using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Tweens/Tween Event Binding")]
public class dfTweenEventBinding : MonoBehaviour
{
    [CompilerGenerated]
    private static Func<ParameterInfo, Type> <>f__am$cacheC;
    public Component EventSource;
    private bool isBound;
    public string ResetEvent;
    private FieldInfo resetEventField;
    private Delegate resetEventHandler;
    public string StartEvent;
    private FieldInfo startEventField;
    private Delegate startEventHandler;
    public string StopEvent;
    private FieldInfo stopEventField;
    private Delegate stopEventHandler;
    public Component Tween;

    public void Bind()
    {
        if (!this.isBound || this.isValid())
        {
            this.isBound = true;
            if (!string.IsNullOrEmpty(this.StartEvent))
            {
                this.bindEvent(this.StartEvent, "Play", out this.startEventField, out this.startEventHandler);
            }
            if (!string.IsNullOrEmpty(this.StopEvent))
            {
                this.bindEvent(this.StopEvent, "Stop", out this.stopEventField, out this.stopEventHandler);
            }
            if (!string.IsNullOrEmpty(this.ResetEvent))
            {
                this.bindEvent(this.ResetEvent, "Reset", out this.resetEventField, out this.resetEventHandler);
            }
        }
    }

    private void bindEvent(string eventName, string handlerName, out FieldInfo eventField, out Delegate eventHandler)
    {
        eventField = null;
        eventHandler = null;
        MethodInfo method = this.Tween.GetType().GetMethod(handlerName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (method == null)
        {
            throw new MissingMemberException("Method not found: " + handlerName);
        }
        eventField = this.getField(this.EventSource.GetType(), eventName);
        if (eventField == null)
        {
            throw new MissingMemberException("Event not found: " + eventName);
        }
        try
        {
            ParameterInfo[] parameters = eventField.FieldType.GetMethod("Invoke").GetParameters();
            ParameterInfo[] infoArray2 = method.GetParameters();
            if (parameters.Length != infoArray2.Length)
            {
                if ((parameters.Length <= 0) || (infoArray2.Length != 0))
                {
                    throw new InvalidCastException("Event signature mismatch: " + eventHandler);
                }
                eventHandler = this.createDynamicWrapper(this.Tween, eventField.FieldType, parameters, method);
            }
            else
            {
                eventHandler = Delegate.CreateDelegate(eventField.FieldType, this.Tween, method, true);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError("Event binding failed - Failed to create event handler: " + exception.ToString());
            return;
        }
        Delegate delegate2 = Delegate.Combine(eventHandler, (Delegate) eventField.GetValue(this.EventSource));
        eventField.SetValue(this.EventSource, delegate2);
    }

    private Delegate createDynamicWrapper(object target, Type delegateType, ParameterInfo[] eventParams, MethodInfo eventHandler)
    {
        Type[] first = new Type[] { target.GetType() };
        if (<>f__am$cacheC == null)
        {
            <>f__am$cacheC = p => p.ParameterType;
        }
        Type[] parameterTypes = first.Concat<Type>(Enumerable.Select<ParameterInfo, Type>(eventParams, <>f__am$cacheC)).ToArray<Type>();
        DynamicMethod method = new DynamicMethod("DynamicEventWrapper_" + eventHandler.Name, typeof(void), parameterTypes);
        ILGenerator iLGenerator = method.GetILGenerator();
        iLGenerator.Emit(OpCodes.Ldarg_0);
        iLGenerator.EmitCall(OpCodes.Callvirt, eventHandler, Type.EmptyTypes);
        iLGenerator.Emit(OpCodes.Ret);
        return method.CreateDelegate(delegateType, target);
    }

    private FieldInfo getField(Type type, string fieldName)
    {
        <getField>c__AnonStorey5E storeye = new <getField>c__AnonStorey5E {
            fieldName = fieldName
        };
        return Enumerable.Where<FieldInfo>(type.GetAllFields(), new Func<FieldInfo, bool>(storeye.<>m__33)).FirstOrDefault<FieldInfo>();
    }

    private bool isValid()
    {
        if ((this.Tween == null) || !(this.Tween is dfTweenComponentBase))
        {
            return false;
        }
        if (this.EventSource == null)
        {
            return false;
        }
        if ((string.IsNullOrEmpty(this.StartEvent) && string.IsNullOrEmpty(this.StopEvent)) && string.IsNullOrEmpty(this.ResetEvent))
        {
            return false;
        }
        Type type = this.EventSource.GetType();
        if (!string.IsNullOrEmpty(this.StartEvent) && (this.getField(type, this.StartEvent) == null))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(this.StopEvent) && (this.getField(type, this.StopEvent) == null))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(this.ResetEvent) && (this.getField(type, this.ResetEvent) == null))
        {
            return false;
        }
        return true;
    }

    private void OnDisable()
    {
        this.Unbind();
    }

    private void OnEnable()
    {
        if (this.isValid())
        {
            this.Bind();
        }
    }

    private void Start()
    {
        if (this.isValid())
        {
            this.Bind();
        }
    }

    public void Unbind()
    {
        if (this.isBound)
        {
            this.isBound = false;
            if (this.startEventField != null)
            {
                this.unbindEvent(this.startEventField, this.startEventHandler);
                this.startEventField = null;
                this.startEventHandler = null;
            }
            if (this.stopEventField != null)
            {
                this.unbindEvent(this.stopEventField, this.stopEventHandler);
                this.stopEventField = null;
                this.stopEventHandler = null;
            }
            if (this.resetEventField != null)
            {
                this.unbindEvent(this.resetEventField, this.resetEventHandler);
                this.resetEventField = null;
                this.resetEventHandler = null;
            }
        }
    }

    private void unbindEvent(FieldInfo eventField, Delegate eventDelegate)
    {
        Delegate source = (Delegate) eventField.GetValue(this.EventSource);
        Delegate delegate3 = Delegate.Remove(source, eventDelegate);
        eventField.SetValue(this.EventSource, delegate3);
    }

    [CompilerGenerated]
    private sealed class <getField>c__AnonStorey5E
    {
        internal string fieldName;

        internal bool <>m__33(FieldInfo f)
        {
            return (f.Name == this.fieldName);
        }
    }
}

