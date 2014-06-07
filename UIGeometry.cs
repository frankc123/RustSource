using NGUI.Meshing;
using System;
using UnityEngine;

public class UIGeometry
{
    private Vector3 lastPivotOffset;
    private Matrix4x4 lastWidgetToPanel;
    [NonSerialized]
    public MeshBuffer meshBuffer = new MeshBuffer();
    private bool vertsTransformed;

    public void Apply(ref Matrix4x4 widgetToPanel)
    {
        if (!this.vertsTransformed)
        {
            Debug.LogWarning("This overload of apply suggests you have called the other overload once before");
        }
        this.Apply(ref this.lastPivotOffset, ref widgetToPanel);
    }

    public void Apply(ref Vector3 pivotOffset, ref Matrix4x4 widgetToPanel)
    {
        if (this.vertsTransformed)
        {
            if (pivotOffset == this.lastPivotOffset)
            {
                if (widgetToPanel != this.lastWidgetToPanel)
                {
                    Matrix4x4 inverse = this.lastWidgetToPanel.inverse;
                    this.lastWidgetToPanel = widgetToPanel;
                    inverse = widgetToPanel * inverse;
                    this.meshBuffer.TransformVertices(inverse.m00, inverse.m10, inverse.m20, inverse.m01, inverse.m11, inverse.m21, inverse.m02, inverse.m12, inverse.m22, inverse.m03, inverse.m13, inverse.m23);
                }
            }
            else
            {
                Debug.LogWarning("Verts were transformed more than once");
                Matrix4x4 matrixx2 = this.lastWidgetToPanel.inverse;
                this.meshBuffer.TransformThenOffsetVertices(matrixx2.m00, matrixx2.m10, matrixx2.m20, matrixx2.m01, matrixx2.m11, matrixx2.m21, matrixx2.m02, matrixx2.m12, matrixx2.m22, matrixx2.m03, matrixx2.m13, matrixx2.m23, -this.lastPivotOffset.x, -this.lastPivotOffset.y, -this.lastPivotOffset.z);
                this.meshBuffer.OffsetThenTransformVertices(pivotOffset.x, pivotOffset.y, pivotOffset.z, widgetToPanel.m00, widgetToPanel.m10, widgetToPanel.m20, widgetToPanel.m01, widgetToPanel.m11, widgetToPanel.m21, widgetToPanel.m02, widgetToPanel.m12, widgetToPanel.m22, widgetToPanel.m03, widgetToPanel.m13, widgetToPanel.m23);
                this.lastWidgetToPanel = widgetToPanel;
                this.lastPivotOffset = pivotOffset;
            }
        }
        else
        {
            this.meshBuffer.OffsetThenTransformVertices(pivotOffset.x, pivotOffset.y, pivotOffset.z, widgetToPanel.m00, widgetToPanel.m10, widgetToPanel.m20, widgetToPanel.m01, widgetToPanel.m11, widgetToPanel.m21, widgetToPanel.m02, widgetToPanel.m12, widgetToPanel.m22, widgetToPanel.m03, widgetToPanel.m13, widgetToPanel.m23);
            this.lastWidgetToPanel = widgetToPanel;
            this.lastPivotOffset = pivotOffset;
            this.vertsTransformed = true;
        }
    }

    public void Clear()
    {
        this.meshBuffer.Clear();
        this.vertsTransformed = false;
    }

    public void WriteToBuffers(MeshBuffer m)
    {
        this.meshBuffer.WriteBuffers(m);
    }

    public bool hasTransformed
    {
        get
        {
            return (this.vertsTransformed || (this.meshBuffer.vSize == 0));
        }
    }

    public bool hasVertices
    {
        get
        {
            return (this.meshBuffer.vSize > 0);
        }
    }
}

