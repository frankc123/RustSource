using Rust.Steam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class SteamClient : MonoBehaviour
{
    public static GameObject steamClientObject;
    protected static Vector3 vOldPosition = Vector3.zero;

    [DebuggerHidden]
    private IEnumerator _fjgh_()
    {
        return new <_fjgh_>c__Iterator30();
    }

    [DllImport("librust")]
    public static extern void a7d4f2d0762c4d5f3cf94d9da25207d8(int level, float x, float y, float z, float height, float speed, float fps);
    public static void Create()
    {
        if (!SteamClient_Init())
        {
            Application.Quit();
        }
        else
        {
            steamClientObject = new GameObject();
            Object.DontDestroyOnLoad(steamClientObject);
            steamClientObject.AddComponent<SteamClient>();
            steamClientObject.name = "SteamClient";
        }
    }

    [DllImport("librust")]
    public static extern void ed616599838f8ea421098a9e9f5e2d57(string strName);
    [DebuggerHidden]
    private IEnumerator hgueg()
    {
        return new <hgueg>c__Iterator31();
    }

    public static void Needed()
    {
        if (steamClientObject == null)
        {
            Create();
        }
    }

    public void OnDestroy()
    {
        SteamClient_Shutdown();
    }

    public void Start()
    {
        SteamGroups.Init();
        base.StartCoroutine(this._fjgh_());
        base.StartCoroutine(this.hgueg());
    }

    [DllImport("librust")]
    public static extern void SteamClient_Cycle();
    [DllImport("librust")]
    public static extern bool SteamClient_Init();
    [DllImport("librust")]
    public static extern void SteamClient_OnJoinServer(string strHost, int iIP);
    [DllImport("librust")]
    public static extern void SteamClient_Shutdown();
    [DllImport("librust")]
    public static extern void SteamUser_AdvertiseGame(ulong serverid, uint serverip, int serverport);
    public void Update()
    {
        Vector3 zero = Vector3.zero;
        float speed = 0f;
        float height = 1024f;
        PlayerClient localPlayer = PlayerClient.GetLocalPlayer();
        Controllable controllable = (localPlayer == null) ? null : localPlayer.controllable;
        if (controllable != null)
        {
            zero = controllable.transform.position;
            Vector3 vector2 = zero - vOldPosition;
            if ((vOldPosition != Vector3.zero) && (zero != Vector3.zero))
            {
                speed = vector2.magnitude;
            }
            vOldPosition = zero;
            if (controllable.GetComponent<Character>() != null)
            {
                RaycastHit hit;
                if (Physics.SphereCast(new Ray(zero + ((Vector3) (Vector3.up * 0.5f)), Vector3.down), 0.3f, out hit, 256f, 0x20180403))
                {
                    height = hit.distance;
                }
                else
                {
                    height = 1024f;
                }
            }
        }
        else
        {
            vOldPosition = Vector3.zero;
        }
        a7d4f2d0762c4d5f3cf94d9da25207d8(Application.loadedLevel, zero.x, zero.y, zero.z, height, speed, 1f / Time.smoothDeltaTime);
        SteamClient_Cycle();
    }

    [CompilerGenerated]
    private sealed class <_fjgh_>c__Iterator30 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <strFilename>__0;

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
                    break;

                case 1:
                    this.<strFilename>__0 = Environment.GetEnvironmentVariable("TEMP") + "/" + Path.GetRandomFileName();
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 2;
                    goto Label_00A6;

                case 2:
                    Application.CaptureScreenshot(this.<strFilename>__0);
                    SteamClient.ed616599838f8ea421098a9e9f5e2d57(this.<strFilename>__0);
                    break;

                default:
                    goto Label_00A4;
            }
            this.$current = new WaitForSeconds((float) Random.Range(300, 0x1c20));
            this.$PC = 1;
            goto Label_00A6;
            this.$PC = -1;
        Label_00A4:
            return false;
        Label_00A6:
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

    [CompilerGenerated]
    private sealed class <hgueg>c__Iterator31 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Type[] <$s_308>__7;
        internal int <$s_309>__8;
        internal Assembly[] <assemblies>__1;
        internal int <i>__2;
        internal int <iFields>__4;
        internal int <iMethods>__6;
        internal int <intCRC>__3;
        internal int <iProps>__5;
        internal Stopwatch <stopWatch>__0;
        internal TimeSpan <ts>__10;
        internal Type <v>__9;

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
                    this.$current = new WaitForSeconds(Random.Range((float) 60f, (float) 720f));
                    this.$PC = 1;
                    goto Label_0321;

                case 1:
                    this.<stopWatch>__0 = new Stopwatch();
                    this.<stopWatch>__0.Start();
                    this.<assemblies>__1 = AppDomain.CurrentDomain.GetAssemblies();
                    this.<i>__2 = 0;
                    goto Label_02E9;

                case 2:
                    this.<intCRC>__3 = (int) CRC32.String(this.<assemblies>__1[this.<i>__2].GetName().Name);
                    this.<iFields>__4 = 0;
                    this.<iProps>__5 = 0;
                    this.<iMethods>__6 = 0;
                    this.<$s_308>__7 = this.<assemblies>__1[this.<i>__2].GetTypes();
                    this.<$s_309>__8 = 0;
                    break;

                case 3:
                    this.<iFields>__4 += this.<v>__9.GetFields().Length;
                    this.<iProps>__5 += this.<v>__9.GetProperties().Length;
                    this.<iMethods>__6 += this.<v>__9.GetMethods().Length;
                    this.<$s_309>__8++;
                    break;

                default:
                    goto Label_031F;
            }
            if (this.<$s_309>__8 < this.<$s_308>__7.Length)
            {
                this.<v>__9 = this.<$s_308>__7[this.<$s_309>__8];
                this.$current = new WaitForEndOfFrame();
                this.$PC = 3;
                goto Label_0321;
            }
            FeedbackLog.Start(FeedbackLog.TYPE.Mods);
            FeedbackLog.Writer.Write(this.<intCRC>__3);
            FeedbackLog.Writer.Write(this.<assemblies>__1[this.<i>__2].GetName().Version.Major);
            FeedbackLog.Writer.Write((int) this.<assemblies>__1[this.<i>__2].GetName().Version.MajorRevision);
            FeedbackLog.Writer.Write(this.<assemblies>__1[this.<i>__2].GetName().Version.Minor);
            FeedbackLog.Writer.Write((int) this.<assemblies>__1[this.<i>__2].GetName().Version.MinorRevision);
            if (this.<assemblies>__1[this.<i>__2].ManifestModule is ModuleBuilder)
            {
                FeedbackLog.Writer.Write(0);
            }
            else
            {
                FeedbackLog.Writer.Write(CRC32.String(this.<assemblies>__1[this.<i>__2].Location));
            }
            FeedbackLog.Writer.Write(this.<assemblies>__1[this.<i>__2].GetTypes().Length);
            FeedbackLog.Writer.Write(this.<iFields>__4);
            FeedbackLog.Writer.Write(this.<iProps>__5);
            FeedbackLog.Writer.Write(this.<iMethods>__6);
            FeedbackLog.End(FeedbackLog.TYPE.Mods);
            this.<i>__2++;
        Label_02E9:
            if (this.<i>__2 < this.<assemblies>__1.Length)
            {
                this.$current = new WaitForEndOfFrame();
                this.$PC = 2;
                goto Label_0321;
            }
            this.<stopWatch>__0.Stop();
            this.<ts>__10 = this.<stopWatch>__0.Elapsed;
            this.$PC = -1;
        Label_031F:
            return false;
        Label_0321:
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

