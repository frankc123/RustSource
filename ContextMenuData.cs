using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;

internal class ContextMenuData
{
    private static readonly BitStreamCodec.Deserializer deserializer = new BitStreamCodec.Deserializer(ContextMenuData.Deserialize);
    [NonSerialized]
    public readonly ContextMenuItemData[] options;
    [NonSerialized]
    public readonly int options_length;
    private static readonly BitStreamCodec.Serializer serializer = new BitStreamCodec.Serializer(ContextMenuData.Serialize);

    static ContextMenuData()
    {
        BitStreamCodec.Add<ContextMenuData>(deserializer, serializer);
    }

    public ContextMenuData(IEnumerable<ContextActionPrototype> prototypeEnumerable)
    {
        if (prototypeEnumerable is ICollection<ContextActionPrototype>)
        {
            this.options = new ContextMenuItemData[((ICollection<ContextActionPrototype>) prototypeEnumerable).Count];
            int num = 0;
            using (IEnumerator<ContextActionPrototype> enumerator = prototypeEnumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    this.options[num++] = new ContextMenuItemData(enumerator.Current);
                }
            }
            if (num < this.options.Length)
            {
                Array.Resize<ContextMenuItemData>(ref this.options, this.options.Length);
            }
            this.options_length = this.options.Length;
        }
        else
        {
            this.options = ToArray(prototypeEnumerable, out this.options_length);
        }
    }

    public ContextMenuData(int optionCount, ContextMenuItemData[] data)
    {
        this.options_length = optionCount;
        this.options = data;
    }

    private static object Deserialize(BitStream stream, params object[] codecOptions)
    {
        int optionCount = stream.Read<int>(codecOptions);
        ContextMenuItemData[] data = (optionCount != 0) ? new ContextMenuItemData[optionCount] : null;
        for (int i = 0; i < optionCount; i++)
        {
            byte[] buffer;
            int num4;
            int name = stream.Read<int>(codecOptions);
            stream.ReadByteArray_MinimalCalls(out buffer, out num4, codecOptions);
            data[i] = new ContextMenuItemData(name, num4, buffer);
        }
        return new ContextMenuData(optionCount, data);
    }

    private static void Serialize(BitStream stream, object value, params object[] codecOptions)
    {
        ContextMenuData data = (ContextMenuData) value;
        stream.Write<int>(data.options_length, codecOptions);
        for (int i = 0; i < data.options_length; i++)
        {
            stream.Write<int>(data.options[i].name, codecOptions);
            stream.WriteByteArray_MinimumCalls(data.options[i].utf8_text, 0, data.options[i].utf8_length, codecOptions);
        }
    }

    private static ContextMenuItemData[] ToArray(IEnumerable<ContextActionPrototype> enumerable, out int length)
    {
        using (enumerable.GetEnumerator())
        {
            EnumerableConverter converter = new EnumerableConverter {
                enumerator = enumerable.GetEnumerator()
            };
            converter.R();
            length = converter.length;
            return converter.array;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct EnumerableConverter
    {
        public IEnumerator<ContextActionPrototype> enumerator;
        public int length;
        public int spot;
        public ContextMenuItemData[] array;
        public void R()
        {
            if (this.enumerator.MoveNext())
            {
                this.length++;
                ContextActionPrototype current = this.enumerator.Current;
                this.R();
                this.array[--this.spot] = new ContextMenuItemData(current);
            }
            else if (this.length == 0)
            {
                this.array = null;
            }
            else
            {
                this.array = new ContextMenuItemData[this.length];
                this.spot = this.length;
            }
        }
    }
}

