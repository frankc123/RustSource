using System;
using UnityEngine;

public class CharacterDeathDropPrefabTrait : CharacterTrait
{
    [NonSerialized]
    private bool _loaded;
    [NonSerialized]
    private GameObject _loadedPrefab;
    [NonSerialized]
    private bool _loadFail;
    [SerializeField]
    private string _prefabName;

    public bool hasPrefab
    {
        get
        {
            return (!this._loaded ? ((bool) this.prefab) : !this._loadFail);
        }
    }

    public string instantiateString
    {
        get
        {
            if (this.prefab != null)
            {
                return this._prefabName;
            }
            return null;
        }
    }

    private GameObject prefab
    {
        get
        {
            if (!this._loaded)
            {
                this._loaded = true;
                this._loadFail = ((int) NetCull.LoadPrefab(this._prefabName, out this._loadedPrefab)) == 0;
            }
            return this._loadedPrefab;
        }
    }

    public Transform prefabTransform
    {
        get
        {
            GameObject prefab = this.prefab;
            return ((prefab == null) ? null : prefab.transform);
        }
    }
}

