using Facepunch.Build;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public class ClientInit : MonoBehaviour
{
    private void Awake()
    {
        Object.DontDestroyOnLoad(base.gameObject);
    }

    private void Start()
    {
        SteamClient.Create();
        ConsoleSystem.Run("config.load", false);
        ConsoleSystem.Run("serverfavourite.load", false);
        HudEnabled.Disable();
        DatablockDictionary.Initialize();
        Application.LoadLevelAdditive("GameUI");
        FeedbackLog.Start(FeedbackLog.TYPE.HardwareInfo);
        FeedbackLog.Writer.Write(SystemInfo.deviceModel);
        FeedbackLog.Writer.Write(SystemInfo.deviceName);
        FeedbackLog.Writer.Write(SystemInfo.graphicsDeviceName);
        FeedbackLog.Writer.Write(SystemInfo.graphicsDeviceVendor);
        FeedbackLog.Writer.Write(SystemInfo.graphicsDeviceVersion);
        FeedbackLog.Writer.Write(SystemInfo.graphicsMemorySize);
        FeedbackLog.Writer.Write(SystemInfo.graphicsShaderLevel);
        FeedbackLog.Writer.Write(SystemInfo.operatingSystem);
        FeedbackLog.Writer.Write(SystemInfo.processorCount);
        FeedbackLog.Writer.Write(SystemInfo.processorType);
        FeedbackLog.Writer.Write(SystemInfo.supportedRenderTargetCount);
        FeedbackLog.Writer.Write(SystemInfo.supportsComputeShaders);
        FeedbackLog.Writer.Write(SystemInfo.supportsImageEffects);
        FeedbackLog.Writer.Write(SystemInfo.supportsInstancing);
        FeedbackLog.Writer.Write(SystemInfo.supportsRenderTextures);
        FeedbackLog.Writer.Write(SystemInfo.supportsRenderToCubemap);
        FeedbackLog.Writer.Write(SystemInfo.supportsShadows);
        FeedbackLog.Writer.Write(SystemInfo.supportsStencil);
        FeedbackLog.Writer.Write(SystemInfo.systemMemorySize);
        FeedbackLog.End(FeedbackLog.TYPE.HardwareInfo);
        Connection.GameLoaded();
    }

    [DebuggerHidden]
    private IEnumerator uLink_OnDisconnectedFromServer(NetworkDisconnection netDisconnect)
    {
        return new <uLink_OnDisconnectedFromServer>c__Iterator42();
    }

    [CompilerGenerated]
    private sealed class <uLink_OnDisconnectedFromServer>c__Iterator42 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Exception <e>__0;
        internal Exception <e>__1;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_0098;

                case 1:
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_0098;

                case 2:
                    try
                    {
                        SoundPool.Drain();
                    }
                    catch (Exception exception)
                    {
                        this.<e>__0 = exception;
                        Debug.LogException(this.<e>__0);
                    }
                    try
                    {
                        DestroysOnDisconnect.OnDisconnectedFromServer();
                    }
                    catch (Exception exception2)
                    {
                        this.<e>__1 = exception2;
                        Debug.LogException(this.<e>__1);
                    }
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0098:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

