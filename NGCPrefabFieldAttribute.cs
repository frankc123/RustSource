using Facepunch.Attributes;
using System;

public sealed class NGCPrefabFieldAttribute : ObjectLookupFieldFixedTypeAttribute
{
    public NGCPrefabFieldAttribute() : base(PrefabLookupKinds.NGC, typeof(GameObject), SearchMode.MainAsset, null)
    {
    }
}

