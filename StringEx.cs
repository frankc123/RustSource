using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

public static class StringEx
{
    private static readonly char[] cb = new char[0x400];
    private static readonly object cbLock = new object();
    private static uint lockCount;
    private const int maxLockSize = 0x400;

    private static string c2s(char[] c, int l)
    {
        return ((l != 0) ? new string(c, 0, l) : string.Empty);
    }

    private static string c2s(int l, char[] c)
    {
        return ((l != 0) ? new string(c, 0, l) : string.Empty);
    }

    public static string MakeNice(this string s)
    {
        int num;
        if ((s == null) || ((num = s.Length) <= 1))
        {
            return s;
        }
        int index = -1;
        while (++index < num)
        {
            if (char.IsLetterOrDigit(s, index))
            {
                char[] chArray;
                if (index == (num - 1))
                {
                    return s.Substring(num - 1, 1);
                }
                bool flag = char.IsDigit(s, index);
                bool flag2 = true;
                bool flag3 = true;
                int num3 = 0;
                using (L l = S(s, num - index, (num - (index + 1)) * 2, out chArray))
                {
                    if (!l.V)
                    {
                        return s;
                    }
                    if (!flag)
                    {
                        chArray[num3++] = char.ToUpper(s[index]);
                    }
                    else
                    {
                        chArray[num3++] = s[index];
                    }
                    while (++index < num)
                    {
                        if (flag != char.IsNumber(s, index))
                        {
                            flag = !flag;
                            if (!flag3)
                            {
                                chArray[num3++] = ' ';
                            }
                            else
                            {
                                flag3 = false;
                            }
                            chArray[num3++] = !flag ? char.ToUpperInvariant(s[index]) : s[index];
                            flag2 = true;
                        }
                        else
                        {
                            if (flag)
                            {
                                chArray[num3++] = s[index];
                                continue;
                            }
                            if (char.IsUpper(s, index))
                            {
                                if (!flag2)
                                {
                                    if (!flag3)
                                    {
                                        chArray[num3++] = ' ';
                                    }
                                    else
                                    {
                                        flag3 = false;
                                    }
                                    flag2 = true;
                                }
                                chArray[num3++] = s[index];
                                continue;
                            }
                            if (char.IsLower(s, index))
                            {
                                chArray[num3++] = s[index];
                                flag2 = false;
                            }
                            else if (!flag3)
                            {
                                chArray[num3++] = ' ';
                                flag3 = true;
                            }
                        }
                    }
                    return c2s(chArray, !flag3 ? num3 : (num3 - 1));
                }
            }
        }
        return string.Empty;
    }

    [Obsolete("You gotta specify at least one char", true)]
    public static string RemoveChars(this string s)
    {
        return s;
    }

    public static string RemoveChars(this string s, params char[] rem)
    {
        int length = rem.Length;
        if (length != 0)
        {
            int num2 = (s != null) ? s.Length : 0;
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (s[i] == rem[j])
                    {
                        char[] chArray;
                        if (i == (num2 - 1))
                        {
                            return s.Remove(num2 - 1);
                        }
                        using (L l = S(s, num2, out chArray))
                        {
                            if (!l.V)
                            {
                                return s;
                            }
                            int num5 = i;
                            while (++i < num2)
                            {
                                for (j = 0; j < length; j++)
                                {
                                    if (rem[j] == chArray[i])
                                    {
                                    }
                                }
                                chArray[num5++] = chArray[i];
                            }
                            return c2s(chArray, num5);
                        }
                    }
                }
            }
        }
        return s;
    }

    public static string RemoveChars(this string s, char rem)
    {
        int num = (s != null) ? s.Length : 0;
        for (int i = 0; i < num; i++)
        {
            if (s[i] == rem)
            {
                char[] chArray;
                if (i == (num - 1))
                {
                    return s.Remove(num - 1);
                }
                using (L l = S(s, num, out chArray))
                {
                    if (!l.V)
                    {
                        return s;
                    }
                    int num3 = i;
                    while (++i < num)
                    {
                        if (chArray[i] != rem)
                        {
                            chArray[num3++] = chArray[i];
                        }
                    }
                    return c2s(chArray, num3);
                }
            }
        }
        return s;
    }

    public static string RemoveWhiteSpaces(this string s)
    {
        int num = (s != null) ? s.Length : 0;
        for (int i = 0; i < num; i++)
        {
            if (char.IsWhiteSpace(s[i]))
            {
                char[] chArray;
                if (i == (num - 1))
                {
                    return s.Remove(num - 1);
                }
                using (L l = S(s, num, out chArray))
                {
                    if (!l.V)
                    {
                        return s;
                    }
                    int num3 = i;
                    while (++i < num)
                    {
                        if (!char.IsWhiteSpace(chArray[i]))
                        {
                            chArray[num3++] = chArray[i];
                        }
                    }
                    return c2s(chArray, num3);
                }
            }
        }
        return s;
    }

    private static L S(string s, int l, out char[] buffer)
    {
        if ((s == null) || (l <= 0))
        {
            buffer = null;
            return new L();
        }
        L l2 = new L(l <= 0x400);
        if (l2.locked)
        {
            char[] chArray;
            buffer = chArray = cb;
            s.CopyTo(0, chArray, 0, l);
            return l2;
        }
        buffer = s.ToCharArray();
        return l2;
    }

    private static L S(string s, int l, int minSafeSize, out char[] buffer)
    {
        if ((s == null) || (l <= 0))
        {
            buffer = null;
            return new L();
        }
        L l2 = new L(minSafeSize <= 0x400);
        if (l2.locked)
        {
            char[] chArray;
            buffer = chArray = cb;
            s.CopyTo(0, chArray, 0, l);
            return l2;
        }
        buffer = s.ToCharArray();
        return l2;
    }

    public static string ToLowerEx(this string s)
    {
        int num = (s != null) ? s.Length : 0;
        for (int i = 0; i < num; i++)
        {
            if (char.IsUpper(s, i))
            {
                char[] chArray;
                using (L l = S(s, num, out chArray))
                {
                    if (!l.V)
                    {
                        return s;
                    }
                    do
                    {
                        chArray[i] = char.ToLowerInvariant(chArray[i]);
                    }
                    while (++i < num);
                    return c2s(chArray, num);
                }
            }
        }
        return s;
    }

    public static string ToUpperEx(this string s)
    {
        int num = (s != null) ? s.Length : 0;
        for (int i = 0; i < num; i++)
        {
            if (char.IsLower(s, i))
            {
                char[] chArray;
                using (L l = S(s, num, out chArray))
                {
                    if (!l.V)
                    {
                        return s;
                    }
                    do
                    {
                        chArray[i] = char.ToUpperInvariant(chArray[i]);
                    }
                    while (++i < num);
                    return c2s(chArray, num);
                }
            }
        }
        return s;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct L : IDisposable
    {
        private bool _locked;
        private bool _valid;
        public L(bool locked)
        {
            this._locked = locked && Monitor.TryEnter(StringEx.cbLock);
            this._valid = true;
        }

        public bool locked
        {
            get
            {
                return this._locked;
            }
        }
        public bool V
        {
            get
            {
                return this._valid;
            }
        }
        public void Dispose()
        {
            if (this._locked)
            {
                Monitor.Exit(StringEx.cbLock);
                this._locked = false;
            }
        }
    }
}

