using System;
using UnityEngine;

public class NavMeshMovement : BaseAIMovement
{
    public NavMeshAgent _agent;
    private Vector3 lastStuckPos = Vector3.zero;
    public Transform movementTransform;
    public float targetLookRotation;

    public void Awake()
    {
    }

    public override bool IsStuck()
    {
        Vector3 vector = base.transform.InverseTransformDirection(this._agent.velocity);
        return ((this._agent.hasPath && (this._agent.speed > 0.5f)) && (vector.z < (this._agent.speed * 0.25f)));
    }

    public override void ProcessNetworkUpdate(ref Vector3 origin, ref Quaternion rotation)
    {
        Vector3 vector;
        Vector3 vector2;
        TransformHelpers.GetGroundInfo(origin + new Vector3(0f, 0.25f, 0f), 10f, out vector, out vector2);
        Vector3 from = (Vector3) (rotation * Vector3.up);
        float num = Vector3.Angle(from, vector2);
        if (num > 20f)
        {
            vector2 = Vector3.Slerp(from, vector2, 20f / num);
        }
        origin = vector;
        rotation = TransformHelpers.LookRotationForcedUp(rotation, vector2);
    }

    public bool RemoveIfNotOnNavmesh()
    {
        if ((this._agent != null) && this._agent.enabled)
        {
            return false;
        }
        TakeDamage.KillSelf(base.GetComponent<IDBase>(), null);
        return true;
    }

    public virtual void SetAgentAiming(bool enabled)
    {
        this._agent.updateRotation = enabled;
    }

    public override void SetLookDirection(Vector3 worldDir)
    {
        this._agent.SetDestination(base.transform.position);
        this._agent.Stop();
        this.SetAgentAiming(false);
        if (worldDir != Vector3.zero)
        {
            this.movementTransform.rotation = Quaternion.LookRotation(worldDir);
        }
    }

    public override void SetMoveDirection(Vector3 worldDir, float speed)
    {
        this.SetAgentAiming(true);
        this._agent.SetDestination(this.movementTransform.position + ((Vector3) (worldDir * 30f)));
        this._agent.speed = speed;
    }

    public override void SetMovePosition(Vector3 worldPos, float speed)
    {
        this.SetAgentAiming(true);
        this._agent.SetDestination(worldPos);
        this._agent.speed = speed;
    }

    public override void SetMoveTarget(GameObject target, float speed)
    {
        this.SetAgentAiming(true);
        Vector3 vector = target.transform.position - base.transform.position;
        this._agent.SetDestination(target.transform.position + ((Vector3) (vector.normalized * 0.5f)));
        this._agent.speed = speed;
    }

    public override void Stop()
    {
        if (!this.RemoveIfNotOnNavmesh())
        {
            this._agent.Stop();
            this.SetAgentAiming(false);
            base.desiredSpeed = 0f;
        }
    }
}

