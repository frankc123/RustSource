using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class CCDesc : MonoBehaviour
{
    [NonSerialized]
    internal HitManager AssignedHitManager;
    [SerializeField]
    private Vector3 m_Center;
    [SerializeField, HideInInspector, PrefetchComponent]
    private CharacterController m_Collider;
    [SerializeField]
    private float m_Height = 2f;
    [SerializeField]
    private float m_MinMoveDistance;
    [SerializeField]
    private float m_Radius = 0.4f;
    [SerializeField]
    private float m_SkinWidth = 0.05f;
    [SerializeField]
    private float m_SlopeLimit = 90f;
    [SerializeField]
    private float m_StepOffset = 0.5f;
    private static CCDesc s_CurrentMovingCCDesc;
    [NonSerialized]
    public object Tag;

    public CCDesc()
    {
        Vector3 vector = new Vector3 {
            y = 1f
        };
        this.m_Center = vector;
    }

    public Vector3 ClosestPointOnBounds(Vector3 position)
    {
        return this.m_Collider.ClosestPointOnBounds(position);
    }

    public HeightModification ModifyHeight(float newEffectiveSkinnedHeight, bool preview = false)
    {
        HeightModification modification;
        float num = this.m_Radius + this.m_Radius;
        float num2 = (this.m_SkinWidth + this.m_SkinWidth) + num;
        float num3 = (num <= this.m_Height) ? ((this.m_Height + this.m_SkinWidth) + this.m_SkinWidth) : num2;
        modification.original.effectiveSkinnedHeight = num3;
        modification.original.center = this.m_Center;
        if (newEffectiveSkinnedHeight < num2)
        {
            modification.modified.effectiveSkinnedHeight = num2;
        }
        else
        {
            modification.modified.effectiveSkinnedHeight = newEffectiveSkinnedHeight;
        }
        if (modification.differed = !(modification.original.effectiveSkinnedHeight == modification.modified.effectiveSkinnedHeight))
        {
            float num4 = num3 * 0.5f;
            float num5 = modification.original.center.y - num4;
            float num6 = modification.original.center.y + num4;
            modification.delta.effectiveSkinnedHeight = modification.modified.effectiveSkinnedHeight - modification.original.effectiveSkinnedHeight;
            modification.scale = modification.modified.effectiveSkinnedHeight / modification.original.effectiveSkinnedHeight;
            float num7 = num5 * modification.scale;
            float num8 = num6 * modification.scale;
            modification.modified.center.x = modification.original.center.x;
            modification.modified.center.z = modification.original.center.z;
            modification.modified.center.y = num7 + ((num8 - num7) * 0.5f);
            modification.delta.center.x = 0f;
            modification.delta.center.z = 0f;
            modification.delta.center.y = modification.modified.center.y - modification.original.center.y;
            if (modification.applied = !preview)
            {
                this.m_Height = modification.modified.effectiveSkinnedHeight - (this.m_SkinWidth + this.m_SkinWidth);
                this.m_Center = modification.modified.center;
                if (modification.scale < 1f)
                {
                    this.m_Collider.center = this.m_Center;
                    this.m_Collider.height = this.m_Height;
                    return modification;
                }
                this.m_Collider.height = this.m_Height;
                this.m_Collider.center = this.m_Center;
            }
            return modification;
        }
        modification.modified = modification.original;
        modification.delta = new HeightModification.State();
        modification.applied = false;
        modification.scale = 1f;
        return modification;
    }

    public CollisionFlags Move(Vector3 motion)
    {
        CollisionFlags flags;
        CCDesc desc = s_CurrentMovingCCDesc;
        try
        {
            s_CurrentMovingCCDesc = this;
            if (!object.ReferenceEquals(this.AssignedHitManager, null))
            {
                this.AssignedHitManager.Clear();
            }
            flags = this.m_Collider.Move(motion);
        }
        finally
        {
            s_CurrentMovingCCDesc = (desc == null) ? null : desc;
        }
        return flags;
    }

    public Vector3 OffsetToWorld(Vector3 offset)
    {
        if ((offset.x != 0f) || (offset.z != 0f))
        {
            offset = (Vector3) (this.flatRotation * offset);
        }
        Vector3 lossyScale = base.transform.lossyScale;
        offset.x *= lossyScale.x;
        offset.y *= lossyScale.y;
        offset.z *= lossyScale.z;
        Vector3 position = base.transform.position;
        offset.x += position.x;
        offset.y += position.y;
        offset.z += position.z;
        return offset;
    }

    public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
    {
        return this.m_Collider.Raycast(ray, out hitInfo, distance);
    }

    public bool SimpleMove(Vector3 speed)
    {
        bool flag;
        CCDesc desc = s_CurrentMovingCCDesc;
        try
        {
            s_CurrentMovingCCDesc = this;
            flag = this.m_Collider.SimpleMove(speed);
        }
        finally
        {
            s_CurrentMovingCCDesc = (desc == null) ? null : desc;
        }
        return flag;
    }

    public Rigidbody attachedRigidbody
    {
        get
        {
            return this.m_Collider.attachedRigidbody;
        }
    }

    public Vector3 bottom
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float radius = this.m_Height * 0.5f;
            if (this.m_Radius > radius)
            {
                radius = this.m_Radius;
            }
            vector.y = this.m_Center.y - radius;
            return vector;
        }
    }

    public Bounds bounds
    {
        get
        {
            return this.m_Collider.bounds;
        }
    }

    public Vector3 center
    {
        get
        {
            return this.m_Center;
        }
    }

    public Vector3 centroidBottom
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float num = this.m_Height * 0.5f;
            if (this.m_Radius > num)
            {
                num = 0f;
            }
            else
            {
                num -= this.m_Radius;
            }
            vector.y = this.m_Center.y - num;
            return vector;
        }
    }

    public Vector3 centroidTop
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float num = this.m_Height * 0.5f;
            if (this.m_Radius > num)
            {
                num = 0f;
            }
            else
            {
                num -= this.m_Radius;
            }
            vector.y = this.m_Center.y + num;
            return vector;
        }
    }

    public CharacterController collider
    {
        get
        {
            return this.m_Collider;
        }
    }

    public CollisionFlags collisionFlags
    {
        get
        {
            return this.m_Collider.collisionFlags;
        }
    }

    public bool detectCollisions
    {
        get
        {
            return this.m_Collider.detectCollisions;
        }
        set
        {
            this.m_Collider.detectCollisions = value;
        }
    }

    public float diameter
    {
        get
        {
            return (this.m_Radius + this.m_Radius);
        }
    }

    public float effectiveHeight
    {
        get
        {
            float num = this.m_Radius + this.m_Radius;
            return ((num <= this.m_Height) ? this.m_Height : num);
        }
    }

    public float effectiveSkinnedHeight
    {
        get
        {
            float num = this.m_Radius + this.m_Radius;
            return (((num <= this.m_Height) ? this.m_Height : num) + (this.m_SkinWidth + this.m_SkinWidth));
        }
    }

    public bool enabled
    {
        get
        {
            return this.m_Collider.enabled;
        }
        set
        {
            this.m_Collider.enabled = value;
        }
    }

    public Quaternion flatRotation
    {
        get
        {
            Vector3 forward = base.transform.forward;
            forward.y = (forward.x * forward.x) + (forward.z * forward.z);
            if (Mathf.Approximately(forward.y, 0f))
            {
                Vector3 right = base.transform.right;
                forward.z = right.x;
                forward.x = -right.z;
                forward.y = (right.x * right.x) + (right.z * right.z);
            }
            if (forward.y != 1f)
            {
                forward.y = 1f / Mathf.Sqrt(forward.y);
            }
            forward.x *= forward.y;
            forward.z *= forward.z;
            forward.y = 0f;
            return Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    public float height
    {
        get
        {
            return this.m_Height;
        }
    }

    public bool isGrounded
    {
        get
        {
            return this.m_Collider.isGrounded;
        }
    }

    public bool isTrigger
    {
        get
        {
            return this.m_Collider.isTrigger;
        }
        set
        {
            this.m_Collider.isTrigger = value;
        }
    }

    public PhysicMaterial material
    {
        get
        {
            return this.m_Collider.material;
        }
        set
        {
            this.m_Collider.material = value;
        }
    }

    public float minMoveDistance
    {
        get
        {
            return this.m_MinMoveDistance;
        }
    }

    public static CCDesc Moving
    {
        get
        {
            return s_CurrentMovingCCDesc;
        }
    }

    public float radius
    {
        get
        {
            return this.m_Radius;
        }
    }

    public PhysicMaterial sharedMaterial
    {
        get
        {
            return this.m_Collider.sharedMaterial;
        }
        set
        {
            this.m_Collider.sharedMaterial = value;
        }
    }

    public Vector3 skinnedBottom
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float radius = this.m_Height * 0.5f;
            if (this.m_Radius > radius)
            {
                radius = this.m_Radius;
            }
            vector.y = this.m_Center.y - (radius + this.m_SkinWidth);
            return vector;
        }
    }

    public float skinnedDiameter
    {
        get
        {
            return (((this.m_Radius + this.m_Radius) + this.m_SkinWidth) + this.m_SkinWidth);
        }
    }

    public float skinnedHeight
    {
        get
        {
            return ((this.m_Height + this.m_SkinWidth) + this.m_SkinWidth);
        }
    }

    public float skinnedRadius
    {
        get
        {
            return (this.m_Radius + this.m_SkinWidth);
        }
    }

    public Vector3 skinnedTop
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float radius = this.m_Height * 0.5f;
            if (this.m_Radius > radius)
            {
                radius = this.m_Radius;
            }
            vector.y = (this.m_Center.y + radius) + this.m_SkinWidth;
            return vector;
        }
    }

    public float skinWidth
    {
        get
        {
            return this.m_SkinWidth;
        }
    }

    public float slopeLimit
    {
        get
        {
            return this.m_SlopeLimit;
        }
    }

    public float stepOffset
    {
        get
        {
            return this.m_StepOffset;
        }
    }

    public Vector3 top
    {
        get
        {
            Vector3 vector;
            vector.x = this.m_Center.x;
            vector.z = this.m_Center.z;
            float radius = this.m_Height * 0.5f;
            if (this.m_Radius > radius)
            {
                radius = this.m_Radius;
            }
            vector.y = this.m_Center.y + radius;
            return vector;
        }
    }

    public Vector3 velocity
    {
        get
        {
            return this.m_Collider.velocity;
        }
    }

    public Vector3 worldBottom
    {
        get
        {
            return this.OffsetToWorld(this.bottom);
        }
    }

    public Vector3 worldCenter
    {
        get
        {
            return this.OffsetToWorld(this.m_Center);
        }
    }

    public Vector3 worldCentroidBottom
    {
        get
        {
            return this.OffsetToWorld(this.centroidBottom);
        }
    }

    public Vector3 worldCentroidTop
    {
        get
        {
            return this.OffsetToWorld(this.centroidTop);
        }
    }

    public Vector3 worldSkinnedBottom
    {
        get
        {
            return this.OffsetToWorld(this.skinnedBottom);
        }
    }

    public Vector3 worldSkinnedTop
    {
        get
        {
            return this.OffsetToWorld(this.skinnedTop);
        }
    }

    public Vector3 worldTop
    {
        get
        {
            return this.OffsetToWorld(this.top);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HeightModification
    {
        public State original;
        public State modified;
        public State delta;
        public float scale;
        public bool differed;
        public bool applied;
        public float bottomDeltaHeight
        {
            get
            {
                return (this.modified.skinnedBottomY - this.original.skinnedBottomY);
            }
        }
        public float topDeltaHeight
        {
            get
            {
                return (this.modified.skinnedTopY - this.original.skinnedTopY);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct State
        {
            public float effectiveSkinnedHeight;
            public Vector3 center;
            public float skinnedBottomY
            {
                get
                {
                    return (this.center.y - (this.effectiveSkinnedHeight * 0.5f));
                }
            }
            public float skinnedTopY
            {
                get
                {
                    return (this.center.y + (this.effectiveSkinnedHeight * 0.5f));
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hit
    {
        public readonly UnityEngine.CharacterController CharacterController;
        public readonly CCDesc CCDesc;
        public readonly UnityEngine.Collider Collider;
        public readonly Vector3 Point;
        public readonly Vector3 Normal;
        public readonly Vector3 MoveDirection;
        public readonly float MoveLength;
        public Hit(ControllerColliderHit ControllerColliderHit)
        {
            this.CharacterController = ControllerColliderHit.controller;
            CCDesc desc = CCDesc.s_CurrentMovingCCDesc;
            if ((desc == null) || (desc.collider != this.CharacterController))
            {
                this.CCDesc = this.CharacterController.GetComponent<CCDesc>();
            }
            else
            {
                this.CCDesc = desc;
            }
            this.Collider = ControllerColliderHit.collider;
            this.Point = ControllerColliderHit.point;
            this.Normal = ControllerColliderHit.normal;
            this.MoveDirection = ControllerColliderHit.moveDirection;
            this.MoveLength = ControllerColliderHit.moveLength;
        }

        public UnityEngine.GameObject GameObject
        {
            get
            {
                return ((this.Collider == null) ? null : this.Collider.transform.gameObject);
            }
        }
        public UnityEngine.Transform Transform
        {
            get
            {
                return ((this.Collider == null) ? null : this.Collider.transform);
            }
        }
        public UnityEngine.Rigidbody Rigidbody
        {
            get
            {
                return ((this.Collider == null) ? null : this.Collider.attachedRigidbody);
            }
        }
    }

    public delegate bool HitFilter(CCDesc.HitManager hitManager, ref CCDesc.Hit hit);

    public class HitManager : IDisposable
    {
        private CCDesc.Hit[] buffer;
        private int bufferSize;
        private int filledCount;
        private bool issuingEvent;
        public object Tag;

        public event CCDesc.HitFilter OnHit;

        public HitManager() : this(8)
        {
        }

        public HitManager(int bufferSize)
        {
            this.bufferSize = bufferSize;
            this.buffer = new CCDesc.Hit[bufferSize];
            this.filledCount = 0;
        }

        public void Clear()
        {
            while (this.filledCount > 0)
            {
                CCDesc.Hit hit = new CCDesc.Hit();
                this.buffer[--this.filledCount] = hit;
            }
        }

        public void CopyTo(CCDesc.Hit[] array, int startIndex = 0)
        {
            for (int i = 0; i < this.filledCount; i++)
            {
                array[startIndex++] = this.buffer[i];
            }
        }

        public void Dispose()
        {
            this.buffer = null;
            this.OnHit = null;
        }

        public bool Push(ControllerColliderHit cchit)
        {
            if (this.issuingEvent)
            {
                Debug.LogError("Push during event call back");
                return false;
            }
            if (!object.ReferenceEquals(cchit, null))
            {
                CCDesc.Hit evnt = new CCDesc.Hit(cchit);
                return this.Push(ref evnt);
            }
            return false;
        }

        public bool Push(ref CCDesc.Hit evnt)
        {
            if (this.issuingEvent)
            {
                Debug.LogError("Push during event call back");
                return false;
            }
            CCDesc.HitFilter onHit = this.OnHit;
            if (onHit != null)
            {
                bool flag = false;
                try
                {
                    this.issuingEvent = true;
                    flag = !onHit(this, ref evnt);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
                finally
                {
                    this.issuingEvent = false;
                }
                if (flag)
                {
                    return false;
                }
            }
            int index = this.filledCount++;
            if (this.filledCount > this.bufferSize)
            {
                do
                {
                    this.bufferSize += 8;
                }
                while (this.filledCount > this.bufferSize);
                if (this.filledCount > 1)
                {
                    CCDesc.Hit[] buffer = this.buffer;
                    this.buffer = new CCDesc.Hit[this.bufferSize];
                    Array.Copy(buffer, this.buffer, (int) (this.filledCount - 1));
                }
                else
                {
                    this.buffer = new CCDesc.Hit[this.bufferSize];
                }
            }
            this.buffer[index] = evnt;
            return true;
        }

        public CCDesc.Hit[] ToArray()
        {
            CCDesc.Hit[] array = new CCDesc.Hit[this.filledCount];
            this.CopyTo(array, 0);
            return array;
        }

        public int Count
        {
            get
            {
                return this.filledCount;
            }
        }

        public CCDesc.Hit this[int i]
        {
            get
            {
                if ((i < 0) || (i >= this.filledCount))
                {
                    throw new ArgumentOutOfRangeException("i");
                }
                return this.buffer[i];
            }
        }
    }
}

