using Facepunch.Attributes;
using System;

public sealed class CharacterPrefabFieldAttribute : ObjectLookupFieldFixedTypeAttribute
{
    public CharacterPrefabFieldAttribute() : base(PrefabLookupKinds.Character, typeof(CharacterPrefab), SearchMode.MainAsset, null)
    {
    }
}

