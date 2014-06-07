using System;
using UnityEngine;

public class Angle2EncDecTest : MonoBehaviour
{
    private Angle2 a;
    private int contentIndex;
    public GUIContent[] contents;
    private Angle2? dec;
    public float rate = 360f;

    private void OnGUI()
    {
        if (!this.dec.HasValue)
        {
            this.dec = new Angle2?(this.a.decoded);
            object[] objArray1 = new object[] { "Enc:\t", this.a.x, "\tDec:\t", this.dec.Value.x, "\tRED:\t", this.dec.Value.decoded.x };
            this.contents[this.contentIndex++].text = string.Concat(objArray1);
            this.contentIndex = this.contentIndex % this.contents.Length;
        }
        foreach (GUIContent content in this.contents)
        {
            GUILayout.Label(content, new GUILayoutOption[0]);
        }
    }

    private void Update()
    {
        float num = Time.deltaTime * this.rate;
        if (num != 0f)
        {
            this.a.x += num;
            while (this.a.x > 360f)
            {
                this.a.x -= 360f;
            }
            while (this.a.x < 0f)
            {
                this.a.x += 360f;
            }
            this.dec = null;
        }
    }
}

