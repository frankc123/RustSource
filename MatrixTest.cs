using Facepunch.Precision;
using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("Precision/Tests/Matrix Test")]
public class MatrixTest : MonoBehaviour
{
    private const float cellHeight = 30f;
    private const float cellWidth = 100f;
    private GUIContent[,,] contents;
    private Matrix4x4G facep = Matrix4x4G.identity;
    private const string formatFloat = "0.#####";
    private ProjectionTest lastProjectionTest;
    private Matrix4x4 lastUnity;
    public ProjectionTest projection;
    private Rect[,,] rects;
    public bool revG;
    public bool revMul;
    public TRS[] transforms;
    private Matrix4x4 unity = Matrix4x4.identity;

    private void Awake()
    {
        this.contents = new GUIContent[2, 4, 4];
        this.rects = new Rect[2, 4, 4];
        float top = 20f;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float left = 20f;
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
        if (this.contents[m, col, row].text != this.contents[(m + 1) % 2, col, row].text)
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
            if ((this.lastUnity != this.unity) || (this.projection != this.lastProjectionTest))
            {
                int num = 0;
                for (int j = 0; j < 4; j++)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        this.contents[num, j, m].text = this.unity[j, m].ToString("0.#####");
                    }
                }
                num = 1;
                for (int k = 0; k < 4; k++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        this.contents[num, k, n].text = this.facep[k, n].ToString("0.#####");
                    }
                }
                this.lastProjectionTest = this.projection;
                this.lastUnity = this.unity;
            }
            GUIStyle textField = GUI.skin.textField;
            for (int i = 0; i < 2; i++)
            {
                for (int num7 = 0; num7 < 4; num7++)
                {
                    for (int num8 = 0; num8 < 4; num8++)
                    {
                        this.DrawLabel(i, num7, num8, textField);
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
        Matrix4x4 unity;
        Matrix4x4G facep;
        if ((this.transforms != null) && (this.transforms.Length > 0))
        {
            if (this.revMul)
            {
                int index = this.transforms.Length - 1;
                unity = this.transforms[index].unity;
                facep = this.transforms[index].facep;
                index--;
                while (index >= 0)
                {
                    unity = this.transforms[index].unity * unity;
                    facep = this.MultG(this.transforms[index].facep, facep);
                    index--;
                }
            }
            else
            {
                int num2 = 0;
                unity = this.transforms[num2].unity;
                facep = this.transforms[num2].facep;
                num2++;
                while (num2 < this.transforms.Length)
                {
                    unity *= this.transforms[num2].unity;
                    facep = this.MultG(facep, this.transforms[num2].facep);
                    num2++;
                }
            }
        }
        else
        {
            unity = Matrix4x4.identity;
            facep = Matrix4x4G.identity;
        }
        if (this.projection != null)
        {
            unity = this.projection.UnityMatrix * unity;
            facep = this.MultG(this.projection.GMatrix, facep);
        }
        this.unity = unity;
        this.facep = facep;
    }

    [Serializable]
    public class TRS
    {
        public Vector3 eulerR;
        public Vector3 S = Vector3.one;
        public Vector3 T;

        public Matrix4x4G facep
        {
            get
            {
                Matrix4x4G matrixxg;
                Vector3G translation = new Vector3G(this.T);
                QuaternionG rotation = this.R_facep;
                Vector3G scale = new Vector3G(this.S);
                Matrix4x4G.TRS(ref translation, ref rotation, ref scale, out matrixxg);
                return matrixxg;
            }
        }

        public QuaternionG R_facep
        {
            get
            {
                QuaternionG ng;
                Vector3G deg = new Vector3G(this.eulerR);
                QuaternionG.Euler(ref deg, out ng);
                return ng;
            }
        }

        public Quaternion R_unity
        {
            get
            {
                return Quaternion.Euler(this.eulerR);
            }
        }

        public Matrix4x4 unity
        {
            get
            {
                return Matrix4x4.TRS(this.T, this.R_unity, this.S);
            }
        }
    }
}

