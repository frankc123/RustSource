using NGUI.Meshing;
using NGUI.Structures;
using System;
using UnityEngine;

public class UISpriteWheel : UISlicedSprite
{
    [SerializeField, HideInInspector]
    private float _addDegrees;
    [HideInInspector, SerializeField]
    private Vector2 _center;
    [SerializeField, HideInInspector]
    private float _degreesOfRotation = 360f;
    [HideInInspector, SerializeField]
    private float _facialRotationOffset;
    [HideInInspector, SerializeField]
    private float _innerRadius = 0.5f;
    [HideInInspector, SerializeField]
    private float _sliceDegrees;
    [HideInInspector, SerializeField]
    private float _sliceFill = 1f;
    [HideInInspector, SerializeField]
    private int _slices;
    [SerializeField, HideInInspector]
    private float _targetDegreeResolution = 10f;

    public override void OnFill(MeshBuffer m)
    {
        NineRectangle rectangle;
        NineRectangle rectangle2;
        NineRectangle rectangle3;
        NineRectangle rectangle4;
        Vector4 vector;
        Vector4 vector2;
        float num = this._degreesOfRotation * 0.01745329f;
        float num2 = this._sliceDegrees * 0.01745329f;
        float num3 = this._sliceFill;
        int num4 = this.slices + 1;
        float num5 = (num - (num2 * this.slices)) * num3;
        float num6 = num5 / ((float) num4);
        float num7 = num5 / 6.283185f;
        float num8 = (num - num5) / ((float) num4);
        float3 num9 = new float3 {
            xyz = base.cachedTransform.localScale
        };
        float num10 = (num9.x >= num9.y) ? num9.x : num9.y;
        num9.xy.x = ((3.141593f * num10) / ((float) num4)) * num7;
        num9.xy.y = num10 * (this.outerRadius * 0.5f);
        vector.x = this.mOuterUV.xMin;
        vector.y = this.mInnerUV.xMin;
        vector.z = this.mInnerUV.xMax;
        vector.w = this.mOuterUV.xMax;
        vector2.x = this.mOuterUV.yMin;
        vector2.y = this.mInnerUV.yMin;
        vector2.z = this.mInnerUV.yMax;
        vector2.w = this.mOuterUV.yMax;
        NineRectangle.Calculate(UIWidget.Pivot.Center, base.atlas.pixelSize, base.mainTexture, ref vector, ref vector2, ref num9.xy, out rectangle, out rectangle3);
        if ((this.innerRadius > 0f) && !Mathf.Approximately(rectangle.zz.x - rectangle.yy.x, 0f))
        {
            num9.xy.x = (((3.141593f * num10) * this.innerRadius) / ((float) num4)) * num7;
            NineRectangle.Calculate(UIWidget.Pivot.Center, base.atlas.pixelSize, base.mainTexture, ref vector, ref vector2, ref num9.xy, out rectangle2, out rectangle4);
            float num11 = (rectangle.yy.x + rectangle.zz.x) * 0.5f;
            if (rectangle2.yy.x > num11)
            {
                float num12 = (rectangle2.yy.x - num11) / (rectangle.ww.x - num11);
                if (num12 >= 1f)
                {
                    rectangle2.xx.x = rectangle.xx.x;
                    rectangle2.xx.y = rectangle.xx.y;
                    rectangle2.yy.x = rectangle.yy.x;
                    rectangle2.yy.y = rectangle.yy.y;
                    rectangle2.zz.x = rectangle.zz.x;
                    rectangle2.zz.y = rectangle.zz.y;
                    rectangle2.ww.x = rectangle.ww.x;
                    rectangle2.ww.y = rectangle.ww.y;
                    rectangle4.xx.x = rectangle3.xx.x;
                    rectangle4.xx.y = rectangle3.xx.y;
                    rectangle4.yy.x = rectangle3.yy.x;
                    rectangle4.yy.y = rectangle3.yy.y;
                    rectangle4.zz.x = rectangle3.zz.x;
                    rectangle4.zz.y = rectangle3.zz.y;
                    rectangle4.ww.x = rectangle3.ww.x;
                    rectangle4.ww.y = rectangle3.ww.y;
                }
                else
                {
                    float num13 = 1f - num12;
                    rectangle2.xx.y = (rectangle.xx.y * num12) + (rectangle2.xx.y * num13);
                    rectangle2.yy.x = (rectangle.yy.x * num12) + (0.5f * num13);
                    rectangle2.yy.y = (rectangle.yy.y * num12) + (rectangle2.yy.y * num13);
                    rectangle2.zz.x = (rectangle.zz.x * num12) + (0.5f * num13);
                    rectangle2.zz.y = (rectangle.zz.y * num12) + (rectangle2.zz.y * num13);
                    rectangle2.ww.y = (rectangle.ww.y * num12) + (rectangle2.ww.y * num13);
                    rectangle2.ww.x = rectangle.ww.x;
                    rectangle2.xx.x = rectangle.xx.x;
                }
            }
        }
        else
        {
            rectangle2.xx.x = rectangle.xx.x;
            rectangle2.xx.y = rectangle.xx.y;
            rectangle2.yy.x = rectangle.yy.x;
            rectangle2.yy.y = rectangle.yy.y;
            rectangle2.zz.x = rectangle.zz.x;
            rectangle2.zz.y = rectangle.zz.y;
            rectangle2.ww.x = rectangle.ww.x;
            rectangle2.ww.y = rectangle.ww.y;
            rectangle4.xx.x = rectangle3.xx.x;
            rectangle4.xx.y = rectangle3.xx.y;
            rectangle4.yy.x = rectangle3.yy.x;
            rectangle4.yy.y = rectangle3.yy.y;
            rectangle4.zz.x = rectangle3.zz.x;
            rectangle4.zz.y = rectangle3.zz.y;
            rectangle4.ww.x = rectangle3.ww.x;
            rectangle4.ww.y = rectangle3.ww.y;
        }
        float num14 = Mathf.Abs((float) (rectangle.ww.x - rectangle.xx.x));
        float num15 = num6 / num14;
        if (num2 > 0f)
        {
            num14 += num2 / num15;
            num15 = num6 / num14;
        }
        float num16 = this.innerRadius * 0.5f;
        float num17 = this.outerRadius * 0.5f;
        float num18 = Mathf.Min(rectangle.xx.y, rectangle.ww.y);
        float num19 = Mathf.Max(rectangle.ww.y, rectangle.xx.y) - num18;
        Color color = base.color;
        int vSize = m.vSize;
        float num21 = num8 + num6;
        float num22 = ((num8 * -0.5f) + (((this._facialRotationOffset * 0.5f) + 0.5f) * num6)) + (this._addDegrees * 0.01745329f);
        while (true)
        {
            Vertex[] v = m.v;
            int num23 = m.vSize;
            for (int i = vSize; i < num23; i++)
            {
                float num25 = num16 + (((v[i].y - num18) / num19) * num17);
                float f = (v[i].x * num15) + num22;
                v[i].x = 0.5f + (Mathf.Sin(f) * num25);
                v[i].y = -0.5f + (Mathf.Cos(f) * num25);
            }
            if (--num4 <= 0)
            {
                return;
            }
            num22 += num21;
            vSize = num23;
        }
    }

