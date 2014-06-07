using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct UIHighlight
{
    public Node a;
    public Node b;
    public static UIHighlight invalid
    {
        get
        {
            UIHighlight highlight = new UIHighlight();
            Node node = new Node {
                i = -1
            };
            highlight.a = node;
            Node node2 = new Node {
                i = -1
            };
            highlight.b = node2;
            return highlight;
        }
    }
    public int lineCount
    {
        get
        {
            return ((this.a.i == this.b.i) ? 0 : ((this.b.L - this.a.L) + 1));
        }
    }
    public bool empty
    {
        get
        {
            return (this.a.i == this.b.i);
        }
    }
    public bool any
    {
        get
        {
            return (this.a.i != this.b.i);
        }
    }
    public int characterCount
    {
        get
        {
            return (this.b.i - this.a.i);
        }
    }
    public int lineSpan
    {
        get
        {
            return (this.b.L - this.a.L);
        }
    }
    public Node delta
    {
        get
        {
            Node node;
            node.i = this.b.i - this.a.i;
            node.L = this.b.L - this.a.L;
            node.C = this.b.C - this.a.C;
            return node;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Node
    {
        public int i;
        public int L;
        public int C;
        public override string ToString()
        {
            return string.Format("[{0}({1}:{2})]", this.i, this.L, this.C);
        }
    }
}

