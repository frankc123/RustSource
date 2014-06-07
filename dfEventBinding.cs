using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Data Binding/Event Binding")]
public class dfEventBinding : MonoBehaviour, IDataBindingComponent
{
    public dfComponentMemberInfo DataSource;
    public dfComponentMemberInfo DataTarget;
    private Delegate eventDelegate;
    private FieldInfo eventField;
    private MethodInfo handlerProxy;
    private bool isBound;
    private Component sourceComponent;
    private Component targetComponent;

    private bool areTypesCompatible(ParameterInfo lhs, ParameterInfo rhs)
    {
        return (lhs.ParameterType.Equals(rhs.ParameterType) || lhs.ParameterType.IsAssignableFrom(rhs.ParameterType));
    }

    public void Bind()
    {
        if (!this.isBound && (this.DataSource != null))
        {
            if (!this.DataSource.IsValid || !this.DataTarget.IsValid)
            {
                Debug.LogError(string.Format("Invalid event binding configuration - Source:{0}, Target:{1}", this.DataSource, this.DataTarget));
            }
            else
            {
                this.sourceComponent = this.DataSource.Component;
                this.targetComponent = this.DataTarget.Component;
                MethodInfo method = this.DataTarget.GetMethod();
                if (method == null)
                {
                    Debug.LogError("Event handler not found: " + this.targetComponent.GetType().Name + "." + this.DataTarget.MemberName);
                }
                else
                {
                    this.eventField = this.getField(this.sourceComponent, this.DataSource.MemberName);
                    if (this.eventField == null)
                    {
                        Debug.LogError("Event definition not found: " + this.sourceComponent.GetType().Name + "." + this.DataSource.MemberName);
                    }
                    else
                    {
                        try
                        {
                            ParameterInfo[] parameters = this.eventField.FieldType.GetMethod("Invoke").GetParameters();
                            ParameterInfo[] infoArray2 = method.GetParameters();
                            if (parameters.Length != infoArray2.Length)
                            {
                                if ((parameters.Length <= 0) || (infoArray2.Length != 0))
                                {
                                    base.enabled = false;
                                    throw new InvalidCastException("Event signature mismatch: " + method);
                                }
                                this.eventDelegate = this.createEventProxyDelegate(this.targetComponent, this.eventField.FieldType, parameters, method);
                            }
                            else
                            {
                                this.eventDelegate = Delegate.CreateDelegate(this.eventField.FieldType, this.targetComponent, method, true);
                            }
                        }
                        catch (Exception exception)
                        {
                            base.enabled = false;
                            Debug.LogError("Event binding failed - Failed to create event handler: " + exception.ToString());
                            return;
                        }
                        Delegate delegate2 = Delegate.Combine(this.eventDelegate, (Delegate) this.eventField.GetValue(this.sourceComponent));
                        this.eventField.SetValue(this.sourceComponent, delegate2);
                        this.isBound = true;
                    }
                }
            }
        }
    }

    private void callProxyEventHandler()
    {
        if (this.handlerProxy != null)
        {
            this.handlerProxy.Invoke(this.targetComponent, null);
        }
    }

    [dfEventProxy]
    private void ChildControlEventProxy(dfControl container, dfControl child)
    {
        this.callProxyEventHandler();
    }

