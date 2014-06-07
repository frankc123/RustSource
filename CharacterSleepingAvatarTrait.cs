using System;
using UnityEngine;

public class CharacterSleepingAvatarTrait : CharacterTrait
{
    [SerializeField]
    private bool _allowDroppingOfInventory;
    [SerializeField]
    private bool _grabCarrierOnCreate;
    [NonSerialized]
    private bool _hasInventory;
    [NonSerialized]
    private bool _hasTakeDamage;
    [NonSerialized]
    private bool? _prefabValid;
    [SerializeField]
    private string _sleepingAvatarPrefab;
    [NonSerialized]
    private Type _takeDamageType;
    [SerializeField]
    private Vector3 boxCenter;
    [SerializeField]
    private Vector3 boxSize;

    public Vector3 SolvePlacement(Vector3 origin, Quaternion rot, int iter)
    {
        return TransformHelpers.TestBoxCorners(origin, rot, this.boxCenter, this.boxSize, 0x400, iter);
    }

    private bool ValidatePrefab()
    {
        GameObject obj2;
        if (string.IsNullOrEmpty(this._sleepingAvatarPrefab))
        {
            return false;
        }
        NetCull.PrefabSearch search = NetCull.LoadPrefab(this._sleepingAvatarPrefab, out obj2);
        if (((int) search) != 1)
        {
            Debug.LogError(string.Format("sleeping avatar prefab named \"{0}\" resulted in {1} which was not {2}(required)", this.prefab, search, NetCull.PrefabSearch.NGC));
            return false;
        }
        IDMain component = obj2.GetComponent<IDMain>();
        if (!(component is SleepingAvatar))
        {
            Debug.LogError(string.Format("Theres no Sleeping avatar on prefab \"{0}\"", this.prefab), obj2);
            return false;
        }
        this._hasInventory = component.GetLocal<Inventory>();
        TakeDamage local = component.GetLocal<TakeDamage>();
        this._hasTakeDamage = (bool) local;
        this._takeDamageType = !this._hasTakeDamage ? null : local.GetType();
        return true;
    }

    public bool canDropInventories
    {
        get
        {
            return (this._allowDroppingOfInventory && this.hasInventory);
        }
    }

    public bool grabsCarrierOnCreate
    {
        get
        {
            return (this.valid && this._grabCarrierOnCreate);
        }
    }

    public bool hasInventory
    {
        get
        {
            return (this.valid && this._hasInventory);
        }
    }

    public bool hasTakeDamage
    {
        get
        {
            return (this.valid && this._hasTakeDamage);
        }
    }

    public string prefab
    {
        get
        {
            if (this._sleepingAvatarPrefab == null)
            {
            }
            return string.Empty;
        }
    }

    public Type takeDamageType
    {
        get
        {
            if (!this.hasTakeDamage)
            {
                throw new InvalidOperationException("You need to check hasTakeDamage before requesting this. hasTakeDamage == False");
            }
            return this._takeDamageType;
        }
    }

    public bool valid
    {
        get
        {
            bool? nullable2;
            bool? nullable = this._prefabValid;
            return (!nullable.HasValue ? (nullable2 = nullable2).Value : nullable.Value);
        }
    }
}

