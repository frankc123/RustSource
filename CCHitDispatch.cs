using System;
using UnityEngine;

[RequireComponent(typeof(CCDesc))]
public sealed class CCHitDispatch : MonoBehaviour
{
    [NonSerialized]
    private CCDesc ccdesc;
    [NonSerialized]
    private bool didSetup;
    [NonSerialized]
    private CCDesc.HitManager hitManager;

    public event CCDesc.HitFilter OnHit
    {
        add
        {
            CCDesc.HitManager hits = this.Hits;
            if (!object.ReferenceEquals(hits, null))
            {
                hits.OnHit += value;
            }
        }
        remove
        {
            CCDesc.HitManager hits = this.Hits;
            if (!object.ReferenceEquals(hits, null))
            {
                hits.OnHit -= value;
            }
        }
    }

    private void DoSetup()
    {
        if (!this.didSetup && Application.isPlaying)
        {
            this.didSetup = true;
            (this.ccdesc = base.GetComponent<CCDesc>()).AssignedHitManager = this.hitManager = new CCDesc.HitManager();
        }
    }

    public static CCHitDispatch GetHitDispatch(CCDesc CCDesc)
    {
        if (CCDesc == null)
        {
            return null;
        }
        if (!object.ReferenceEquals(CCDesc.AssignedHitManager, null))
        {
            return CCDesc.GetComponent<CCHitDispatch>();
        }
        CCHitDispatch component = CCDesc.GetComponent<CCHitDispatch>();
        if (component != null)
        {
            return component;
        }
        return CCDesc.gameObject.AddComponent<CCHitDispatch>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CCDesc.HitManager hits = this.Hits;
        if (!object.ReferenceEquals(hits, null))
        {
            hits.Push(hit);
        }
    }

    private void OnDestroy()
    {
        if (this.didSetup && !object.ReferenceEquals(this.hitManager, null))
        {
            CCDesc.HitManager hitManager = this.hitManager;
            this.hitManager = null;
            if (this.ccdesc != null)
            {
                this.ccdesc.AssignedHitManager = null;
            }
            hitManager.Dispose();
        }
    }

    public CCDesc CCDesc
    {
        get
        {
            if (!Application.isPlaying)
            {
                return ((this.ccdesc == null) ? base.GetComponent<CCDesc>() : this.ccdesc);
            }
            if (!this.didSetup)
            {
                this.DoSetup();
            }
            return this.ccdesc;
        }
    }

    public CCDesc.HitManager Hits
    {
        get
        {
            if (!this.didSetup)
            {
                this.DoSetup();
            }
            return this.hitManager;
        }
    }
}

