using System;
using System.Collections.Generic;
using UnityEngine;

public class LightStyleTest : MonoBehaviour
{
    public float enterCrossfadeLength = 0.3f;
    private int index;
    public float spacebarFadeLength = 1.3f;
    public float spacebarTargetWeight = 1f;
    public LightStylist stylist;
    public string[] tests;
    private List<float> weights;

    private void OnGUI()
    {
        for (int i = 0; i < this.tests.Length; i++)
        {
            if (this.index == i)
            {
                GUILayout.Box(this.tests[i], new GUILayoutOption[0]);
            }
            else
            {
                GUILayout.Label(this.tests[i], new GUILayoutOption[0]);
            }
        }
        if (Event.current.type == EventType.Repaint)
        {
            if (this.weights == null)
            {
                this.weights = new List<float>();
            }
            else
            {
                this.weights.Clear();
            }
            this.weights.AddRange(this.stylist.Weights);
            int count = this.weights.Count;
            for (int j = 0; j < count; j++)
            {
                GUI.Box(new Rect((float) ((Screen.width / count) * j), (float) (Screen.height - 120), (float) (Screen.width / count), 120f * this.weights[j]), this.weights[j].ToString());
            }
            GUI.Label(new Rect((float) (Screen.width - 400), 0f, 400f, 100f), "\nPress up and down to change light style.\nHold space to apply it through LightStylist.Blend\nPress enter to apply it through LightStylist.CrossFade\nPress ctrl to apply it through LightStylist.Play");
        }
    }

    private void Reset()
    {
        this.tests = new string[] { "pulsate" };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.index = (this.index + 1) % this.tests.Length;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.index = (this.index + (this.tests.Length - 1)) % this.tests.Length;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            this.stylist.Blend(this.tests[this.index], this.spacebarTargetWeight, this.spacebarFadeLength);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.stylist.CrossFade(this.tests[this.index], this.enterCrossfadeLength);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) | Input.GetKeyDown(KeyCode.RightControl))
        {
            this.stylist.Play(this.tests[this.index]);
        }
    }
}

