namespace NGUI.MessageUtil
{
    using System;
    using UnityEngine;

    public static class Boxed
    {
        public static readonly object @false = false;
        public static readonly object int_0 = 0;
        public static readonly object int_1 = 1;
        public static readonly object int_2 = 2;
        public static readonly object key_down = KeyCode.DownArrow;
        public static readonly object key_escape = KeyCode.Escape;
        public static readonly object key_left = KeyCode.LeftArrow;
        public static readonly object key_none = KeyCode.None;
        public static readonly object key_right = KeyCode.RightArrow;
        public static readonly object key_tab = KeyCode.Tab;
        public static readonly object key_up = KeyCode.UpArrow;
        public static readonly object @true = true;

        public static object Box(bool b)
        {
            return (!b ? @false : @true);
        }

        public static object Box(int i)
        {
            switch (i)
            {
                case 0:
                    return int_0;

                case 1:
                    return int_1;

                case 2:
                    return int_2;
            }
            return i;
        }

        public static object Box(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.UpArrow:
                    return key_up;

                case KeyCode.DownArrow:
                    return key_down;

                case KeyCode.RightArrow:
                    return key_right;

                case KeyCode.LeftArrow:
                    return key_left;

                case KeyCode.None:
                    return key_none;

                case KeyCode.Tab:
                    return key_tab;

                case KeyCode.Escape:
                    return key_escape;
            }
            return k;
        }

        public static object Box<T>(T o)
        {
            return o;
        }
    }
}

