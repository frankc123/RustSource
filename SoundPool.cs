using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SoundPool
{
    private static bool _enabled;
    private static bool _quitting;
    private static readonly Settings DEF;
    private static bool firstLeak = false;
    private const string goName = "zzz-soundpoolnode";
    private static readonly Type[] goTypes = new Type[] { typeof(AudioSource) };
    private static bool hadFirstLeak;
    private const float logarithmicMaxScale = 2f;
    private static readonly Root playing = new Root(RootID.PLAYING);
    private static readonly Root playingAttached = new Root(RootID.PLAYING_ATTACHED);
    private static readonly Root playingCamera = new Root(RootID.PLAYING_CAMERA);
    private static readonly Root reserved = new Root(RootID.RESERVED);
    private const sbyte SelectRoot_Attach = 2;
    private const sbyte SelectRoot_Attach_WorldOffset = 6;
    private const sbyte SelectRoot_Camera = 1;
    private const sbyte SelectRoot_Camera_WorldOffset = 5;
    private const sbyte SelectRoot_Default = 0;

    static SoundPool()
    {
        Settings settings = new Settings {
            volume = 1f,
            pitch = 1f,
            mode = AudioRolloffMode.Linear,
            min = 1f,
            max = 500f,
            panLevel = 1f,
            doppler = 1f,
            priority = 0x80,
            localRotation = Quaternion.identity
        };
        DEF = settings;
    }

    private static Node CreateNode()
    {
        if (reserved.first.has)
        {
            Node node = reserved.first.node;
            node.EnterLimbo();
            return node;
        }
        return NewNode();
    }

    public static void Drain()
    {
        Node node;
        Dir first = playing.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Dispose();
        }
        first = playingAttached.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Dispose();
        }
        first = playingCamera.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Dispose();
        }
        first = reserved.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Dispose();
        }
    }

    public static void DrainReserves()
    {
        Dir first = reserved.first;
        while (first.has)
        {
            Node node = first.node;
            first = first.node.way.next;
            node.Dispose();
        }
    }

    private static Node NewNode()
    {
        Node node = new Node();
        GameObject target = new GameObject("zzz-soundpoolnode", goTypes) {
            hideFlags = HideFlags.NotEditable
        };
        Object.DontDestroyOnLoad(target);
        node.audio = target.audio;
        node.transform = target.transform;
        node.audio.playOnAwake = false;
        node.audio.enabled = false;
        return node;
    }

    private static void Play(ref Settings settings)
    {
        Root playingCamera;
        RootID pLAYING;
        bool flag;
        Vector3 localPosition;
        Vector3 vector2;
        Quaternion quaternion;
        Quaternion localRotation;
        RootID tid2;
        if ((!_enabled || (settings.volume <= 0f)) || ((settings.pitch == 0f) || (settings.clip == null)))
        {
            return;
        }
        Transform transform = null;
        switch (settings.SelectRoot)
        {
            case 1:
                if (Camera.main != null)
                {
                    transform = Camera.main.transform;
                    playingCamera = SoundPool.playingCamera;
                    pLAYING = RootID.PLAYING_CAMERA;
                    flag = false;
                    break;
                }
                return;

            case 2:
                if (settings.parent != null)
                {
                    playingCamera = playingAttached;
                    pLAYING = RootID.PLAYING_ATTACHED;
                    flag = false;
                    break;
                }
                return;

            case 5:
                if (Camera.main != null)
                {
                    transform = Camera.main.transform;
                    playingCamera = SoundPool.playingCamera;
                    pLAYING = RootID.PLAYING_CAMERA;
                    flag = true;
                    break;
                }
                return;

            case 6:
                if (settings.parent != null)
                {
                    playingCamera = playingAttached;
                    pLAYING = RootID.PLAYING_ATTACHED;
                    flag = true;
                    break;
                }
                return;

            default:
                playingCamera = playing;
                pLAYING = RootID.PLAYING;
                flag = false;
                break;
        }
        if (!flag)
        {
            localPosition = settings.localPosition;
            quaternion = settings.localRotation;
            tid2 = pLAYING;
            switch ((((int) tid2) + 3))
            {
                case RootID.LIMBO:
                    vector2 = settings.parent.TransformPoint(localPosition);
                    localRotation = settings.parent.rotation * quaternion;
                    goto Label_022B;

                case RootID.RESERVED:
                    vector2 = transform.TransformPoint(localPosition);
                    localRotation = transform.rotation * quaternion;
                    goto Label_022B;

                case RootID.DISPOSED:
                    vector2 = localPosition;
                    localRotation = quaternion;
                    goto Label_022B;
            }
            return;
        }
        tid2 = pLAYING;
        if (tid2 != RootID.PLAYING_ATTACHED)
        {
            if (tid2 != RootID.PLAYING_CAMERA)
            {
                return;
            }
        }
        else
        {
            localPosition = settings.parent.InverseTransformPoint(settings.localPosition);
            quaternion = settings.localRotation * Quaternion.Inverse(settings.parent.rotation);
            goto Label_0195;
        }
        localPosition = transform.InverseTransformPoint(settings.localPosition);
        quaternion = settings.localRotation * Quaternion.Inverse(transform.rotation);
    Label_0195:
        vector2 = settings.localPosition;
        localRotation = settings.localRotation;
    Label_022B:
        if (transform == null)
        {
            Camera main = Camera.main;
            if (main == null)
            {
                return;
            }
            transform = main.transform;
            float num = Vector3.Distance(vector2, transform.position);
            switch (settings.mode)
            {
                case AudioRolloffMode.Logarithmic:
                    if (num <= (settings.max * 2f))
                    {
                        break;
                    }
                    return;

                case AudioRolloffMode.Linear:
                case AudioRolloffMode.Custom:
                    if (num <= settings.max)
                    {
                        break;
                    }
                    return;
            }
        }
        Node node = CreateNode();
        if (((int) node.rootID) != 0)
        {
            Debug.LogWarning("Wasn't Limbo " + node.rootID);
        }
        node.root = playingCamera;
        node.rootID = pLAYING;
        node.audio.pan = settings.pan;
        node.audio.panLevel = settings.panLevel;
        node.audio.volume = settings.volume;
        node.audio.dopplerLevel = settings.doppler;
        node.audio.pitch = settings.pitch;
        node.audio.rolloffMode = settings.mode;
        node.audio.minDistance = settings.min;
        node.audio.maxDistance = settings.max;
        node.audio.spread = settings.spread;
        node.audio.bypassEffects = settings.bypassEffects;
        node.audio.priority = settings.priority;
        node.parent = settings.parent;
        node.transform.position = vector2;
        node.transform.rotation = localRotation;
        node.translation = localPosition;
        node.rotation = quaternion;
        node.audio.clip = settings.clip;
        node.Bind();
        node.audio.enabled = true;
        node.audio.Play();
    }

    public static void Play(this AudioClip clip)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, float volume, float pan)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, float volume, float pan, float pitch)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, float volume, float pan, Vector3 worldPosition)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 5;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        dEF.localPosition = worldPosition;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, float volume, float pan, Vector3 worldPosition, Quaternion worldRotation)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 5;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        dEF.localPosition = worldPosition;
        dEF.localRotation = worldRotation;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float minDistance, float maxDistance, int priority)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.priority = priority;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, int priority)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.priority = priority;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Play(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 6;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, float volume, float pan, Vector3 worldPosition)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        dEF.localPosition = worldPosition;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, float volume, float pan, Vector3 worldPosition, Quaternion worldRotation)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 1;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pan = pan;
        dEF.localPosition = worldPosition;
        dEF.localRotation = worldRotation;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, int priority)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.priority = priority;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, int priority)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.priority = priority;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, AudioRolloffMode rolloffMode, float minDistance, float maxDistance)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.mode = rolloffMode;
        dEF.spread = spread;
        Play(ref dEF);
    }

    public static void PlayLocal(this AudioClip clip, Transform on, Vector3 position, Quaternion rotation, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float dopplerLevel, float spread, bool bypassEffects)
    {
        Settings dEF = DEF;
        dEF.SelectRoot = 2;
        dEF.parent = on;
        dEF.localPosition = position;
        dEF.localRotation = rotation;
        dEF.clip = clip;
        dEF.volume = volume;
        dEF.pitch = pitch;
        dEF.doppler = dopplerLevel;
        dEF.min = minDistance;
        dEF.max = maxDistance;
        dEF.spread = spread;
        dEF.bypassEffects = bypassEffects;
        dEF.mode = rolloffMode;
        Play(ref dEF);
    }

    public static void Pump()
    {
        if (firstLeak)
        {
            if (!hadFirstLeak)
            {
                Debug.LogWarning("SoundPool node leaked for the first time. Though performance should still be good, from now on until application exit there will be extra processing in Pump to clean up game objects of leaked/gc'd nodes. [ie. a mutex is now being locked and unlocked]");
                hadFirstLeak = true;
            }
            NodeGC.JOIN();
        }
        Dir first = playingCamera.first;
        if (first.has)
        {
            Camera main = Camera.main;
            if (main != null)
            {
                Transform transform = main.transform;
                Quaternion rotation = transform.rotation;
                do
                {
                    Node node = first.node;
                    first = first.node.way.next;
                    if (node.audio.isPlaying)
                    {
                        node.transform.position = transform.TransformPoint(node.translation);
                        node.transform.rotation = rotation * node.rotation;
                    }
                    else
                    {
                        node.Reserve();
                    }
                }
                while (first.has);
            }
            else
            {
                do
                {
                    Node node2 = first.node;
                    first = first.node.way.next;
                    node2.Reserve();
                }
                while (first.has);
            }
        }
        first = playingAttached.first;
        while (first.has)
        {
            Node node3 = first.node;
            first = first.node.way.next;
            if (node3.audio.isPlaying && (node3.parent != null))
            {
                node3.transform.position = node3.parent.TransformPoint(node3.translation);
                node3.transform.rotation = node3.parent.rotation * node3.rotation;
            }
            else
            {
                node3.Reserve();
            }
        }
        first = playing.first;
        while (first.has)
        {
            Node node4 = first.node;
            first = first.node.way.next;
            if (!node4.audio.isPlaying)
            {
                node4.Reserve();
            }
        }
    }

    public static void Stop()
    {
        Node node;
        Dir first = playing.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Reserve();
        }
        first = playingAttached.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Reserve();
        }
        first = playingCamera.first;
        while (first.has)
        {
            node = first.node;
            first = first.node.way.next;
            node.Reserve();
        }
    }

    private static Object TARG(ref Settings settings)
    {
        return ((settings.parent == null) ? ((Object) settings.clip) : ((Object) settings.parent));
    }

    internal static bool enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (value)
            {
                _enabled = !_quitting;
            }
            else
            {
                _enabled = false;
            }
        }
    }

    public static int playingCount
    {
        get
        {
            return ((playingCamera.count + playingAttached.count) + playing.count);
        }
    }

    internal static bool quitting
    {
        set
        {
            if (!_quitting && value)
            {
                _quitting = true;
                _enabled = false;
                Drain();
            }
        }
    }

    public static int reserveCount
    {
        get
        {
            return reserved.count;
        }
    }

    public static int totalCount
    {
        get
        {
            return (((playingCamera.count + playingAttached.count) + playing.count) + reserved.count);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Dir
    {
        public SoundPool.Node node;
        public bool has;
    }

    private sealed class Node : IDisposable
    {
        public AudioSource audio;
        public Transform parent;
        public SoundPool.Root root;
        public SoundPool.RootID rootID;
        public Quaternion rotation;
        public Transform transform;
        public Vector3 translation;
        public SoundPool.Way way;

        public void Bind()
        {
            this.way.prev = new SoundPool.Dir();
            this.way.next = this.root.first;
            this.root.first.has = true;
            this.root.first.node = this;
            if (this.way.next.has)
            {
                this.way.next.node.way.prev.has = true;
                this.way.next.node.way.prev.node = this;
            }
            this.root.count++;
        }

        public void Dispose()
        {
            switch (this.rootID)
            {
                case SoundPool.RootID.LIMBO:
                    break;

                case SoundPool.RootID.DISPOSED:
                    return;

                default:
                    this.EnterLimbo();
                    break;
            }
            Object.Destroy(this.transform.gameObject);
            this.transform = null;
            this.audio = null;
            this.rootID = SoundPool.RootID.DISPOSED;
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public void EnterLimbo()
        {
            switch (this.rootID)
            {
                case SoundPool.RootID.LIMBO:
                case SoundPool.RootID.DISPOSED:
                    return;

                case SoundPool.RootID.RESERVED:
                    break;

                default:
                    this.audio.Stop();
                    this.audio.enabled = false;
                    this.audio.clip = null;
                    this.parent = null;
                    break;
            }
            if (this.way.prev.has)
            {
                this.way.prev.node.way.next = this.way.next;
            }
            else
            {
                this.root.first = this.way.next;
            }
            if (this.way.next.has)
            {
                this.way.next.node.way.prev = this.way.prev;
            }
            this.root.count--;
            this.way = new SoundPool.Way();
            this.root = null;
            this.rootID = SoundPool.RootID.LIMBO;
        }

        ~Node()
        {
            if (((int) this.rootID) != 2)
            {
                SoundPool.NodeGC.LEAK(this.transform);
            }
            this.transform = null;
            this.audio = null;
        }

        public void Reserve()
        {
            switch (this.rootID)
            {
                case SoundPool.RootID.LIMBO:
                    break;

                case SoundPool.RootID.RESERVED:
                case SoundPool.RootID.DISPOSED:
                    return;

                default:
                    this.audio.Stop();
                    this.audio.enabled = false;
                    this.audio.clip = null;
                    this.parent = null;
                    if (this.way.next.has)
                    {
                        this.way.next.node.way.prev = this.way.prev;
                    }
                    if (this.way.prev.has)
                    {
                        this.way.prev.node.way.next = this.way.next;
                    }
                    else
                    {
                        this.root.first = this.way.next;
                    }
                    this.root.count--;
                    this.way = new SoundPool.Way();
                    break;
            }
            this.root = SoundPool.reserved;
            this.rootID = SoundPool.RootID.RESERVED;
            this.way.next = SoundPool.reserved.first;
            if (this.way.next.has)
            {
                this.way.next.node.way.prev.has = true;
                this.way.next.node.way.prev.node = this;
            }
            SoundPool.reserved.first.has = true;
            SoundPool.reserved.first.node = this;
            SoundPool.reserved.count++;
        }
    }

    private static class NodeGC
    {
        public static void JOIN()
        {
            Transform[] transformArray = null;
            bool flag = false;
            object destroyNextPumpLock = GCDAT.destroyNextPumpLock;
            lock (destroyNextPumpLock)
            {
                if (GCDAT.destroyNextQueued)
                {
                    flag = true;
                    transformArray = GCDAT.destroyTheseNextPump.ToArray();
                    GCDAT.destroyTheseNextPump.Clear();
                    GCDAT.destroyNextQueued = false;
                }
            }
            if (flag)
            {
                foreach (Transform transform in transformArray)
                {
                    if (transform != null)
                    {
                        Object.Destroy(transform.gameObject);
                    }
                }
                Debug.LogWarning("There were " + transformArray.Length + " SoundPool nodes leaked!. Cleaned them up.");
            }
        }

        public static void LEAK(Transform transform)
        {
            object destroyNextPumpLock = GCDAT.destroyNextPumpLock;
            lock (destroyNextPumpLock)
            {
                GCDAT.destroyNextQueued = true;
                GCDAT.destroyTheseNextPump.Add(transform);
            }
        }

        private static class GCDAT
        {
            public static readonly object destroyNextPumpLock = new object();
            public static bool destroyNextQueued;
            public static readonly List<Transform> destroyTheseNextPump = new List<Transform>();

            static GCDAT()
            {
                SoundPool.firstLeak = true;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Player2D
    {
        public static readonly SoundPool.Player2D Default;
        public SoundPool.PlayerShared super;
        public float pan;
        public Player2D(AudioClip clip)
        {
            this.super = new SoundPool.PlayerShared(clip);
            this.pan = SoundPool.DEF.pan;
        }

        static Player2D()
        {
            SoundPool.Player2D playerd = new SoundPool.Player2D {
                super = SoundPool.PlayerShared.Default,
                pan = SoundPool.DEF.pan
            };
            Default = playerd;
        }

        public float volume
        {
            get
            {
                return this.super.volume;
            }
            set
            {
                this.super.volume = value;
            }
        }
        public float pitch
        {
            get
            {
                return this.super.pitch;
            }
            set
            {
                this.super.pitch = value;
            }
        }
        public int priority
        {
            get
            {
                return this.super.priority;
            }
            set
            {
                this.super.priority = value;
            }
        }
        public AudioClip clip
        {
            get
            {
                return this.super.clip;
            }
            set
            {
                this.super.clip = value;
            }
        }
        public void Play()
        {
            this.Play(this.clip);
        }

        public void Play(AudioClip clip)
        {
            if (clip == null)
            {
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Player3D
    {
        public static readonly SoundPool.Player3D Default;
        public SoundPool.PlayerShared super;
        public float minDistance;
        public float maxDistance;
        public float spread;
        public float dopplerLevel;
        public float panLevel;
        public AudioRolloffMode rolloffMode;
        public bool cameraSticky;
        public bool bypassEffects;
        public Player3D(AudioClip clip)
        {
            this.super = new SoundPool.PlayerShared(clip);
            this.minDistance = SoundPool.DEF.min;
            this.maxDistance = SoundPool.DEF.max;
            this.spread = SoundPool.DEF.spread;
            this.dopplerLevel = SoundPool.DEF.doppler;
            this.panLevel = SoundPool.DEF.panLevel;
            this.rolloffMode = SoundPool.DEF.mode;
            this.bypassEffects = SoundPool.DEF.bypassEffects;
            this.cameraSticky = false;
        }

        static Player3D()
        {
            SoundPool.Player3D playerd = new SoundPool.Player3D {
                super = SoundPool.PlayerShared.Default,
                minDistance = SoundPool.DEF.min,
                maxDistance = SoundPool.DEF.max,
                rolloffMode = SoundPool.DEF.mode,
                spread = SoundPool.DEF.spread,
                dopplerLevel = SoundPool.DEF.doppler,
                bypassEffects = SoundPool.DEF.bypassEffects,
                panLevel = SoundPool.DEF.panLevel
            };
            Default = playerd;
        }

        public float volume
        {
            get
            {
                return this.super.volume;
            }
            set
            {
                this.super.volume = value;
            }
        }
        public float pitch
        {
            get
            {
                return this.super.pitch;
            }
            set
            {
                this.super.pitch = value;
            }
        }
        public int priority
        {
            get
            {
                return this.super.priority;
            }
            set
            {
                this.super.priority = value;
            }
        }
        public AudioClip clip
        {
            get
            {
                return this.super.clip;
            }
            set
            {
                this.super.clip = value;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerChild
    {
        public static readonly SoundPool.PlayerChild Default;
        public SoundPool.PlayerLocal super;
        public bool unglue;
        public Transform parent;
        public PlayerChild(AudioClip clip)
        {
            this.super = new SoundPool.PlayerLocal(clip);
            this.parent = null;
            this.unglue = false;
        }

        static PlayerChild()
        {
            SoundPool.PlayerChild child = new SoundPool.PlayerChild {
                super = SoundPool.PlayerLocal.Default
            };
            Default = child;
        }

        public float volume
        {
            get
            {
                return this.super.volume;
            }
            set
            {
                this.super.volume = value;
            }
        }
        public float pitch
        {
            get
            {
                return this.super.pitch;
            }
            set
            {
                this.super.pitch = value;
            }
        }
        public int priority
        {
            get
            {
                return this.super.priority;
            }
            set
            {
                this.super.priority = value;
            }
        }
        public AudioClip clip
        {
            get
            {
                return this.super.clip;
            }
            set
            {
                this.super.clip = value;
            }
        }
        public float minDistance
        {
            get
            {
                return this.super.minDistance;
            }
            set
            {
                this.super.minDistance = value;
            }
        }
        public float maxDistance
        {
            get
            {
                return this.super.maxDistance;
            }
            set
            {
                this.super.maxDistance = value;
            }
        }
        public float spread
        {
            get
            {
                return this.super.spread;
            }
            set
            {
                this.super.spread = value;
            }
        }
        public float dopplerLevel
        {
            get
            {
                return this.super.dopplerLevel;
            }
            set
            {
                this.super.dopplerLevel = value;
            }
        }
        public AudioRolloffMode rolloffMode
        {
            get
            {
                return this.super.rolloffMode;
            }
            set
            {
                this.super.rolloffMode = value;
            }
        }
        public bool bypassEffects
        {
            get
            {
                return this.super.bypassEffects;
            }
            set
            {
                this.super.bypassEffects = value;
            }
        }
        public bool cameraSticky
        {
            get
            {
                return this.super.cameraSticky;
            }
            set
            {
                this.super.cameraSticky = value;
            }
        }
        public Vector3 localPosition
        {
            get
            {
                return this.super.localPosition;
            }
            set
            {
                this.super.localPosition = value;
            }
        }
        public Quaternion localRotation
        {
            get
            {
                return this.super.localRotation;
            }
            set
            {
                this.super.localRotation = value;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerLocal
    {
        public static readonly SoundPool.PlayerLocal Default;
        public SoundPool.Player3D super;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public PlayerLocal(AudioClip clip)
        {
            this.super = new SoundPool.Player3D(clip);
            this.localPosition = new Vector3();
            this.localRotation = Quaternion.identity;
        }

        static PlayerLocal()
        {
            SoundPool.PlayerLocal local = new SoundPool.PlayerLocal {
                super = SoundPool.Player3D.Default,
                localPosition = new Vector3(),
                localRotation = Quaternion.identity
            };
            Default = local;
        }

        public float volume
        {
            get
            {
                return this.super.volume;
            }
            set
            {
                this.super.volume = value;
            }
        }
        public float pitch
        {
            get
            {
                return this.super.pitch;
            }
            set
            {
                this.super.pitch = value;
            }
        }
        public int priority
        {
            get
            {
                return this.super.priority;
            }
            set
            {
                this.super.priority = value;
            }
        }
        public AudioClip clip
        {
            get
            {
                return this.super.clip;
            }
            set
            {
                this.super.clip = value;
            }
        }
        public float minDistance
        {
            get
            {
                return this.super.minDistance;
            }
            set
            {
                this.super.minDistance = value;
            }
        }
        public float maxDistance
        {
            get
            {
                return this.super.maxDistance;
            }
            set
            {
                this.super.maxDistance = value;
            }
        }
        public float spread
        {
            get
            {
                return this.super.spread;
            }
            set
            {
                this.super.spread = value;
            }
        }
        public float dopplerLevel
        {
            get
            {
                return this.super.dopplerLevel;
            }
            set
            {
                this.super.dopplerLevel = value;
            }
        }
        public float panLevel
        {
            get
            {
                return this.super.panLevel;
            }
            set
            {
                this.super.panLevel = value;
            }
        }
        public AudioRolloffMode rolloffMode
        {
            get
            {
                return this.super.rolloffMode;
            }
            set
            {
                this.super.rolloffMode = value;
            }
        }
        public bool bypassEffects
        {
            get
            {
                return this.super.bypassEffects;
            }
            set
            {
                this.super.bypassEffects = value;
            }
        }
        public bool cameraSticky
        {
            get
            {
                return this.super.cameraSticky;
            }
            set
            {
                this.super.cameraSticky = value;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerShared
    {
        public static readonly SoundPool.PlayerShared Default;
        public AudioClip clip;
        public float volume;
        public float pitch;
        public int priority;
        public PlayerShared(AudioClip clip)
        {
            this.clip = clip;
            this.volume = SoundPool.DEF.volume;
            this.pitch = SoundPool.DEF.pitch;
            this.priority = SoundPool.DEF.priority;
        }

        static PlayerShared()
        {
            SoundPool.PlayerShared shared = new SoundPool.PlayerShared {
                volume = SoundPool.DEF.volume,
                pitch = SoundPool.DEF.pitch,
                priority = SoundPool.DEF.priority
            };
            Default = shared;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerWorld
    {
        public static readonly SoundPool.PlayerWorld Default;
        public SoundPool.Player3D super;
        public Vector3 position;
        public Quaternion rotation;
        public PlayerWorld(AudioClip clip)
        {
            this.super = new SoundPool.Player3D(clip);
            this.position = new Vector3();
            this.rotation = Quaternion.identity;
        }

        static PlayerWorld()
        {
            SoundPool.PlayerWorld world = new SoundPool.PlayerWorld {
                super = SoundPool.Player3D.Default,
                position = new Vector3(),
                rotation = Quaternion.identity
            };
            Default = world;
        }

        public float volume
        {
            get
            {
                return this.super.volume;
            }
            set
            {
                this.super.volume = value;
            }
        }
        public float pitch
        {
            get
            {
                return this.super.pitch;
            }
            set
            {
                this.super.pitch = value;
            }
        }
        public int priority
        {
            get
            {
                return this.super.priority;
            }
            set
            {
                this.super.priority = value;
            }
        }
        public AudioClip clip
        {
            get
            {
                return this.super.clip;
            }
            set
            {
                this.super.clip = value;
            }
        }
        public float minDistance
        {
            get
            {
                return this.super.minDistance;
            }
            set
            {
                this.super.minDistance = value;
            }
        }
        public float maxDistance
        {
            get
            {
                return this.super.maxDistance;
            }
            set
            {
                this.super.maxDistance = value;
            }
        }
        public float spread
        {
            get
            {
                return this.super.spread;
            }
            set
            {
                this.super.spread = value;
            }
        }
        public float dopplerLevel
        {
            get
            {
                return this.super.dopplerLevel;
            }
            set
            {
                this.super.dopplerLevel = value;
            }
        }
        public float panLevel
        {
            get
            {
                return this.super.panLevel;
            }
            set
            {
                this.super.panLevel = value;
            }
        }
        public AudioRolloffMode rolloffMode
        {
            get
            {
                return this.super.rolloffMode;
            }
            set
            {
                this.super.rolloffMode = value;
            }
        }
        public bool bypassEffects
        {
            get
            {
                return this.super.bypassEffects;
            }
            set
            {
                this.super.bypassEffects = value;
            }
        }
        public bool cameraSticky
        {
            get
            {
                return this.super.cameraSticky;
            }
            set
            {
                this.super.cameraSticky = value;
            }
        }
    }

    private class Root
    {
        public int count;
        public SoundPool.Dir first;
        public readonly SoundPool.RootID id;

        public Root(SoundPool.RootID id)
        {
            this.id = id;
        }
    }

    private enum RootID : sbyte
    {
        DISPOSED = 2,
        LIMBO = 0,
        PLAYING = -1,
        PLAYING_ATTACHED = -3,
        PLAYING_CAMERA = -2,
        RESERVED = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Settings
    {
        public AudioClip clip;
        public Transform parent;
        public Quaternion localRotation;
        public Vector3 localPosition;
        public float volume;
        public float pitch;
        public float pan;
        public float panLevel;
        public float min;
        public float max;
        public float doppler;
        public float spread;
        public int priority;
        public AudioRolloffMode mode;
        public sbyte SelectRoot;
        public bool bypassEffects;
        public static explicit operator SoundPool.Settings(SoundPool.PlayerShared player)
        {
            SoundPool.Settings dEF = SoundPool.DEF;
            dEF.clip = player.clip;
            dEF.volume = player.volume;
            dEF.pitch = player.pitch;
            dEF.priority = player.priority;
            return dEF;
        }

        public static explicit operator SoundPool.Settings(SoundPool.Player3D player)
        {
            SoundPool.Settings super = (SoundPool.Settings) player.super;
            super.doppler = player.dopplerLevel;
            super.min = player.minDistance;
            super.max = player.maxDistance;
            super.panLevel = player.panLevel;
            super.spread = player.spread;
            super.mode = player.rolloffMode;
            super.bypassEffects = player.bypassEffects;
            super.SelectRoot = !player.cameraSticky ? ((sbyte) 0) : ((sbyte) 5);
            return super;
        }

        public static explicit operator SoundPool.Settings(SoundPool.PlayerLocal player)
        {
            SoundPool.Settings super = (SoundPool.Settings) player.super;
            super.localPosition = player.localPosition;
            super.localRotation = player.localRotation;
            return super;
        }

        public static explicit operator SoundPool.Settings(SoundPool.Player2D player)
        {
            SoundPool.Settings super = (SoundPool.Settings) player.super;
            super.pan = player.pan;
            return super;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Way
    {
        public SoundPool.Dir prev;
        public SoundPool.Dir next;
    }
}

