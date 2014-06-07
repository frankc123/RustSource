using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public class ContextMessenger : MonoBehaviour, IContextRequestable, IContextRequestableMenu, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    public string[] messageOptions;

    public ContextExecution ContextQuery(Controllable controllable, ulong timestamp)
    {
        return (((this.messageOptions != null) && (this.messageOptions.Length != 0)) ? ContextExecution.Menu : ContextExecution.NotAvailable);
    }

    [DebuggerHidden]
    public IEnumerable<ContextActionPrototype> ContextQueryMenu(Controllable controllable, ulong timestamp)
    {
        return new <ContextQueryMenu>c__Iterator3A { <>f__this = this, $PC = -2 };
    }

    public ContextResponse ContextRespondMenu(Controllable controllable, ContextActionPrototype action, ulong timestamp)
    {
        base.SendMessage(((MessageAction) action).message, controllable);
        return ContextResponse.DoneBreak;
    }

    [CompilerGenerated]
    private sealed class <ContextQueryMenu>c__Iterator3A : IDisposable, IEnumerator, IEnumerable, IEnumerable<ContextActionPrototype>, IEnumerator<ContextActionPrototype>
    {
        internal ContextActionPrototype $current;
        internal int $PC;
        internal string[] <$s_401>__1;
        internal int <$s_402>__2;
        internal ContextMessenger <>f__this;
        internal string <message>__3;
        internal int <name>__0;

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
                    this.<name>__0 = 0;
                    this.<$s_401>__1 = this.<>f__this.messageOptions;
                    this.<$s_402>__2 = 0;
                    break;

                case 1:
                    this.<$s_402>__2++;
                    break;

                default:
                    goto Label_00B4;
            }
            if (this.<$s_402>__2 < this.<$s_401>__1.Length)
            {
                this.<message>__3 = this.<$s_401>__1[this.<$s_402>__2];
                this.$current = new ContextMessenger.MessageAction(this.<name>__0++, this.<message>__3, this.<message>__3);
                this.$PC = 1;
                return true;
            }
            this.$PC = -1;
        Label_00B4:
            return false;
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
            return new ContextMessenger.<ContextQueryMenu>c__Iterator3A { <>f__this = this.<>f__this };
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

    private class MessageAction : ContextActionPrototype
    {
        public string message;

        public MessageAction(int name, string text, string message)
        {
            base.name = name;
            base.text = text;
            this.message = message;
        }
    }
}

