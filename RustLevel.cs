using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class RustLevel
{
    private static void BroadcastGlobalMessage(string messageName)
    {
        foreach (GameObject obj2 in CollectRootGameObjects())
        {
            if (obj2 != null)
            {
                obj2.BroadcastMessage(messageName, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private static List<GameObject> CollectRootGameObjects()
    {
        HashSet<Transform> set = new HashSet<Transform>();
        List<GameObject> list = new List<GameObject>();
        foreach (Object obj2 in Object.FindObjectsOfType(typeof(Transform)))
        {
            if (obj2 != null)
            {
                Transform root = ((Transform) obj2).root;
                if (set.Add(root))
                {
                    list.Add(root.gameObject);
                }
            }
        }
        return list;
    }

    public static void LevelLoadLog(byte iStage)
    {
        FeedbackLog.Start(FeedbackLog.TYPE.LoadProgress);
        FeedbackLog.Writer.Write(iStage);
        FeedbackLog.End(FeedbackLog.TYPE.LoadProgress);
    }

    public static Coroutine Load(string levelName)
    {
        GameObject obj2;
        return Load(levelName, out obj2);
    }

    public static Coroutine Load(string levelName, out GameObject loader)
    {
        Globals.currentLevel = levelName;
        Type[] components = new Type[] { typeof(MonoBehaviour) };
        loader = new GameObject("Loading Level:" + levelName, components);
        Object.DontDestroyOnLoad(loader);
        MonoBehaviour component = loader.GetComponent<MonoBehaviour>();
        return component.StartCoroutine(LoadRoutine(component, levelName));
    }

    [DebuggerHidden]
    private static IEnumerator LoadRoutine(MonoBehaviour script, string levelName)
    {
        return new <LoadRoutine>c__Iterator32 { levelName = levelName, script = script, <$>levelName = levelName, <$>script = script };
    }

    [DebuggerHidden]
    private static IEnumerator WaitForCondition(Func<bool> condition, string requestLabel)
    {
        return new <WaitForCondition>c__Iterator33 { condition = condition, requestLabel = requestLabel, <$>condition = condition, <$>requestLabel = requestLabel };
    }

    private static Coroutine WaitForCondition(MonoBehaviour script, Func<bool> condition, string requestLabel)
    {
        return script.StartCoroutine(WaitForCondition(condition, requestLabel));
    }

    [CompilerGenerated]
    private sealed class <LoadRoutine>c__Iterator32 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>levelName;
        internal MonoBehaviour <$>script;
        private static Func<bool> <>f__am$cacheA;
        private static Func<bool> <>f__am$cacheB;
        internal AsyncOperation <async>__0;
        internal AsyncOperation <async>__1;
        internal AsyncOperation <async>__2;
        internal AsyncOperation <async>__3;
        internal string levelName;
        internal MonoBehaviour script;

        private static bool <>m__10()
        {
            return (ServerManagement.Get() != null);
        }

        private static bool <>m__11()
        {
            return Controllable.localPlayerControllableExists;
        }

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
                    RustLevel.LevelLoadLog(1);
                    Globals.isLoading = true;
                    Application.backgroundLoadingPriority = ThreadPriority.Low;
                    LoadingScreen.Update("loading " + this.levelName);
                    NetCull.isMessageQueueRunning = false;
                    HudEnabled.Disable();
                    this.<async>__0 = Application.LoadLevelAsync(this.levelName);
                    this.<async>__0.allowSceneActivation = false;
                    LoadingScreen.Operations.Add(this.<async>__0);
                    break;

                case 1:
                    break;

                case 2:
                    this.<async>__0.allowSceneActivation = true;
                    this.$current = this.<async>__0;
                    this.$PC = 3;
                    goto Label_045B;

                case 3:
                    RustLevel.LevelLoadLog(2);
                    if (!Application.CanStreamedLevelBeLoaded(this.levelName + "-TREES"))
                    {
                        Debug.Log("No tree level found.");
                        goto Label_01D3;
                    }
                    LoadingScreen.Update("loading trees");
                    this.<async>__1 = Application.LoadLevelAdditiveAsync(this.levelName + "-TREES");
                    LoadingScreen.Operations.Add(this.<async>__1);
                    this.$current = this.<async>__1;
                    this.$PC = 4;
                    goto Label_045B;

                case 4:
                    LoadingScreen.Operations.Clear();
                    goto Label_01D3;

                case 5:
                    RustLevel.LevelLoadLog(3);
                    LoadingScreen.Update("loading gui");
                    this.<async>__2 = Application.LoadLevelAdditiveAsync("RPOS");
                    LoadingScreen.Operations.Add(this.<async>__2);
                    this.$current = this.<async>__2;
                    this.$PC = 6;
                    goto Label_045B;

                case 6:
                    LoadingScreen.Operations.Clear();
                    LoadingScreen.Update("loading shared");
                    this.<async>__3 = Application.LoadLevelAdditiveAsync("LevelShared");
                    LoadingScreen.Operations.Add(this.<async>__3);
                    this.$current = this.<async>__3;
                    this.$PC = 7;
                    goto Label_045B;

                case 7:
                    LoadingScreen.Operations.Clear();
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 8;
                    goto Label_045B;

                case 8:
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 9;
                    goto Label_045B;

                case 9:
                    RustLevel.LevelLoadLog(4);
                    NetCull.isMessageQueueRunning = true;
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 10;
                    goto Label_045B;

                case 10:
                    LoadingScreen.Update("growing trees");
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 11;
                    goto Label_045B;

                case 11:
                    LoadingScreen.Operations.AddMultiple<IProgress>(ThrottledTask.AllWorkingTasksProgress);
                    goto Label_032E;

                case 12:
                    goto Label_032E;

                case 13:
                    if (<>f__am$cacheA == null)
                    {
                        <>f__am$cacheA = new Func<bool>(RustLevel.<LoadRoutine>c__Iterator32.<>m__10);
                    }
                    this.$current = RustLevel.WaitForCondition(this.script, <>f__am$cacheA, "ServerManagement.Get()");
                    this.$PC = 14;
                    goto Label_045B;

                case 14:
                    RustLevel.LevelLoadLog(8);
                    LoadingScreen.Update("becoming ready");
                    ServerManagement.Get().LocalClientPoliteReady();
                    LoadingScreen.Update("waiting for character");
                    if (<>f__am$cacheB == null)
                    {
                        <>f__am$cacheB = new Func<bool>(RustLevel.<LoadRoutine>c__Iterator32.<>m__11);
                    }
                    this.$current = RustLevel.WaitForCondition(this.script, <>f__am$cacheB, "Controllable.localPlayerControllableExists == true");
                    this.$PC = 15;
                    goto Label_045B;

                case 15:
                    RustLevel.LevelLoadLog(9);
                    ConsoleSystem.Run("gameui.hide", false);
                    LoadingScreen.Update("finished");
                    HudEnabled.Enable();
                    RustLevel.LevelLoadLog(10);
                    Globals.isLoading = false;
                    Object.Destroy(this.script.gameObject);
                    this.$PC = -1;
                    goto Label_0459;

                default:
                    goto Label_0459;
            }
            if (this.<async>__0.progress < 0.9f)
            {
                this.$current = new WaitForSeconds(0.2f);
                this.$PC = 1;
            }
            else
            {
                LoadingScreen.Operations.Clear();
                LoadingScreen.Update("activating " + this.levelName);
                this.$current = new WaitForSeconds(0.2f);
                this.$PC = 2;
            }
            goto Label_045B;
        Label_01D3:
            this.$current = new WaitForEndOfFrame();
            this.$PC = 5;
            goto Label_045B;
        Label_032E:
            if (ThrottledTask.Operational)
            {
                this.$current = new WaitForSeconds(0.1f);
                this.$PC = 12;
            }
            else
            {
                LoadingScreen.Operations.Clear();
                RustLevel.LevelLoadLog(7);
                LoadingScreen.Update("waiting for server");
                this.$current = new WaitForSeconds(0.1f);
                this.$PC = 13;
            }
            goto Label_045B;
        Label_0459:
            return false;
        Label_045B:
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
    private sealed class <WaitForCondition>c__Iterator33 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Func<bool> <$>condition;
        internal string <$>requestLabel;
        internal ulong <counter>__0;
        internal Func<bool> condition;
        internal string requestLabel;

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
                    if (this.condition())
                    {
                        goto Label_010B;
                    }
                    this.<counter>__0 = 0L;
                    break;

                case 1:
                    if (!this.condition())
                    {
                        break;
                    }
                    this.<counter>__0 += (ulong) 1L;
                    if (this.<counter>__0 > 50L)
                    {
                        Debug.LogWarning(string.Concat(new object[] { "Took ", this.<counter>__0, " additional frame(s) for condition ", this.requestLabel }));
                    }
                    goto Label_010B;

                default:
                    goto Label_0112;
            }
            if (((this.<counter>__0 += ((ulong) 1L)) % ((ulong) 50L)) == 0)
            {
                Debug.LogWarning(string.Concat(new object[] { "condition still not met:", this.requestLabel, " ( ", this.<counter>__0, " frames later )" }));
            }
            this.$current = new WaitForEndOfFrame();
            this.$PC = 1;
            return true;
        Label_010B:
            this.$PC = -1;
        Label_0112:
            return false;
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

