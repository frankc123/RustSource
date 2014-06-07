using System;
using uLink;
using UnityEngine;

public abstract class BandageItem<T> : HeldItem<T> where T: BandageDataBlock
{
    private float _bandageStartTime;
    private float _lastBandageTime;
    private bool _lastFramePrimary;

    protected BandageItem(T db) : base(db)
    {
        this._bandageStartTime = -1f;
    }

    public virtual bool CanBandage()
    {
        HumanBodyTakeDamage component = base.inventory.gameObject.GetComponent<HumanBodyTakeDamage>();
        return ((component.IsBleeding() || ((component.healthLossFraction > 0f) && base.datablock.DoesGiveBlood())) && ((Time.time - this.lastBandageTime) > 1.5f));
    }

    public void CancelBandage()
    {
        RPOS.SetActionProgress(false, null, 0f);
        this.bandageStartTime = -1f;
    }

    public void FinishBandage()
    {
        this.bandageStartTime = -1f;
        RPOS.SetActionProgress(false, null, 0f);
        int numWant = 1;
        if (base.Consume(ref numWant))
        {
            base.inventory.RemoveItem(base.slot);
        }
        base.itemRepresentation.Action(3, RPCMode.Server);
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        if (sample.attack && this.CanBandage())
        {
            this.Primary(ref sample);
        }
        else
        {
            if (this.lastFramePrimary)
            {
                this.CancelBandage();
            }
            this.lastFramePrimary = false;
        }
    }

    public virtual void Primary(ref HumanController.InputSample sample)
    {
        this.lastFramePrimary = true;
        sample.crouch = true;
        sample.walk = 0f;
        sample.strafe = 0f;
        sample.jump = false;
        sample.sprint = false;
        if (this.bandageStartTime == -1f)
        {
            this.StartBandage();
        }
        float num = Time.time - this.bandageStartTime;
        float progress = Mathf.Clamp((float) (num / base.datablock.bandageDuration), (float) 0f, (float) 1f);
        string label = string.Empty;
        bool flag = base.datablock.DoesGiveBlood();
        bool flag2 = base.datablock.DoesBandage();
        if (flag2 && !flag)
        {
            label = "Bandaging...";
        }
        else if (flag2 && flag)
        {
            label = "Bandage + Transfusion...";
        }
        else if (!flag2 && flag)
        {
            label = "Transfusing...";
        }
        RPOS.SetActionProgress(true, label, progress);
        if (progress >= 1f)
        {
            this.FinishBandage();
        }
    }

    public void StartBandage()
    {
        this.bandageStartTime = Time.time;
    }

    public float bandageStartTime
    {
        get
        {
            return this._bandageStartTime;
        }
        set
        {
            this._bandageStartTime = value;
        }
    }

    public float lastBandageTime
    {
        get
        {
            return this._lastBandageTime;
        }
        set
        {
            this._lastBandageTime = value;
        }
    }

    public bool lastFramePrimary
    {
        get
        {
            return this._lastFramePrimary;
        }
        set
        {
            this._lastFramePrimary = value;
        }
    }
}

