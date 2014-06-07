namespace Facepunch.Load
{
    using Facepunch;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class Utility
    {
        public static string GetBuildInvariantTypeName(this Type type)
        {
            string fullName = type.Assembly.FullName;
            int index = fullName.IndexOf(',');
            if (index != -1)
            {
                fullName = fullName.Substring(0, index);
            }
            return (type.FullName + ", " + fullName);
        }

        public sealed class ReferenceCountedCoroutine : IEnumerator
        {
            private readonly Callback callback;
            private readonly Runner runner;
            private bool skipOnce;
            private object tag;
            private object yieldInstruction;

            private ReferenceCountedCoroutine(Runner runner, Callback callback, object yieldInstruction, object tag, bool skipOnce)
            {
                this.runner = runner;
                this.callback = callback;
                this.yieldInstruction = yieldInstruction;
                this.tag = tag;
                this.skipOnce = skipOnce;
            }

            bool IEnumerator.MoveNext()
            {
                bool flag;
                if (this.skipOnce)
                {
                    this.skipOnce = false;
                    return true;
                }
                try
                {
                    flag = this.callback(ref this.yieldInstruction, ref this.tag);
                }
                catch (Exception exception)
                {
                    flag = false;
                    Debug.LogException(exception);
                }
                if (!flag)
                {
                    this.runner.Release();
                    this.tag = null;
                    this.yieldInstruction = null;
                    return false;
                }
                return true;
            }

            void IEnumerator.Reset()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.yieldInstruction;
                }
            }

            public delegate bool Callback(ref object yieldInstruction, ref object tag);

            public sealed class Runner
            {
                private readonly string gameObjectName;
                private GameObject go;
                private int refCount;
                private MonoBehaviour script;

                public Runner(string gameObjectName)
                {
                    this.gameObjectName = gameObjectName;
                }

                public Coroutine Install(Utility.ReferenceCountedCoroutine.Callback callback, object tag, object defaultYieldInstruction, bool skipFirst)
                {
                    this.Retain();
                    return this.script.StartCoroutine(new Utility.ReferenceCountedCoroutine(this, callback, defaultYieldInstruction, tag, skipFirst));
                }

                public void Release()
                {
                    if (--this.refCount == 0)
                    {
                        Object.Destroy(this.go);
                        Object.Destroy(this.script);
                        this.go = null;
                        this.script = null;
                    }
                }

                public void Retain()
                {
                    if (this.refCount++ == 0)
                    {
                        Type[] components = new Type[] { typeof(MonoBehaviour) };
                        this.go = new GameObject(this.gameObjectName, components);
                        Object.DontDestroyOnLoad(this.go);
                        this.script = this.go.GetComponent<MonoBehaviour>();
                    }
                }
            }
        }
    }
}