    private Delegate createEventProxyDelegate(object target, Type delegateType, ParameterInfo[] eventParams, MethodInfo eventHandler)
    {
        <createEventProxyDelegate>c__AnonStorey5C storeyc = new <createEventProxyDelegate>c__AnonStorey5C {
            eventParams = eventParams,
            <>f__this = this
        };
        MethodInfo method = Enumerable.Where<MethodInfo>(typeof(dfEventBinding).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance), new Func<MethodInfo, bool>(storeyc.<>m__2F)).FirstOrDefault<MethodInfo>();
        if (method == null)
        {
            return null;
        }
        this.handlerProxy = eventHandler;
        return Delegate.CreateDelegate(delegateType, this, method, true);
    }

    [dfEventProxy]
    private void DragEventProxy(dfControl control, dfDragEventArgs dragEvent)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void FocusEventProxy(dfControl control, dfFocusEventArgs args)
    {
        this.callProxyEventHandler();
    }

    private FieldInfo getField(Component sourceComponent, string fieldName)
    {
        <getField>c__AnonStorey5B storeyb = new <getField>c__AnonStorey5B {
            fieldName = fieldName
        };
        return Enumerable.Where<FieldInfo>(sourceComponent.GetType().GetAllFields(), new Func<FieldInfo, bool>(storeyb.<>m__2E)).FirstOrDefault<FieldInfo>();
    }

    [dfEventProxy]
    private void KeyEventProxy(dfControl control, dfKeyEventArgs keyEvent)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void MouseEventProxy(dfControl control, dfMouseEventArgs mouseEvent)
    {
        this.callProxyEventHandler();
    }

    public void OnDisable()
    {
        this.Unbind();
    }

    public void OnEnable()
    {
        if (((this.DataSource != null) && !this.isBound) && (this.DataSource.IsValid && this.DataTarget.IsValid))
        {
            this.Bind();
        }
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, dfButton.ButtonState value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, dfPivotPoint value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, bool value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, int value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, float value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, string value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Material value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Quaternion value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Texture2D value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Vector2 value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Vector3 value)
    {
        this.callProxyEventHandler();
    }

    [dfEventProxy]
    private void PropertyChangedProxy(dfControl control, Vector4 value)
    {
        this.callProxyEventHandler();
    }

    private bool signatureIsCompatible(ParameterInfo[] lhs, ParameterInfo[] rhs)
    {
        if ((lhs == null) || (rhs == null))
        {
            return false;
        }
        if (lhs.Length != rhs.Length)
        {
            return false;
        }
        for (int i = 0; i < lhs.Length; i++)
        {
            if (!this.areTypesCompatible(lhs[i], rhs[i]))
            {
                return false;
            }
        }
        return true;
    }

    public void Start()
    {
        if (((this.DataSource != null) && !this.isBound) && (this.DataSource.IsValid && this.DataTarget.IsValid))
        {
            this.Bind();
        }
    }

    public override string ToString()
    {
        string str = ((this.DataSource == null) || (this.DataSource.Component == null)) ? "[null]" : this.DataSource.Component.GetType().Name;
        string str2 = ((this.DataSource == null) || string.IsNullOrEmpty(this.DataSource.MemberName)) ? "[null]" : this.DataSource.MemberName;
        string str3 = ((this.DataTarget == null) || (this.DataTarget.Component == null)) ? "[null]" : this.DataTarget.Component.GetType().Name;
        string str4 = ((this.DataTarget == null) || string.IsNullOrEmpty(this.DataTarget.MemberName)) ? "[null]" : this.DataTarget.MemberName;
        object[] args = new object[] { str, str2, str3, str4 };
        return string.Format("Bind {0}.{1} -> {2}.{3}", args);
    }

    public void Unbind()
    {
        if (this.isBound)
        {
            this.isBound = false;
            Delegate source = (Delegate) this.eventField.GetValue(this.sourceComponent);
            Delegate delegate3 = Delegate.Remove(source, this.eventDelegate);
            this.eventField.SetValue(this.sourceComponent, delegate3);
            this.eventField = null;
            this.eventDelegate = null;
            this.handlerProxy = null;
            this.sourceComponent = null;
            this.targetComponent = null;
        }
    }

    [CompilerGenerated]
    private sealed class <createEventProxyDelegate>c__AnonStorey5C
    {
        internal dfEventBinding <>f__this;
        internal ParameterInfo[] eventParams;

        internal bool <>m__2F(MethodInfo m)
        {
            return (m.IsDefined(typeof(dfEventProxyAttribute), true) && this.<>f__this.signatureIsCompatible(this.eventParams, m.GetParameters()));
        }
    }

    [CompilerGenerated]
    private sealed class <getField>c__AnonStorey5B
    {
        internal string fieldName;

        internal bool <>m__2E(FieldInfo f)
        {
            return (f.Name == this.fieldName);
        }
    }
}

