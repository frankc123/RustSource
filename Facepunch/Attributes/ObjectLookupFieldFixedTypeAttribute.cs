namespace Facepunch.Attributes
{
    using System;

    public abstract class ObjectLookupFieldFixedTypeAttribute : ObjectLookupFieldAttribute
    {
        public readonly Type[] RequiredComponents;

        protected ObjectLookupFieldFixedTypeAttribute(PrefabLookupKinds kinds, Type minimalType, SearchMode defaultSearchMode, Type[] interfacesRequired) : base(kinds, minimalType, defaultSearchMode, interfacesRequired)
        {
        }

        public Type MinimumType
        {
            get
            {
                return base.MinimumType;
            }
        }
    }
}

