using Facepunch.Attributes;
using System;

public sealed class NetMainPrefabFieldAttribute : ObjectLookupFieldFixedTypeAttribute
{
    public NetMainPrefabFieldAttribute() : base(PrefabLookupKinds.NetMain, typeof(NetMainPrefab), SearchMode.MainAsset, null)
    {
    }
}

