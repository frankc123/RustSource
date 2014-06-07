using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RPOSWindowScrollable : RPOSWindow
{
    protected bool autoResetScrolling = true;
    private bool cancelCalculationNextFrame;
    private bool didManualStart;
    public bool horizontal;
    protected Vector2 initialScrollValue;
    public UIDraggablePanel myDraggablePanel;
    private bool queuedCalculationNextFrame;
    public bool vertical = true;

    protected void NextFrameRecalculateBounds()
    {
        this.cancelCalculationNextFrame = false;
        if (!this.queuedCalculationNextFrame)
        {
            base.StartCoroutine(this.Routine_NextFrameRecalculateBounds());
        }
    }

    protected override void OnWindowShow()
    {
        base.OnWindowShow();
        if (this.autoResetScrolling)
        {
            this.ResetScrolling();
        }
    }

    protected void ResetScrolling()
    {
        this.ResetScrolling(false);
    }

    protected virtual void ResetScrolling(bool retainCurrentValue)
    {
        UIScrollBar bar = null;
        UIScrollBar bar2 = null;
        if (this.myDraggablePanel != null)
        {
            if (!retainCurrentValue)
            {
                bar = !this.vertical ? null : this.myDraggablePanel.verticalScrollBar;
                bar2 = !this.horizontal ? null : this.myDraggablePanel.horizontalScrollBar;
            }
            if (!this.didManualStart)
            {
                this.myDraggablePanel.ManualStart();
                this.didManualStart = true;
            }
            this.myDraggablePanel.calculateBoundsEveryChange = false;
            this.NextFrameRecalculateBounds();
        }
        else if (!retainCurrentValue)
        {
            bar = (!this.vertical || this.horizontal) ? null : base.GetComponentInChildren<UIScrollBar>();
            bar2 = (!this.horizontal || this.vertical) ? null : base.GetComponentInChildren<UIScrollBar>();
        }
        if (!retainCurrentValue)
        {
            if (this.vertical && (bar != null))
            {
                bar.scrollValue = this.initialScrollValue.y;
                bar.ForceUpdate();
            }
            if (this.horizontal && (bar2 != null))
            {
                bar2.scrollValue = this.initialScrollValue.x;
                bar2.ForceUpdate();
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator Routine_NextFrameRecalculateBounds()
    {
        return new <Routine_NextFrameRecalculateBounds>c__Iterator36 { <>f__this = this };
    }

    [CompilerGenerated]
    private sealed class <Routine_NextFrameRecalculateBounds>c__Iterator36 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RPOSWindowScrollable <>f__this;

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
                    return true;

                case 1:
                    this.<>f__this.queuedCalculationNextFrame = false;
                    if (!this.<>f__this.cancelCalculationNextFrame && (this.<>f__this.myDraggablePanel != null))
                    {
                        this.<>f__this.myDraggablePanel.CalculateBoundsIfNeeded();
                    }
                    this.$PC = -1;
                    break;
            }
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

