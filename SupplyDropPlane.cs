using System;
using uLink;
using UnityEngine;

public class SupplyDropPlane : IDMain
{
    protected TransformInterpolator _interp;
    protected double _lastMoveTime;
    protected bool approachingTarget;
    protected bool droppedPayload;
    public Vector3 dropTargetPos;
    protected float lastDist;
    public float maxSpeed;
    private bool passedTarget;
    public GameObject[] propellers;
    public Quaternion startAng;
    public Vector3 startPos;
    protected Vector3 targetPos;
    protected float targetReachedTime;
    public int TEMP_numCratesToDrop;

    public SupplyDropPlane() : this(IDFlags.Unknown)
    {
    }

    protected SupplyDropPlane(IDFlags idFlags) : base(idFlags)
    {
        this.maxSpeed = 250f;
        this.lastDist = float.PositiveInfinity;
        this.approachingTarget = true;
        this.TEMP_numCratesToDrop = 3;
    }

    [RPC]
    public void GetNetworkUpdate(Vector3 pos, Quaternion rot, NetworkMessageInfo info)
    {
        this._interp.SetGoals(pos, rot, info.timestamp);
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this._interp = base.GetComponent<TransformInterpolator>();
        this._interp.running = true;
    }

    public void Update()
    {
        foreach (GameObject obj2 in this.propellers)
        {
            obj2.transform.RotateAroundLocal(Vector3.forward, 12f * Time.deltaTime);
        }
    }
}

