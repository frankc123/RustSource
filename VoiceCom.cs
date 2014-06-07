using MoPhoGames.USpeak.Interface;
using System;
using uLink;
using UnityEngine;

public sealed class VoiceCom : IDLocalCharacter, IVoiceCom, ISpeechDataHandler, IUSpeakTalkController
{
    [NonSerialized]
    private USpeaker _uspeaker;
    [NonSerialized]
    private bool _uspeakerChecked;
    private int setupData;

    [RPC]
    private void clientspeak(int setupData, byte[] data)
    {
    }

    public static float GetVolume()
    {
        return 0f;
    }

    [RPC]
    private void init(int data)
    {
        this.uspeaker.InitializeSettings(data);
    }

    void IUSpeakTalkController.OnInspectorGUI()
    {
    }

    public bool ShouldSend()
    {
        Character idMain = base.idMain;
        return (((idMain != null) && idMain.alive) && GameInput.GetButton("Voice").IsDown());
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (!base.networkView.isMine)
        {
            this.uspeaker.SpeakerMode = SpeakerMode.Remote;
        }
        else
        {
            this.uspeaker.SpeakerMode = SpeakerMode.Local;
        }
    }

    public void USpeakInitializeSettings(int data)
    {
        this.setupData = data;
    }

    public void USpeakOnSerializeAudio(byte[] data)
    {
        object[] args = new object[] { this.setupData, data };
        base.networkView.RPC("clientspeak", RPCMode.Server, args);
    }

    [RPC]
    private void voiceplay(float hearDistance, int setupData, byte[] data)
    {
        Camera main = Camera.main;
        if ((main != null) && (hearDistance > 0f))
        {
            USpeaker uspeaker = this.uspeaker;
            if (uspeaker == null)
            {
                Debug.LogWarning("voiceplayback:" + base.gameObject + " didn't have a USpeaker!?");
            }
            if (!uspeaker.HasSettings())
            {
                uspeaker.InitializeSettings(setupData);
            }
            if (data == null)
            {
                Debug.LogWarning("voiceplayback: data was null!");
            }
            float num = Vector3.Distance(main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0001f)), base.eyesOrigin);
            float num2 = Mathf.Clamp01(1f - (num / hearDistance));
            uspeaker.SpeakerVolume = num2;
            uspeaker.ReceiveAudio(data);
            AudioSource audio = uspeaker.audio;
            if (audio != null)
            {
                audio.rolloffMode = AudioRolloffMode.Linear;
                audio.maxDistance = hearDistance;
                audio.minDistance = 1f;
            }
        }
    }

    public USpeaker uspeaker
    {
        get
        {
            if (!this._uspeakerChecked)
            {
                this._uspeaker = USpeaker.Get(this);
                this._uspeakerChecked = true;
            }
            return this._uspeaker;
        }
    }
}

