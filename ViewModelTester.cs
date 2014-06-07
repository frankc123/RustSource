using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ViewModelTester : MonoBehaviour
{
    public ViewModel viewModel;

    [DebuggerHidden]
    private IEnumerator Start()
    {
        return new <Start>c__Iterator43 { <>f__this = this };
    }

    [CompilerGenerated]
    private sealed class <Start>c__Iterator43 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ViewModelTester <>f__this;
        internal int <i>__0;

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
                    this.<i>__0 = 0;
                    break;

                case 1:
                    this.<i>__0++;
                    break;

                default:
                    goto Label_0072;
            }
            if (this.<i>__0 < 5)
            {
                this.$current = null;
                this.$PC = 1;
                return true;
            }
            CameraFX.ReplaceViewModel(this.<>f__this.viewModel, false);
            this.$PC = -1;
        Label_0072:
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

