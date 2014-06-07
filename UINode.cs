using System;
using UnityEngine;

public class UINode
{
    public int changeFlag = -1;
    public bool lastActive;
    public Vector3 lastPos;
    public Quaternion lastRot;
    public Vector3 lastScale;
    private GameObject mGo;
    private int mVisibleFlag = -1;
    public Transform trans;
    public UIWidget widget;

    public UINode(Transform t)
    {
        this.trans = t;
        this.lastPos = this.trans.localPosition;
        this.lastRot = this.trans.localRotation;
        this.lastScale = this.trans.localScale;
        this.mGo = t.gameObject;
    }

    public bool HasChanged()
    {
        bool flag = this.mGo.activeInHierarchy && ((this.widget == null) || (this.widget.enabled && (this.widget.color.a > 0.001f)));
        if ((this.lastActive == flag) && (!flag || ((!(this.lastPos != this.trans.localPosition) && !(this.lastRot != this.trans.localRotation)) && !(this.lastScale != this.trans.localScale))))
        {
            return false;
        }
        this.lastActive = flag;
        this.lastPos = this.trans.localPosition;
        this.lastRot = this.trans.localRotation;
        this.lastScale = this.trans.localScale;
        return true;
    }

    public int visibleFlag
    {
        get
        {
            return ((this.widget == null) ? this.mVisibleFlag : this.widget.visibleFlag);
        }
        set
        {
            if (this.widget != null)
            {
                this.widget.visibleFlag = value;
            }
            else
            {
                this.mVisibleFlag = value;
            }
        }
    }
}

