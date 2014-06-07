using System;
using UnityEngine;

public class SimpleHitbox : BaseHitBox
{
    public float crouchHeight = 1f;
    public bool fixedUpdate;
    private CapsuleCollider myCap;
    private Transform myTransform;
    private Vector3 offset;
    private bool oldCrouch;
    private Transform parent;
    private Transform root;
    private Transform rootTransform;
    public float standingHeight = 1.85f;

    private void FixedUpdate()
    {
        if (this.fixedUpdate)
        {
            this.Snap();
        }
    }

    private void Snap()
    {
        if (base.idMain.stateFlags.crouch != this.oldCrouch)
        {
            if (base.idMain.stateFlags.crouch)
            {
                this.myCap.height = this.crouchHeight;
            }
            else
            {
                this.myCap.height = this.standingHeight;
            }
            this.oldCrouch = base.idMain.stateFlags.crouch;
        }
        Vector3 vector = this.parent.TransformPoint(this.offset);
        vector.y = this.rootTransform.position.y + (this.myCap.height * 0.5f);
        this.myTransform.position = vector;
    }

    private void Start()
    {
        this.myCap = base.collider as CapsuleCollider;
        this.parent = base.transform.parent;
        this.root = this.parent.root;
        this.offset = Vector3.zero;
        base.transform.parent = null;
        this.rootTransform = this.root.transform;
        this.myTransform = base.transform;
    }

    private void Update()
    {
        if (!this.fixedUpdate)
        {
            this.Snap();
        }
    }
}

