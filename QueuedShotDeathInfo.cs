using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct QueuedShotDeathInfo
{
    public bool queued;
    public Vector3 localPoint;
    public Vector3 localNormal;
    public BodyPart bodyPart;
    public Transform transform;
    public bool exists
    {
        get
        {
            return (this.queued && ((bool) this.transform));
        }
    }
    public void Set(Character character, ref Vector3 localPoint, ref Angle2 localNormal, byte bodyPart, ref NetworkMessageInfo info)
    {
        this.Set(character.hitBoxSystem, ref localPoint, ref localNormal, bodyPart, ref info);
    }

    public void Set(IDMain idMain, ref Vector3 localPoint, ref Angle2 localNormal, byte bodyPart, ref NetworkMessageInfo info)
    {
        if (idMain is Character)
        {
            this.Set((Character) idMain, ref localPoint, ref localNormal, bodyPart, ref info);
        }
        else
        {
            this.Set(idMain.GetRemote<HitBoxSystem>(), ref localPoint, ref localNormal, bodyPart, ref info);
        }
    }

    public void LinkRagdoll(Transform thisRoot, GameObject ragdoll)
    {
        if (this.exists)
        {
            Transform transform;
            if (RagdollHelper.RecursiveLinkTransformsByName(ragdoll.transform, thisRoot, this.transform, out transform))
            {
                Transform transform2 = transform;
                Rigidbody rigidbody = transform2.rigidbody;
                if (rigidbody != null)
                {
                    Vector3 position = transform2.TransformPoint(this.localPoint);
                    Vector3 vector2 = transform2.TransformDirection(this.localNormal);
                    rigidbody.AddForceAtPosition((Vector3) (vector2 * 1000f), position);
                }
            }
        }
        else
        {
            RagdollHelper.RecursiveLinkTransformsByName(ragdoll.transform, thisRoot);
        }
    }

    public void Set(HitBoxSystem hitBoxSystem, ref Vector3 localPoint, ref Angle2 localNormal, byte bodyPart, ref NetworkMessageInfo info)
    {
        this.queued = true;
        this.localPoint = localPoint;
        this.localNormal = localNormal.forward;
        this.bodyPart = (BodyPart) bodyPart;
        if (this.bodyPart != BodyPart.Undefined)
        {
            IDRemoteBodyPart part;
            if (hitBoxSystem.bodyParts.TryGetValue(this.bodyPart, out part))
            {
                this.transform = part.transform;
            }
            else
            {
                this.transform = null;
            }
        }
        else
        {
            this.transform = null;
        }
    }
}

