using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class UITextMarkupUtility
{
    [CompilerGenerated]
    private static Comparison<UITextMarkup> <>f__am$cache0;

    public static string MarkUp(this List<UITextMarkup> list, string input)
    {
        int num;
        if ((list == null) || ((num = list.Count) == 0))
        {
            return input;
        }
        UITextMarkup markup2 = list[0];
        int index = markup2.index;
        StringBuilder builder = new StringBuilder(input, 0, index, input.Length + num);
        int num3 = 0;
        UITextMarkup markup = list[num3];
        for (int i = markup.index; i < input.Length; i++)
        {
            char ch = input[i];
            if (i != markup.index)
            {
                goto Label_0176;
            }
        Label_0068:
            switch (markup.mod)
            {
                case UITextMod.End:
                    i = input.Length + 1;
                    ch = '\0';
                    break;

                case UITextMod.Removed:
                    ch = '\0';
                    break;

                case UITextMod.Replaced:
                    builder.Append(markup.value);
                    ch = '\0';
                    break;

                case UITextMod.Added:
                    builder.Append(markup.value);
                    break;
            }
            if (++num3 == num)
            {
                if (i >= input.Length)
                {
                    goto Label_0153;
                }
                if (ch != '\0')
                {
                    builder.Append(input, i, input.Length - i);
                }
                else
                {
                    if (++i >= input.Length)
                    {
                        goto Label_0161;
                    }
                    builder.Append(input, i, input.Length - i);
                }
                i = input.Length + 1;
                goto Label_0161;
            }
            markup = list[num3];
        Label_0153:
            if (markup.index == i)
            {
                goto Label_0068;
            }
        Label_0161:
            if (ch != '\0')
            {
                builder.Append(ch);
            }
            continue;
        Label_0176:
            if (ch != '\0')
            {
                builder.Append(ch);
            }
        }
        while (++num3 < num)
        {
            switch (markup.mod)
            {
                case UITextMod.End:
                {
                    continue;
                }
                case UITextMod.Added:
                {
                    builder.Append(markup.value);
                    continue;
                }
            }
            Debug.Log("Unsupported end markup " + markup);
        }
        return builder.ToString();
    }

    public static void SortMarkup(this List<UITextMarkup> list)
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = delegate (UITextMarkup x, UITextMarkup y) {
                int num = x.index.CompareTo(y.index);
                return (num != 0) ? num : ((byte) x.mod).CompareTo((byte) y.mod);
            };
        }
        list.Sort(<>f__am$cache0);
    }
}

