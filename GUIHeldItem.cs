using System;
using UnityEngine;

public class GUIHeldItem : MonoBehaviour
{
    private static GUIHeldItem _guiHeldItem;
    public UITexture _icon;
    private IInventoryItem _itemHolding;
    private UIMaterial _myMaterial;
    private float fadeOutAlpha;
    private Vector3 fadeOutPoint;
    private float fadeOutPointDistance;
    private Vector3 fadeOutPointEnd;
    private float fadeOutPointMagnitude;
    private Vector3 fadeOutPointNormal;
    private Vector3 fadeOutPointStart;
    private Vector3 fadeOutVelocity;
    private bool fadingOut;
    private bool hasItem;
    private const float kFadeSmoothTime = 0.1f;
    private const float kFadeSpeed = 50f;
    private const float kOffsetSmoothTime = 0.06f;
    private const float kOffsetSpeed = 600f;
    private float lastTime;
    private Transform mTrans;
    private Vector3 offsetPoint;
    private Vector3 offsetVelocity;
    private Plane planeTest;
    private bool started;
    private Color startingIconColor = Color.white;
    public Camera uiCamera;

    public void ClearHeldItem()
    {
        if (this.hasItem)
        {
            this.SetHeldItem((IInventoryItem) null);
            if (!this.fadingOut)
            {
                this.Opaque();
            }
        }
    }

    public void ClearHeldItem(RPOSInventoryCell fadeToCell)
    {
        if (this.hasItem)
        {
            this.fadingOut = true;
            this.ClearHeldItem();
            try
            {
                Vector3 vector;
                if (NGUITools.GetCentroid(fadeToCell, out vector))
                {
                    this.FadeOutToPoint(vector);
                }
            }
            catch
            {
            }
            return;
            this.Opaque();
        }
    }

    public static IInventoryItem CurrentItem()
    {
        return Get()._itemHolding;
    }

    public void FadeOutToPoint(Vector3 worldPoint)
    {
        this.Opaque();
        this.fadeOutPointStart = this.mTrans.position;
        this.fadeOutPointEnd = new Vector3(worldPoint.x, worldPoint.y, worldPoint.z);
        if (this.fadeOutPointStart == this.fadeOutPointEnd)
        {
            this.fadeOutPointEnd.z++;
        }
        this.fadeOutPointNormal = this.fadeOutPointEnd - this.fadeOutPointStart;
        this.fadeOutPointMagnitude = this.fadeOutPointNormal.magnitude;
        this.fadeOutPointNormal = (Vector3) (this.fadeOutPointNormal / this.fadeOutPointMagnitude);
        this.fadeOutPointDistance = Vector3.Dot(this.fadeOutPointNormal, this.fadeOutPointStart);
        this.fadeOutAlpha = 1f;
        this.fadingOut = true;
        this._icon.enabled = true;
        this.fadeOutPoint = this.fadeOutPointStart;
    }

    public static GUIHeldItem Get()
    {
        return _guiHeldItem;
    }

    private void MakeEmpty()
    {
        if (this._icon != null)
        {
            this._icon.enabled = false;
        }
        this._itemHolding = null;
        this.hasItem = false;
    }

    private void OnDestroy()
    {
        Object.Destroy(this._myMaterial);
    }

    private void Opaque()
    {
        this.fadeOutAlpha = 1f;
        this.fadeOutPointStart = Vector3.zero;
        this.fadeOutPointEnd = Vector3.right;
        this.fadeOutPointDistance = 1f;
        this.fadeOutPointMagnitude = 1f;
        this.fadeOutPointNormal = Vector3.right;
        this.fadeOutVelocity = Vector3.zero;
        this.fadingOut = false;
        if (this.started)
        {
            this._icon.color = this.startingIconColor;
        }
    }

    public bool SetHeldItem(IInventoryItem item)
    {
        if (item == null)
        {
            this.MakeEmpty();
            if (!this.fadingOut)
            {
                this.Opaque();
            }
            return false;
        }
        this.hasItem = true;
        Texture iconTex = item.datablock.iconTex;
        ItemDataBlock.LoadIconOrUnknown<Texture>(item.datablock.icon, ref iconTex);
        this._icon.enabled = true;
        this._myMaterial.Set("_MainTex", iconTex);
        this._itemHolding = item;
        this.offsetVelocity = this.offsetPoint = (Vector3) new Vector2();
        this.Opaque();
        return true;
    }

    public bool SetHeldItem(RPOSInventoryCell cell)
    {
        if (!this.SetHeldItem((cell == null) ? null : cell.slotItem))
        {
            return false;
        }
        try
        {
            Vector3 vector;
            if (NGUITools.GetCentroid(cell, out vector))
            {
                Vector2 vector2 = UICamera.FindCameraForLayer(cell.gameObject.layer).cachedCamera.WorldToScreenPoint(vector);
                this.offsetPoint = (Vector3) (vector2 - UICamera.lastMousePosition);
            }
        }
        catch
        {
            this.offsetPoint = Vector3.zero;
        }
        return true;
    }

    private void SetPosition(Vector3 world)
    {
        Vector3 vector = this.mTrans.localPosition + this.mTrans.InverseTransformPoint(world);
        vector.z = -190f;
        this.mTrans.localPosition = vector;
    }

    private void Start()
    {
        this.startingIconColor = this._icon.color;
        this._icon.enabled = false;
        _guiHeldItem = this;
        this._myMaterial = this._icon.material.Clone();
        this._icon.material = this._myMaterial;
        this.mTrans = base.transform;
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.planeTest = new Plane((Vector3) (this.uiCamera.transform.forward * 1f), new Vector3(0f, 0f, 2f));
        this.started = true;
    }

    private void Update()
    {
        if (this.hasItem)
        {
            Vector3 position = ((Vector3) UICamera.lastMousePosition) + this.offsetPoint;
            Ray ray = this.uiCamera.ScreenPointToRay(position);
            float enter = 0f;
            if (this.planeTest.Raycast(ray, out enter))
            {
                this.SetPosition(ray.GetPoint(enter));
            }
            this.offsetPoint = Vector3.SmoothDamp(this.offsetPoint, Vector3.zero, ref this.offsetVelocity, 0.06f, 600f);
        }
        else if (this.fadingOut)
        {
            this.fadeOutPoint = Vector3.SmoothDamp(this.fadeOutPoint, this.fadeOutPointEnd, ref this.fadeOutVelocity, 0.1f, 50f);
            this.fadeOutAlpha = this.startingIconColor.a * (1f - Mathf.Clamp01(Mathf.Abs((float) (Vector3.Dot(this.fadeOutPointNormal, this.fadeOutPoint) - this.fadeOutPointDistance)) / this.fadeOutPointMagnitude));
            if (this.fadeOutAlpha <= 0.00390625)
            {
                this.fadingOut = false;
                this.MakeEmpty();
            }
            else
            {
                Color color = this._icon.color;
                this.SetPosition(this.fadeOutPoint);
                color.a = this.fadeOutAlpha;
                this._icon.color = color;
            }
        }
    }
}

