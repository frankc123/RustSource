using System;
using UnityEngine;

public class VoiceChatIcon : MonoBehaviour
{
    private dfLabel label;

    private void OnEnable()
    {
        this.label = base.GetComponent<dfLabel>();
    }

    private void Update()
    {
        if (this.label != null)
        {
            float currentVolume = 0f;
            if (GameInput.GetButton("Voice").IsDown())
            {
                currentVolume = USpeaker.CurrentVolume;
            }
            this.label.Opacity = Mathf.Clamp((float) (currentVolume * 20f), (float) 0f, (float) 1f);
        }
    }
}

