using System;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Data Binding/Property Binding")]
public class dfPropertyBinding : MonoBehaviour, IDataBindingComponent
{
    public dfComponentMemberInfo DataSource;
    public dfComponentMemberInfo DataTarget;
    private bool isBound;
    private dfObservableProperty sourceProperty;
    private dfObservableProperty targetProperty;
    public bool TwoWay;

    public void Bind()
    {
        if (!this.isBound)
        {
            if (!this.DataSource.IsValid || !this.DataTarget.IsValid)
            {
                Debug.LogError(string.Format("Invalid data binding configuration - Source:{0}, Target:{1}", this.DataSource, this.DataTarget));
            }
            else
            {
                this.sourceProperty = this.DataSource.GetProperty();
                this.targetProperty = this.DataTarget.GetProperty();
                this.isBound = (this.sourceProperty != null) && (this.targetProperty != null);
                if (this.isBound)
                {
                    this.targetProperty.Value = this.sourceProperty.Value;
                }
            }
        }
    }

    public void OnDisable()
    {
        this.Unbind();
    }

    public void OnEnable()
    {
        if ((!this.isBound && this.DataSource.IsValid) && this.DataTarget.IsValid)
        {
            this.Bind();
        }
    }

    public void Start()
    {
        if ((!this.isBound && this.DataSource.IsValid) && this.DataTarget.IsValid)
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
            this.sourceProperty = null;
            this.targetProperty = null;
            this.isBound = false;
        }
    }

    public void Update()
    {
        if ((this.sourceProperty != null) && (this.targetProperty != null))
        {
            if (this.sourceProperty.HasChanged)
            {
                this.targetProperty.Value = this.sourceProperty.Value;
                this.sourceProperty.ClearChangedFlag();
            }
            else if (this.TwoWay && this.targetProperty.HasChanged)
            {
                this.sourceProperty.Value = this.targetProperty.Value;
                this.targetProperty.ClearChangedFlag();
            }
        }
    }
}

