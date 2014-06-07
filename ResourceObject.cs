using System;
using uLink;
using UnityEngine;

public class ResourceObject : IDMain
{
    private int _lastModelIndex;
    public MeshCollider _meshCollider;
    public MeshFilter _meshFilter;
    private GenericSpawner _mySpawner;
    private int _pendingMeshIndex;
    public ResourceTarget _resTarg;
    public GameObject clientMeshChangeEffect;
    public Mesh[] collisionMeshes;
    private NetEntityID myID;
    public Mesh[] visualMeshes;

    public ResourceObject() : base(IDFlags.Unknown)
    {
        this._pendingMeshIndex = -1;
        this._lastModelIndex = -1;
    }

    public void ChangeModelIndex(int index)
    {
        this._meshCollider.sharedMesh = this.collisionMeshes[index];
        this._meshFilter.sharedMesh = this.visualMeshes[index];
        this._lastModelIndex = index;
    }

    public void DelayedModelChangeIndex()
    {
        this.ChangeModelIndex(this._pendingMeshIndex);
    }

    [RPC]
    public void modelindex(int index, NetworkMessageInfo info)
    {
        bool flag = false;
        if (((EnvironmentControlCenter.Singleton != null) && EnvironmentControlCenter.Singleton.IsNight()) && ((PlayerClient.GetLocalPlayer().controllable != null) && (Vector3.Distance(PlayerClient.GetLocalPlayer().controllable.transform.position, base.transform.position) > 20f)))
        {
            flag = true;
        }
        if (((this.clientMeshChangeEffect != null) && (this._lastModelIndex != -1)) && !flag)
        {
            GameObject obj2 = Object.Instantiate(this.clientMeshChangeEffect, base.transform.position, base.transform.rotation) as GameObject;
            Object.Destroy(obj2, 5f);
        }
        this._pendingMeshIndex = index;
        base.Invoke("DelayedModelChangeIndex", 0.15f);
    }

    private void NGC_OnInstantiate(NGCView view)
    {
        this.myID = NetEntityID.Get((MonoBehaviour) this);
        this._resTarg = base.GetComponent<ResourceTarget>();
    }

    public void SetSpawner(GameObject spawner)
    {
        this._mySpawner = spawner.GetComponent<GenericSpawner>();
    }
}

