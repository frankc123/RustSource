using System;
using UnityEngine;

public class BulletWeaponImpact : WeaponImpact
{
    private Vector3 hitDirection;
    private Vector3 hitPoint;
    public readonly Transform hitTransform;

    public BulletWeaponImpact(BulletWeaponDataBlock dataBlock, IBulletWeaponItem item, ItemRepresentation itemRep, Vector3 worldHitPoint, Vector3 worldHitDirection) : this(dataBlock, item, itemRep, null, worldHitPoint, worldHitDirection)
    {
    }

    public BulletWeaponImpact(BulletWeaponDataBlock dataBlock, IBulletWeaponItem item, ItemRepresentation itemRep, Transform hitTransform, Vector3 localHitPoint, Vector3 localHitDirection) : base(dataBlock, item, itemRep)
    {
        this.hitTransform = hitTransform;
        this.hitPoint = localHitPoint;
        this.hitDirection = localHitDirection;
    }

    public BulletWeaponDataBlock dataBlock
    {
        get
        {
            return (BulletWeaponDataBlock) base.dataBlock;
        }
    }

    public IBulletWeaponItem item
    {
        get
        {
            return (base.item as IBulletWeaponItem);
        }
    }

    public Vector3 localDirection
    {
        get
        {
            return ((this.hitTransform == null) ? Vector3.forward : this.hitDirection);
        }
    }

    public Vector3 localPoint
    {
        get
        {
            return ((this.hitTransform == null) ? new Vector3() : this.hitPoint);
        }
    }

    public Vector3 worldDirection
    {
        get
        {
            return ((this.hitTransform == null) ? this.hitDirection : this.hitTransform.TransformDirection(this.hitDirection));
        }
    }

    public Vector3 worldPoint
    {
        get
        {
            return ((this.hitTransform == null) ? this.hitPoint : this.hitTransform.TransformPoint(this.hitPoint));
        }
    }
}

