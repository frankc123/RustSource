using Facepunch.Precision;
using System;
using UnityEngine;

[AddComponentMenu("Precision/Tests/Projection Test"), ExecuteInEditMode]
public class ProjectionTest : MonoBehaviour
{
    public float aspect = -1f;
    private const float cellHeight = 30f;
    private const float cellWidth = 100f;
    private GUIContent[,,] contents;
    private Matrix4x4G facep = Matrix4x4G.identity;
    public float far = 1000f;
    public float fov = 60f;
    private Matrix4x4 lastUnity;
    public float near = 1f;
    private Rect[,,] rects;
    public bool revG;
    public bool revMul;
    private Matrix4x4 unity = Matrix4x4.identity;
    private Matrix4x4 unity2;

    private void Awake()
    {
        this.contents = new GUIContent[3, 4, 4];
        this.rects = new Rect[3, 4, 4];
        float top = 20f;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float left = 600f;
                for (int k = 0; k < 4; k++)
                {
                    this.contents[i, j, k] = new GUIContent();
                    this.rects[i, j, k] = new Rect(left, top, 100f, 30f);
                    left += 102f;
                }
                top += 32f;
            }
            top += 10f;
        }
    }

    private void DrawLabel(int m, int col, int row, GUIStyle style)
    {
        if (this.contents[m, col, row].text != this.contents[0, col, row].text)
        {
            GUI.contentColor = this.RCCol(col, row);
        }
        else
        {
            GUI.contentColor = Color.white;
        }
        GUI.Label(this.rects[m, col, row], this.contents[m, col, row], style);
    }

    private Matrix4x4G MultG(Matrix4x4G a, Matrix4x4G b)
    {
        Matrix4x4G matrixxg;
        if (this.revG)
        {
            Matrix4x4G.Mult(ref b, ref a, out matrixxg);
            return matrixxg;
        }
        Matrix4x4G.Mult(ref a, ref b, out matrixxg);
        return matrixxg;
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (this.contents == null)
            {
                this.Awake();
            }
            if (this.lastUnity != this.unity)
            {
                int num = 0;
                for (int j = 0; j < 4; j++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        this.contents[num, j, n].text = this.unity[j, n].ToString();
                    }
                }
                num = 1;
                for (int k = 0; k < 4; k++)
                {
                    for (int num5 = 0; num5 < 4; num5++)
                    {
                        this.contents[num, k, num5].text = this.facep[k, num5].ToString();
                    }
                }
                num = 2;
                for (int m = 0; m < 4; m++)
                {
                    for (int num7 = 0; num7 < 4; num7++)
                    {
                        this.contents[num, m, num7].text = this.unity2[m, num7].ToString();
                    }
                }
                this.lastUnity = this.unity;
            }
            GUIStyle textField = GUI.skin.textField;
            for (int i = 0; i < 3; i++)
            {
                for (int num9 = 0; num9 < 4; num9++)
                {
                    for (int num10 = 0; num10 < 4; num10++)
                    {
                        this.DrawLabel(i, num9, num10, textField);
                    }
                }
            }
        }
    }

    private Color RCCol(int col, int row)
    {
        Color red;
        switch ((row | ((col % 2) << 2)))
        {
            case 0:
                red = Color.red;
                break;

            case 1:
                red = Color.green;
                break;

            case 2:
                red = Color.blue;
                break;

            case 3:
                red = Color.magenta;
                break;

            case 4:
                red = Color.cyan;
                break;

            case 5:
                red = Color.yellow;
                break;

            case 6:
                red = Color.gray;
                break;

            case 7:
                red = Color.black;
                break;

            default:
                red = Color.clear;
                break;
        }
        if (col >= 2)
        {
            red.r += 0.25f;
            red.g += 0.25f;
            red.b += 0.25f;
        }
        return red;
    }

    private void Update()
    {
        double aspectRatio = (this.aspect <= 0f) ? (((double) Screen.height) / ((double) Screen.width)) : ((double) this.aspect);
        this.unity = Matrix4x4.Perspective(this.fov, (float) aspectRatio, this.near, this.far);
        double fov = this.fov;
        double near = this.near;
        double far = this.far;
        Matrix4x4G.Perspective(ref fov, ref aspectRatio, ref near, ref far, out this.facep);
        this.unity2 = this.facep.f;
    }

    public Matrix4x4G GMatrix
    {
        get
        {
            return this.facep;
        }
    }

    public Matrix4x4 UnityMatrix
    {
        get
        {
            return this.unity;
        }
    }

    public Matrix4x4 UnityMatrixCasted
    {
        get
        {
            return this.unity2;
        }
    }
}

