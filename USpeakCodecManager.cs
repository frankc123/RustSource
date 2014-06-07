using MoPhoGames.USpeak.Codec;
using System;
using UnityEngine;

public class USpeakCodecManager : ScriptableObject
{
    public string[] CodecNames = new string[0];
    public ICodec[] Codecs;
    public string[] FriendlyNames = new string[0];
    private static USpeakCodecManager instance;

    public static USpeakCodecManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (USpeakCodecManager) Resources.Load("CodecManager");
                if (instance == null)
                {
                    Debug.LogError("Couldn't load Resources/CodecManager!");
                }
                if (Application.isPlaying)
                {
                    instance.Codecs = new ICodec[instance.CodecNames.Length];
                    for (int i = 0; i < instance.Codecs.Length; i++)
                    {
                        instance.Codecs[i] = (ICodec) Activator.CreateInstance(Type.GetType(instance.CodecNames[i]));
                    }
                }
            }
            return instance;
        }
    }
}

