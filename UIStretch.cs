using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Stretch"), ExecuteInEditMode]
public class UIStretch : MonoBehaviour
{
    private UIRoot mRoot;
    private Transform mTrans;
    public Vector2 relativeSize = Vector2.one;
    public Style style;
    public Camera uiCamera;

    private void OnEnable()
    {
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
    }

    private void Update()
    {
        if ((this.uiCamera != null) && (this.style != Style.None))
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            Rect pixelRect = this.uiCamera.pixelRect;
            float width = pixelRect.width;
            float height = pixelRect.height;
            if (((this.mRoot != null) && !this.mRoot.automatic) && (height > 1f))
            {
                float num3 = ((float) this.mRoot.manualHeight) / height;
                width *= num3;
                height *= num3;
            }
            Vector3 localScale = this.mTrans.localScale;
            if (this.style == Style.BasedOnHeight)
            {
                localScale.x = this.relativeSize.x * height;
                localScale.y = this.relativeSize.y * height;
            }
            else
            {
                if ((this.style == Style.Both) || (this.style == Style.Horizontal))
                {
                    localScale.x = this.relativeSize.x * width;
                }
                if ((this.style == Style.Both) || (this.style == Style.Vertical))
                {
                    localScale.y = this.relativeSize.y * height;
                }
            }
            if (this.mTrans.localScale != localScale)
            {
                this.mTrans.localScale = localScale;
            }
        }
    }

    public enum Style
    {
        None,
        Horizontal,
        Vertical,
        Both,
        BasedOnHeight
    }
}

