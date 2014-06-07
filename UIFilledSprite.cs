using NGUI.Meshing;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite (Filled)"), ExecuteInEditMode]
public class UIFilledSprite : UISprite
{
    [HideInInspector, SerializeField]
    private float mFillAmount = 1f;
    [HideInInspector, SerializeField]
    private FillDirection mFillDirection = FillDirection.Radial360;
    [HideInInspector, SerializeField]
    private bool mInvert;

    private bool AdjustRadial(Vector2[] xy, Vector2[] uv, float fill, bool invert)
    {
        if (fill < 0.001f)
        {
            return false;
        }
        if (invert || (fill <= 0.999f))
        {
            float f = Mathf.Clamp01(fill);
            if (!invert)
            {
                f = 1f - f;
            }
            f *= 1.570796f;
            float t = Mathf.Sin(f);
            float num3 = Mathf.Cos(f);
            if (t > num3)
            {
                num3 *= 1f / t;
                t = 1f;
                if (!invert)
                {
                    xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
                    xy[3].y = xy[0].y;
                    uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
                    uv[3].y = uv[0].y;
                }
            }
            else if (num3 > t)
            {
                t *= 1f / num3;
                num3 = 1f;
                if (invert)
                {
                    xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, t);
                    xy[1].x = xy[0].x;
                    uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, t);
                    uv[1].x = uv[0].x;
                }
            }
            else
            {
                t = 1f;
                num3 = 1f;
            }
            if (invert)
            {
                xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
                uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
            }
            else
            {
                xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, t);
                uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, t);
            }
        }
        return true;
    }

    public override void OnFill(MeshBuffer m)
    {
        Vertex vertex;
        Vertex vertex2;
        Vertex vertex3;
        Vertex vertex4;
        float x = 0f;
        float y = 0f;
        float num3 = 1f;
        float num4 = -1f;
        float xMin = this.mOuterUV.xMin;
        float yMin = this.mOuterUV.yMin;
        float xMax = this.mOuterUV.xMax;
        float yMax = this.mOuterUV.yMax;
        if ((this.mFillDirection == FillDirection.Horizontal) || (this.mFillDirection == FillDirection.Vertical))
        {
            float num9 = (xMax - xMin) * this.mFillAmount;
            float num10 = (yMax - yMin) * this.mFillAmount;
            if (this.fillDirection == FillDirection.Horizontal)
            {
                if (this.mInvert)
                {
                    x = 1f - this.mFillAmount;
                    xMin = xMax - num9;
                }
                else
                {
                    num3 *= this.mFillAmount;
                    xMax = xMin + num9;
                }
            }
            else if (this.fillDirection == FillDirection.Vertical)
            {
                if (this.mInvert)
                {
                    num4 *= this.mFillAmount;
                    yMin = yMax - num10;
                }
                else
                {
                    y = -(1f - this.mFillAmount);
                    yMax = yMin + num10;
                }
            }
        }
        Vector2[] xy = new Vector2[4];
        Vector2[] uv = new Vector2[4];
        xy[0] = new Vector2(num3, y);
        xy[1] = new Vector2(num3, num4);
        xy[2] = new Vector2(x, num4);
        xy[3] = new Vector2(x, y);
        uv[0] = new Vector2(xMax, yMax);
        uv[1] = new Vector2(xMax, yMin);
        uv[2] = new Vector2(xMin, yMin);
        uv[3] = new Vector2(xMin, yMax);
        Color color = base.color;
        if (this.fillDirection == FillDirection.Radial90)
        {
            if (!this.AdjustRadial(xy, uv, this.mFillAmount, this.mInvert))
            {
                return;
            }
        }
        else
        {
            if (this.fillDirection == FillDirection.Radial180)
            {
                Vector2[] v = new Vector2[4];
                Vector2[] vectorArray4 = new Vector2[4];
                for (int i = 0; i < 2; i++)
                {
                    float num12;
                    float num13;
                    v[0] = new Vector2(0f, 0f);
                    v[1] = new Vector2(0f, 1f);
                    v[2] = new Vector2(1f, 1f);
                    v[3] = new Vector2(1f, 0f);
                    vectorArray4[0] = new Vector2(0f, 0f);
                    vectorArray4[1] = new Vector2(0f, 1f);
                    vectorArray4[2] = new Vector2(1f, 1f);
                    vectorArray4[3] = new Vector2(1f, 0f);
                    if (this.mInvert)
                    {
                        if (i > 0)
                        {
                            this.Rotate(v, i);
                            this.Rotate(vectorArray4, i);
                        }
                    }
                    else if (i < 1)
                    {
                        this.Rotate(v, 1 - i);
                        this.Rotate(vectorArray4, 1 - i);
                    }
                    if (i == 1)
                    {
                        num12 = !this.mInvert ? 1f : 0.5f;
                        num13 = !this.mInvert ? 0.5f : 1f;
                    }
                    else
                    {
                        num12 = !this.mInvert ? 0.5f : 1f;
                        num13 = !this.mInvert ? 1f : 0.5f;
                    }
                    v[1].y = Mathf.Lerp(num12, num13, v[1].y);
                    v[2].y = Mathf.Lerp(num12, num13, v[2].y);
                    vectorArray4[1].y = Mathf.Lerp(num12, num13, vectorArray4[1].y);
                    vectorArray4[2].y = Mathf.Lerp(num12, num13, vectorArray4[2].y);
                    float fill = (this.mFillAmount * 2f) - i;
                    bool flag = (i % 2) == 1;
                    if (this.AdjustRadial(v, vectorArray4, fill, !flag))
                    {
                        if (this.mInvert)
                        {
                            flag = !flag;
                        }
                        if (flag)
                        {
                            int index = m.Alloc(PrimitiveKind.Quad);
                            for (int j = 0; j < 4; j++)
                            {
                                m.v[index].x = Mathf.Lerp(xy[0].x, xy[2].x, v[j].x);
                                m.v[index].y = Mathf.Lerp(xy[0].y, xy[2].y, v[j].y);
                                m.v[index].z = 0f;
                                m.v[index].u = Mathf.Lerp(uv[0].x, uv[2].x, vectorArray4[j].x);
                                m.v[index].v = Mathf.Lerp(uv[0].y, uv[2].y, vectorArray4[j].y);
                                m.v[index].r = color.r;
                                m.v[index].g = color.g;
                                m.v[index].b = color.b;
                                m.v[index].a = color.a;
                                index++;
                            }
                        }
                        else
                        {
                            int num17 = m.Alloc(PrimitiveKind.Quad);
                            for (int k = 3; k > -1; k--)
                            {
                                m.v[num17].x = Mathf.Lerp(xy[0].x, xy[2].x, v[k].x);
                                m.v[num17].y = Mathf.Lerp(xy[0].y, xy[2].y, v[k].y);
                                m.v[num17].z = 0f;
                                m.v[num17].u = Mathf.Lerp(uv[0].x, uv[2].x, vectorArray4[k].x);
                                m.v[num17].v = Mathf.Lerp(uv[0].y, uv[2].y, vectorArray4[k].y);
                                m.v[num17].r = color.r;
                                m.v[num17].g = color.g;
                                m.v[num17].b = color.b;
                                m.v[num17].a = color.a;
                                num17++;
                            }
                        }
                    }
                }
                return;
            }
            if (this.fillDirection == FillDirection.Radial360)
            {
                float[] numArray = new float[] { 0.5f, 1f, 0f, 0.5f, 0.5f, 1f, 0.5f, 1f, 0f, 0.5f, 0.5f, 1f, 0f, 0.5f, 0f, 0.5f };
                Vector2[] vectorArray5 = new Vector2[4];
                Vector2[] vectorArray6 = new Vector2[4];
                for (int n = 0; n < 4; n++)
                {
                    vectorArray5[0] = new Vector2(0f, 0f);
                    vectorArray5[1] = new Vector2(0f, 1f);
                    vectorArray5[2] = new Vector2(1f, 1f);
                    vectorArray5[3] = new Vector2(1f, 0f);
                    vectorArray6[0] = new Vector2(0f, 0f);
                    vectorArray6[1] = new Vector2(0f, 1f);
                    vectorArray6[2] = new Vector2(1f, 1f);
                    vectorArray6[3] = new Vector2(1f, 0f);
                    if (this.mInvert)
                    {
                        if (n > 0)
                        {
                            this.Rotate(vectorArray5, n);
                            this.Rotate(vectorArray6, n);
                        }
                    }
                    else if (n < 3)
                    {
                        this.Rotate(vectorArray5, 3 - n);
                        this.Rotate(vectorArray6, 3 - n);
                    }
                    for (int num20 = 0; num20 < 4; num20++)
                    {
                        int num21 = !this.mInvert ? (n * 4) : ((3 - n) * 4);
                        float from = numArray[num21];
                        float to = numArray[num21 + 1];
                        float num24 = numArray[num21 + 2];
                        float num25 = numArray[num21 + 3];
                        vectorArray5[num20].x = Mathf.Lerp(from, to, vectorArray5[num20].x);
                        vectorArray5[num20].y = Mathf.Lerp(num24, num25, vectorArray5[num20].y);
                        vectorArray6[num20].x = Mathf.Lerp(from, to, vectorArray6[num20].x);
                        vectorArray6[num20].y = Mathf.Lerp(num24, num25, vectorArray6[num20].y);
                    }
                    float num26 = (this.mFillAmount * 4f) - n;
                    bool flag2 = (n % 2) == 1;
                    if (this.AdjustRadial(vectorArray5, vectorArray6, num26, !flag2))
                    {
                        if (this.mInvert)
                        {
                            flag2 = !flag2;
                        }
                        if (flag2)
                        {
                            int num27 = m.Alloc(PrimitiveKind.Quad);
                            for (int num28 = 0; num28 < 4; num28++)
                            {
                                m.v[num27].x = Mathf.Lerp(xy[0].x, xy[2].x, vectorArray5[num28].x);
                                m.v[num27].y = Mathf.Lerp(xy[0].y, xy[2].y, vectorArray5[num28].y);
                                m.v[num27].z = 0f;
                                m.v[num27].u = Mathf.Lerp(uv[0].x, uv[2].x, vectorArray6[num28].x);
                                m.v[num27].v = Mathf.Lerp(uv[0].y, uv[2].y, vectorArray6[num28].y);
                                m.v[num27].r = color.r;
                                m.v[num27].g = color.g;
                                m.v[num27].b = color.b;
                                m.v[num27].a = color.a;
                                num27++;
                            }
                        }
                        else
                        {
                            int num29 = m.Alloc(PrimitiveKind.Quad);
                            for (int num30 = 3; num30 > -1; num30--)
                            {
                                m.v[num29].x = Mathf.Lerp(xy[0].x, xy[2].x, vectorArray5[num30].x);
                                m.v[num29].y = Mathf.Lerp(xy[0].y, xy[2].y, vectorArray5[num30].y);
                                m.v[num29].z = 0f;
                                m.v[num29].u = Mathf.Lerp(uv[0].x, uv[2].x, vectorArray6[num30].x);
                                m.v[num29].v = Mathf.Lerp(uv[0].y, uv[2].y, vectorArray6[num30].y);
                                m.v[num29].r = color.r;
                                m.v[num29].g = color.g;
                                m.v[num29].b = color.b;
                                m.v[num29].a = color.a;
                                num29++;
                            }
                        }
                    }
                }
                return;
            }
        }
        vertex.x = xy[0].x;
        vertex.y = xy[0].y;
        vertex.u = uv[0].x;
        vertex.v = uv[0].y;
        vertex2.x = xy[1].x;
        vertex2.y = xy[1].y;
        vertex2.u = uv[1].x;
        vertex2.v = uv[1].y;
        vertex3.x = xy[2].x;
        vertex3.y = xy[2].y;
        vertex3.u = uv[2].x;
        vertex3.v = uv[2].y;
        vertex4.x = xy[3].x;
        vertex4.y = xy[3].y;
        vertex4.u = uv[3].x;
        vertex4.v = uv[3].y;
        vertex.z = vertex2.z = vertex3.z = vertex4.z = 0f;
        vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
        vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
        vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
        vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
        m.Quad(vertex, vertex2, vertex3, vertex4);
    }

    private void Rotate(Vector2[] v, int offset)
    {
        for (int i = 0; i < offset; i++)
        {
            Vector2 vector = new Vector2(v[3].x, v[3].y);
            v[3].x = v[2].y;
            v[3].y = v[2].x;
            v[2].x = v[1].y;
            v[2].y = v[1].x;
            v[1].x = v[0].y;
            v[1].y = v[0].x;
            v[0].x = vector.y;
            v[0].y = vector.x;
        }
    }

    public float fillAmount
    {
        get
        {
            return this.mFillAmount;
        }
        set
        {
            float num = Mathf.Clamp01(value);
            if (this.mFillAmount != num)
            {
                this.mFillAmount = num;
                base.ChangedAuto();
            }
        }
    }

    public FillDirection fillDirection
    {
        get
        {
            return this.mFillDirection;
        }
        set
        {
            if (this.mFillDirection != value)
            {
                this.mFillDirection = value;
                base.ChangedAuto();
            }
        }
    }

    public bool invert
    {
        get
        {
            return this.mInvert;
        }
        set
        {
            if (this.mInvert != value)
            {
                this.mInvert = value;
                base.ChangedAuto();
            }
        }
    }

    public enum FillDirection
    {
        Horizontal,
        Vertical,
        Radial90,
        Radial180,
        Radial360
    }
}

