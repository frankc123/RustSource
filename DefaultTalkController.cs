using MoPhoGames.USpeak.Interface;
using System;
using UnityEngine;

[AddComponentMenu("USpeak/Default Talk Controller")]
public class DefaultTalkController : MonoBehaviour, IUSpeakTalkController
{
    [SerializeField, HideInInspector]
    public int ToggleMode;
    [HideInInspector, SerializeField]
    public KeyCode TriggerKey;
    private bool val;

    public void OnInspectorGUI()
    {
    }

    public bool ShouldSend()
    {
        if (this.ToggleMode == 0)
        {
            this.val = Input.GetKey(this.TriggerKey);
        }
        else if (Input.GetKeyDown(this.TriggerKey))
        {
            this.val = !this.val;
        }
        return this.val;
    }
}

