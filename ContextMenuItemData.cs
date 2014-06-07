using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct ContextMenuItemData
{
    public readonly int name;
    public readonly int utf8_length;
    public readonly byte[] utf8_text;
    public ContextMenuItemData(int name, int utf8_length, byte[] utf8_text)
    {
        this.name = name;
        this.utf8_length = utf8_length;
        this.utf8_text = utf8_text;
    }

    public ContextMenuItemData(ContextActionPrototype prototype)
    {
        this.name = prototype.name;
        string text = prototype.text;
        if (string.IsNullOrEmpty(text))
        {
            this.utf8_length = 0;
            this.utf8_text = null;
        }
        else
        {
            this.utf8_text = Encoding.UTF8.GetBytes(text);
            this.utf8_length = this.utf8_text.Length;
        }
    }
}

