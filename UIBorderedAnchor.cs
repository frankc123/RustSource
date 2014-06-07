using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Anchor (Bordered)")]
public class UIBorderedAnchor : UIAnchor
{
    public RectOffset screenPixelBorder;

    protected void Update()
    {
        if (base.uiCamera != null)
        {
            Vector3 newPosition = UIAnchor.WorldOrigin(base.uiCamera, base.side, this.screenPixelBorder, base.depthOffset, this.relativeOffset.x, this.relativeOffset.y, base.halfPixelOffset);
            base.SetPosition(ref newPosition);
        }
    }
}

