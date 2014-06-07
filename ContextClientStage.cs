using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct ContextClientStage
{
    [NonSerialized]
    public ContextClientStageMenuItem[] option;
    [NonSerialized]
    public int length;
    public void Set(ContextMenuData data)
    {
        if (this.length < data.options_length)
        {
            this.option = new ContextClientStageMenuItem[data.options_length];
            this.length = data.options_length;
        }
        else
        {
            while (this.length > data.options_length)
            {
                this.option[--this.length].text = null;
            }
        }
        for (int i = 0; i < data.options_length; i++)
        {
            this.option[i].name = data.options[i].name;
            if (data.options[i].utf8_length == 0)
            {
                this.option[i].text = string.Empty;
            }
            else
            {
                this.option[i].text = Encoding.UTF8.GetString(data.options[i].utf8_text, 0, data.options[i].utf8_length);
            }
        }
    }
}