    public float additionalRotation
    {
        get
        {
            return this._addDegrees;
        }
        set
        {
            if (!float.IsInfinity(value) && !float.IsNaN(value))
            {
                while (value > 180f)
                {
                    value -= 360f;
                }
                while (value <= -180f)
                {
                    value += 360f;
                }
                if (value != this._addDegrees)
                {
                    this._addDegrees = value;
                    this.MarkAsChanged();
                }
            }
        }
    }

    public Vector2 center
    {
        get
        {
            return this._center;
        }
        set
        {
            if (this._center != value)
            {
                this._center = value;
                this.MarkAsChanged();
            }
        }
    }

    public float circumferenceFillRatio
    {
        get
        {
            return this._sliceFill;
        }
        set
        {
            if (value < 0.05f)
            {
                value = 0.05f;
            }
            else if (value > 1f)
            {
                value = 1f;
            }
            if (this._sliceFill != value)
            {
                this._sliceFill = value;
                this.MarkAsChanged();
            }
        }
    }

    public float degreesOfRotation
    {
        get
        {
            return this._degreesOfRotation;
        }
        set
        {
            if (value < 0.01f)
            {
                value = 0.01f;
            }
            else if (value > 360f)
            {
                value = 360f;
            }
            if (value != this._degreesOfRotation)
            {
                this._degreesOfRotation = value;
                this.MarkAsChanged();
            }
        }
    }

    public float facialCrank
    {
        get
        {
            return this._facialRotationOffset;
        }
        set
        {
            if (value < -1f)
            {
                value = -1f;
            }
            else if (value > 1f)
            {
                value = 1f;
            }
            if (value != this._facialRotationOffset)
            {
                this._facialRotationOffset = value;
                this.MarkAsChanged();
            }
        }
    }

    public float innerRadius
    {
        get
        {
            return this._innerRadius;
        }
        set
        {
            if (value < 0f)
            {
                if (this._innerRadius != 0f)
                {
                    this._innerRadius = 0f;
                    this.MarkAsChanged();
                }
            }
            else if (value > 1f)
            {
                if (this._innerRadius != 1f)
                {
                    this._innerRadius = 1f;
                    this.MarkAsChanged();
                }
            }
            else if (this._innerRadius != value)
            {
                this._innerRadius = value;
                this.MarkAsChanged();
            }
        }
    }

    public float outerRadius
    {
        get
        {
            return (1f - this._innerRadius);
        }
        set
        {
            this.innerRadius = 1f - value;
        }
    }

    public float sliceDegrees
    {
        get
        {
            return this._sliceDegrees;
        }
        set
        {
            if (value < 0f)
            {
                value = 0f;
            }
            if (this._sliceDegrees != value)
            {
                this._sliceDegrees = value;
                this.MarkAsChanged();
            }
        }
    }

    public int slices
    {
        get
        {
            return this._slices;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 360)
            {
                value = 360;
            }
            if (this._slices != value)
            {
                this._slices = value;
                this.MarkAsChanged();
            }
        }
    }

    public float targetDegreeResolution
    {
        get
        {
            return this._targetDegreeResolution;
        }
        set
        {
            if (0.5f > value)
            {
                value = 0.5f;
            }
            if (this._targetDegreeResolution != value)
            {
                this._targetDegreeResolution = value;
                this.MarkAsChanged();
            }
        }
    }
}

