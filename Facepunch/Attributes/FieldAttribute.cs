namespace Facepunch.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=true)]
    public abstract class FieldAttribute : Attribute
    {
        protected FieldAttribute()
        {
        }
    }
}

