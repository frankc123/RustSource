using Facepunch.MeshBatch;
using RustProto;
using RustProto.Helpers;
using System;
using UnityEngine;

[NGCAutoAddScript]
public class DeployableObject : IDMain, IDeployedObjectMain, IServerSaveable, IServerSaveNotify, ICarriableTrans
{
    public TransCarrier _carrier;
    private EnvDecay _EnvDecay;
    public bool cantPlaceOn;
    public GameObject clientDeathEffect;
    public GameObject corpseObject;
    public ulong creatorID;
    public bool decayProtector;
    public bool doEdgeCheck;
    public bool handleDeathHere;
    [NonSerialized]
    private HealthDimmer healthDimmer;
    public float maxEdgeDifferential;
    public float maxSlope;
    public ulong ownerID;
    public string ownerName;

    public DeployableObject() : this(IDFlags.Unknown)
    {
    }

    protected DeployableObject(IDFlags flags) : base(flags)
    {
        this.maxEdgeDifferential = 1f;
        this.maxSlope = 30f;
        this.ownerName = string.Empty;
    }

    public void Awake()
    {
    }

    public bool BelongsTo(Controllable controllable)
    {
        if (controllable == null)
        {
            return false;
        }
        PlayerClient playerClient = controllable.playerClient;
        if (playerClient == null)
        {
            return false;
        }
        return (playerClient.userID == this.ownerID);
    }

    public void CacheCreator()
    {
    }

    [RPC]
    public void Client_OnKilled()
    {
        if (this.clientDeathEffect != null)
        {
            GameObject obj2 = Object.Instantiate(this.clientDeathEffect, base.transform.position, base.transform.rotation) as GameObject;
            Object.Destroy(obj2, 5f);
        }
    }

    [RPC]
    public void ClientHealthUpdate(float newHealth)
    {
        this.healthDimmer.UpdateHealthAmount(this, newHealth, false);
    }

    public TransCarrier GetCarrier()
    {
        return this._carrier;
    }

    [RPC]
    public void GetOwnerInfo(ulong creator, ulong owner)
    {
        this.creatorID = creator;
        this.ownerID = owner;
    }

    public void GrabCarrier()
    {
        RaycastHit hit;
        bool flag;
        MeshBatchInstance instance;
        Ray ray = new Ray(base.transform.position + ((Vector3) (Vector3.up * 0.01f)), Vector3.down);
        if (MeshBatchPhysics.Raycast(ray, out hit, 5f, out flag, out instance))
        {
            IDMain main = !flag ? IDBase.GetMain(hit.collider) : instance.idMain;
            if (main != null)
            {
                TransCarrier local = main.GetLocal<TransCarrier>();
                if (local != null)
                {
                    local.AddObject(this);
                }
            }
        }
    }

    void IServerSaveNotify.PostLoad()
    {
    }

    public static bool IsValidLocation(Vector3 location, Vector3 surfaceNormal, Quaternion rotation, DeployableObject prefab)
    {
        if (prefab.doEdgeCheck)
        {
            return false;
        }
        return (Vector3.Angle(surfaceNormal, Vector3.up) <= prefab.maxSlope);
    }

    public void OnAddedToCarrier(TransCarrier carrier)
    {
        this._carrier = carrier;
    }

    public void OnDestroy()
    {
        if (this._carrier != null)
        {
            this._carrier.RemoveObject(this);
            this._carrier = null;
        }
        base.OnDestroy();
    }

    public void OnDroppedFromCarrier(TransCarrier carrier)
    {
        this._carrier = null;
    }

    protected void OnPoolAlive()
    {
        this.ownerID = 0L;
        this.ownerName = string.Empty;
        this.creatorID = 0L;
    }

    protected void OnPoolRetire()
    {
        this.healthDimmer.Reset();
    }

    public void ReadObjectSave(ref SavedObject saveobj)
    {
        if (saveobj.HasDeployable)
        {
            this.creatorID = saveobj.Deployable.CreatorID;
            this.ownerID = saveobj.Deployable.OwnerID;
        }
    }

    public void Touched()
    {
        TransCarrier carrier = this.GetCarrier();
        if (carrier != null)
        {
            IDMain idMain = carrier.idMain;
            if ((idMain != null) && (idMain is StructureComponent))
            {
                ((StructureComponent) idMain).Touched();
            }
        }
    }

    public void WriteObjectSave(ref SavedObject.Builder saveobj)
    {
        using (Recycler<objectDeployable, objectDeployable.Builder> recycler = objectDeployable.Recycler())
        {
            objectDeployable.Builder builderForValue = recycler.OpenBuilder();
            builderForValue.SetCreatorID(this.creatorID);
            builderForValue.SetOwnerID(this.ownerID);
            saveobj.SetDeployable(builderForValue);
        }
        using (Recycler<objectICarriableTrans, objectICarriableTrans.Builder> recycler2 = objectICarriableTrans.Recycler())
        {
            NetEntityID yid;
            objectICarriableTrans.Builder builder2 = recycler2.OpenBuilder();
            if ((this._carrier != null) && (((int) NetEntityID.Of((MonoBehaviour) this._carrier, out yid)) != 0))
            {
                builder2.SetTransCarrierID(yid.id);
            }
            else
            {
                builder2.ClearTransCarrierID();
            }
            saveobj.SetCarriableTrans(builder2);
        }
    }

    DeployedObjectInfo IDeployedObjectMain.DeployedObjectInfo
    {
        get
        {
            DeployedObjectInfo info;
            info.userID = this.ownerID;
            info.valid = this.ownerID != 0L;
            return info;
        }
    }
}

