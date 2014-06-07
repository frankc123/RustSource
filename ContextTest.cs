using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class ContextTest : MonoBehaviour, IContextRequestable, IContextRequestableMenu, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    public ContextExecution ContextQuery(Controllable controllable, ulong timestamp)
    {
        return (ContextExecution.Menu | ContextExecution.Quick);
    }

    [DebuggerHidden]
    public IEnumerable<ContextActionPrototype> ContextQueryMenu(Controllable controllable, ulong timestamp)
    {
        return new <ContextQueryMenu>c__Iterator39 { <>f__this = this, $PC = -2 };
    }

    public ContextResponse ContextRespondMenu(Controllable controllable, ContextActionPrototype action, ulong timestamp)
    {
        ContextCallback callback = (ContextCallback) action;
        return callback.func(controllable);
    }

    private ContextResponse Option1(Controllable control)
    {
        Debug.Log("Wee option 1");
        return ContextResponse.DoneBreak;
    }

    private ContextResponse Option2(Controllable control)
    {
        Debug.Log("Wee option 2");
        return ContextResponse.DoneBreak;
    }

    [CompilerGenerated]
    private sealed class <ContextQueryMenu>c__Iterator39 : IDisposable, IEnumerator, IEnumerable, IEnumerable<ContextActionPrototype>, IEnumerator<ContextActionPrototype>
    {
        internal ContextActionPrototype $current;
        internal int $PC;
        internal ContextTest <>f__this;

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
                    this.$current = new ContextTest.ContextCallback(0, "Option1", new ContextTest.CallbackFunction(this.<>f__this.Option1));
                    this.$PC = 1;
                    goto Label_008A;

                case 1:
                    this.$current = new ContextTest.ContextCallback(1, "Option2", new ContextTest.CallbackFunction(this.<>f__this.Option2));
                    this.$PC = 2;
                    goto Label_008A;

                case 2:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_008A:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<ContextActionPrototype> IEnumerable<ContextActionPrototype>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ContextTest.<ContextQueryMenu>c__Iterator39 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<ContextActionPrototype>.GetEnumerator();
        }

        ContextActionPrototype IEnumerator<ContextActionPrototype>.Current
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

    private delegate ContextResponse CallbackFunction(Controllable controllable);

    private class ContextCallback : ContextActionPrototype
    {
        public ContextTest.CallbackFunction func;

        public ContextCallback(int name, string text, ContextTest.CallbackFunction function)
        {
            base.name = name;
            base.text = text;
            this.func = function;
        }
    }
}

