using Facepunch.Attributes;
using System;

public sealed class ControllablePrefabFieldAttribute : ObjectLookupFieldFixedTypeAttribute
{
    public ControllablePrefabFieldAttribute() : base(PrefabLookupKinds.Controllable, typeof(ControllablePrefab), SearchMode.MainAsset, null)
    {
    }
}

