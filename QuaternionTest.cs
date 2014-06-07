using Facepunch.Precision;
using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("Precision/Tests/Quaternion Test")]
public class QuaternionTest : MonoBehaviour
{
    private const float cellHeight = 30f;
    private const float cellWidth = 100f;
    private GUIContent[,] contents;
    private QuaternionG facep = QuaternionG.identity;
    private const string formatFloat = "0.######";
    private Quaternion lastUnity;
    public bool nonHomogenous;
    private bool nonHomogenousWas;
    public Vector3[] R;
    private Rect[,] rects;
    public bool revMul;
    private Quaternion unity = Quaternion.identity;

    private void Awake()
    {
        this.contents = new GUIContent[3, 4];
        this.rects = new Rect[3, 4];
        float top = 400f;
        for (int i = 0; i < 3; i++)
        {
            float left = 20f;
            for (int j = 0; j < 4; j++)
            {
                this.contents[i, j] = new GUIContent();
                this.rects[i, j] = new Rect(left, top, 100f, 30f);
                left += 102f;
            }
            top += 32f;
        }
        this.contents[2, 0].text = "Degrees:";
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (this.contents == null)
            {
                this.Awake();
            }
            if ((this.lastUnity != this.unity) || (this.nonHomogenous != this.nonHomogenousWas))
            {
                Vector3G vectorg;
                int num = 0;
                for (int j = 0; j < 4; j++)
                {
                    this.contents[num, j].text = this.unity[j].ToString("0.######");
                }
                num = 1;
                for (int k = 0; k < 4; k++)
                {
                    this.contents[num, k].text = this.facep[k].ToString("0.######");
                }
                num = 2;
                if (this.nonHomogenous)
                {
                    QuaternionG.ToEulerNonHomogenious(ref this.facep, out vectorg);
                }
                else
                {
                    QuaternionG.ToEuler(ref this.facep, out vectorg);
                }
                for (int m = 1; m < 4; m++)
                {
                    this.contents[num, m].text = vectorg[m - 1].ToString("0.######");
                }
                this.nonHomogenousWas = this.nonHomogenous;
                this.lastUnity = this.unity;
            }
            GUIStyle textField = GUI.skin.textField;
            for (int i = 0; i < 3; i++)
            {
                for (int n = 0; n < 4; n++)
                {
                    GUI.Label(this.rects[i, n], this.contents[i, n], textField);
                }
            }
        }
    }

    private void Update()
    {
        if ((this.R == null) || (this.R.Length == 0))
        {
            this.unity = Quaternion.identity;
            this.facep = QuaternionG.identity;
        }
        else if (this.revMul)
        {
            int index = this.R.Length - 1;
            this.unity = Quaternion.Euler(this.R[index]);
            Vector3G deg = new Vector3G(this.R[index]);
            QuaternionG.Euler(ref deg, out this.facep);
            index--;
            while (index >= 0)
            {
                QuaternionG ng;
                this.unity = Quaternion.Euler(this.R[index]) * this.unity;
                deg.f = this.R[index];
                QuaternionG.Euler(ref deg, out ng);
                QuaternionG.Mult(ref ng, ref this.facep, out this.facep);
                index--;
            }
        }
        else
        {
            int num2 = 0;
            this.unity = Quaternion.Euler(this.R[num2]);
            Vector3G vectorg2 = new Vector3G(this.R[num2]);
            QuaternionG.Euler(ref vectorg2, out this.facep);
            num2++;
            while (num2 < this.R.Length)
            {
                QuaternionG ng2;
                this.unity *= Quaternion.Euler(this.R[num2]);
                vectorg2.f = this.R[num2];
                QuaternionG.Euler(ref vectorg2, out ng2);
                QuaternionG.Mult(ref this.facep, ref ng2, out this.facep);
                num2++;
            }
        }
    }
}

