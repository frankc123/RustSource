using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct RagdollTransferInfo
{
    public readonly string headBoneName;
    public readonly Transform headBone;
    public readonly bool providedHeadBone;
    public readonly bool providedHeadBoneName;
    public RagdollTransferInfo(string headBoneName)
    {
        this.headBoneName = headBoneName;
        this.headBone = null;
        this.providedHeadBone = false;
        this.providedHeadBoneName = headBoneName != null;
    }

    public RagdollTransferInfo(Transform transform)
    {
        this.providedHeadBone = (bool) transform;
        this.providedHeadBoneName = false;
        this.headBoneName = null;
        this.headBone = transform;
    }

    private static void FindNameRecurse(Transform child, StringBuilder sb)
    {
        Transform parent = child.parent;
        if (parent != null)
        {
            FindNameRecurse(parent, sb);
            if (sb.Length > 0)
            {
                sb.Append('/');
            }
            else
            {
                sb.Append(child.name);
            }
        }
    }

    public bool FindHead(Transform root, out Transform headBone)
    {
        Transform transform;
        if (this.providedHeadBoneName)
        {
            headBone = transform = root.Find(this.headBoneName);
            return (bool) transform;
        }
        if (this.providedHeadBone && (this.headBone != null))
        {
            StringBuilder sb = new StringBuilder();
            FindNameRecurse(this.headBone, sb);
            headBone = transform = root.Find(sb.ToString());
            return (bool) transform;
        }
        headBone = root;
        return false;
    }

    public static implicit operator RagdollTransferInfo(string headBoneName)
    {
        return new RagdollTransferInfo(headBoneName);
    }

    public static implicit operator RagdollTransferInfo(Transform transform)
    {
        return new RagdollTransferInfo(transform);
    }
}

