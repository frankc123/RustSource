using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/Data Binding/Expression Binding")]
public class dfExpressionPropertyBinding : MonoBehaviour, IDataBindingComponent
{
    private Delegate compiledExpression;
    public Component DataSource;
    public dfComponentMemberInfo DataTarget;
    [SerializeField]
    protected string expression;
    private bool isBound;
    private dfObservableProperty targetProperty;

    public void Bind()
    {
        if (!this.isBound && (!(this.DataSource is dfDataObjectProxy) || (((dfDataObjectProxy) this.DataSource).Data != null)))
        {
            dfScriptEngineSettings settings2 = new dfScriptEngineSettings();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Application", typeof(Application));
            dictionary.Add("Color", typeof(Color));
            dictionary.Add("Color32", typeof(Color32));
            dictionary.Add("Random", typeof(Random));
            dictionary.Add("Time", typeof(Time));
            dictionary.Add("ScriptableObject", typeof(ScriptableObject));
            dictionary.Add("Vector2", typeof(Vector2));
            dictionary.Add("Vector3", typeof(Vector3));
            dictionary.Add("Vector4", typeof(Vector4));
            dictionary.Add("Quaternion", typeof(Quaternion));
            dictionary.Add("Matrix", typeof(Matrix4x4));
            dictionary.Add("Mathf", typeof(Mathf));
            settings2.Constants = dictionary;
            dfScriptEngineSettings settings = settings2;
            if (this.DataSource is dfDataObjectProxy)
            {
                dfDataObjectProxy dataSource = this.DataSource as dfDataObjectProxy;
                settings.AddVariable(new dfScriptVariable("source", null, dataSource.DataType));
            }
            else
            {
                settings.AddVariable(new dfScriptVariable("source", this.DataSource));
            }
            this.compiledExpression = dfScriptEngine.CompileExpression(this.expression, settings);
            this.targetProperty = this.DataTarget.GetProperty();
            this.isBound = (this.compiledExpression != null) && (this.targetProperty != null);
        }
    }

    private void evaluate()
    {
        try
        {
            object dataSource = this.DataSource;
            if (dataSource is dfDataObjectProxy)
            {
                dataSource = ((dfDataObjectProxy) dataSource).Data;
            }
            object[] args = new object[] { dataSource };
            object obj3 = this.compiledExpression.DynamicInvoke(args);
            this.targetProperty.Value = obj3;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    public void OnDisable()
    {
        this.Unbind();
    }

    public override string ToString()
    {
        string str = ((this.DataTarget == null) || (this.DataTarget.Component == null)) ? "[null]" : this.DataTarget.Component.GetType().Name;
        string str2 = ((this.DataTarget == null) || string.IsNullOrEmpty(this.DataTarget.MemberName)) ? "[null]" : this.DataTarget.MemberName;
        return string.Format("Bind [expression] -> {0}.{1}", str, str2);
    }

    public void Unbind()
    {
        if (this.isBound)
        {
            this.compiledExpression = null;
            this.targetProperty = null;
            this.isBound = false;
        }
    }

    public void Update()
    {
        if (this.isBound)
        {
            this.evaluate();
        }
        else if (((this.DataSource != null) && !string.IsNullOrEmpty(this.expression)) && this.DataTarget.IsValid)
        {
            this.Bind();
        }
    }

    public string Expression
    {
        get
        {
            return this.expression;
        }
        set
        {
            if (!string.Equals(value, this.expression))
            {
                this.Unbind();
                this.expression = value;
            }
        }
    }
}

