using System;
using UnityEngine;

public class SoundPoolTest : MonoBehaviour
{
    public float chanceOn;
    public AudioClip[] clips;
    private bool first;
    public float intervalPlayRandomClip;
    private float lastTime;
    public Transform[] on;

    private void OnEnable()
    {
        this.first = true;
    }

    public void OnGUI()
    {
        if (this.clips != null)
        {
            foreach (AudioClip clip in this.clips)
            {
                if (GUILayout.Button(clip.name, new GUILayoutOption[0]))
                {
                    clip.Play();
                }
            }
        }
        GUI.Box(new Rect((float) (Screen.width - 0x100), 0f, 256f, 24f), "Total Sound Nodes   " + SoundPool.totalCount);
        GUI.Box(new Rect((float) (Screen.width - 0x100), 30f, 256f, 24f), "Playing Sound Nodes " + SoundPool.playingCount);
        GUI.Box(new Rect((float) (Screen.width - 0x100), 60f, 256f, 24f), "Reserve Sound Nodes " + SoundPool.reserveCount);
        if (GUI.Button(new Rect((float) (Screen.width - 0x80), 90f, 128f, 24f), "Drain Reserves"))
        {
            SoundPool.DrainReserves();
        }
        if (GUI.Button(new Rect((float) (Screen.width - 0x80), 120f, 128f, 24f), "Drain"))
        {
            SoundPool.Drain();
        }
        if (GUI.Button(new Rect((float) (Screen.width - 0x80), 150f, 128f, 24f), "Stop All"))
        {
            SoundPool.Stop();
        }
    }

    private void Update()
    {
        if ((this.clips != null) && (this.intervalPlayRandomClip > 0f))
        {
            float num = Mathf.Max(0.05f, this.intervalPlayRandomClip);
            if (this.first)
            {
                this.lastTime = Time.time - num;
            }
            this.first = false;
            while ((Time.time - this.lastTime) >= num)
            {
                AudioClip clip = this.clips[Random.Range(0, this.clips.Length)];
                if (((this.on != null) && (this.on.Length > 0)) && (Random.value <= this.chanceOn))
                {
                    clip.Play(this.on[Random.Range(0, this.on.Length)]);
                }
                else
                {
                    clip.Play();
                }
                this.lastTime += num;
            }
        }
        else
        {
            this.first = true;
        }
    }
}

