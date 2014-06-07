using Facepunch.Abstract;
using System;
using UnityEngine;

public abstract class TraitMap<Key> : BaseTraitMap where Key: TraitKey
{
    [NonSerialized]
    private bool createdDict;
    [NonSerialized]
    private KeyTypeInfo<Key>.TraitDictionary dict;
    [SerializeField, HideInInspector]
    private Key[] K;

    protected TraitMap()
    {
    }

    public T GetTrait<T>() where T: Key
    {
        return this.map.TryGetSoftCast<T>();
    }

    public Key GetTrait(Type traitType)
    {
        return this.map.TryGet(traitType);
    }

    internal abstract TraitMap<Key> __baseMap { get; }

    private KeyTypeInfo<Key>.TraitDictionary map
    {
        get
        {
            if (!this.createdDict)
            {
                this.dict = new KeyTypeInfo<Key>.TraitDictionary(this.K);
                TraitMap<Key> map = this.__baseMap;
                if (map != null)
                {
                    map.map.MergeUpon(this.dict);
                }
                this.createdDict = true;
            }
            return this.dict;
        }
    }
}

