using System;
using System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
public class dfMarkupTagInfoAttribute : Attribute
{
    public dfMarkupTagInfoAttribute(string tagName)
    {
        this.TagName = tagName;
    }

    public string TagName { get; set; }
}

