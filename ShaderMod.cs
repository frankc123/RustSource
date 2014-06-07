using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShaderMod : ScriptableObject
{
    public DICT macroDefines;
    public string[] postIncludes;
    public string[] preIncludes;
    public DICT replaceIncludes;
    public DICT replaceQueues;

    public bool Replace(Replacement replacement, string incoming, ref string outgoing)
    {
        DICT dict = this[replacement];
        return ((dict != null) && dict.Replace(replacement, incoming, ref outgoing));
    }

    public DICT this[Replacement replacement]
    {
        get
        {
            switch (replacement)
            {
                case Replacement.Include:
                    return this.replaceIncludes;

                case Replacement.Queue:
                    return this.replaceQueues;

                case Replacement.Define:
                    return this.macroDefines;
            }
            return null;
        }
    }

    [Serializable]
    public class DICT
    {
        public ShaderMod.KV[] keyValues;

        public bool Replace(ShaderMod.Replacement replacement, string incoming, ref string outgoing)
        {
            if (this.keyValues != null)
            {
                if (replacement == ShaderMod.Replacement.Queue)
                {
                    for (int i = 0; i < this.keyValues.Length; i++)
                    {
                        if (ShaderMod.QueueCompare.Equals(this.keyValues[i].key, incoming))
                        {
                            outgoing = this.keyValues[i].value;
                            return true;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < this.keyValues.Length; j++)
                    {
                        if (string.Equals(this.keyValues[j].key, incoming, StringComparison.InvariantCultureIgnoreCase))
                        {
                            outgoing = this.keyValues[j].value;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string this[string key]
        {
            get
            {
                foreach (ShaderMod.KV kv in this.keyValues)
                {
                    if (kv.key == key)
                    {
                        return kv.value;
                    }
                }
                return null;
            }
            set
            {
                int index = -1;
                while (++index < this.keyValues.Length)
                {
                    if (this.keyValues[index].key == key)
                    {
                        if (value == null)
                        {
                            this.keyValues[index] = this.keyValues[this.keyValues.Length - 1];
                            Array.Resize<ShaderMod.KV>(ref this.keyValues, this.keyValues.Length - 1);
                        }
                        else
                        {
                            this.keyValues[index].value = value;
                        }
                    }
                }
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                Array.Resize<ShaderMod.KV>(ref this.keyValues, this.keyValues.Length + 1);
                this.keyValues[this.keyValues.Length - 1] = new ShaderMod.KV(key, value);
            }
        }
    }

    [Serializable]
    public class KV
    {
        public string key;
        public string value;

        public KV()
        {
            this.key = string.Empty;
            this.value = string.Empty;
        }

        public KV(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override int GetHashCode()
        {
            return ((this.key != null) ? this.key.GetHashCode() : 0);
        }
    }

    public static class QueueCompare
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map6;
        public const int kAlphaTest = 0x992;
        public const int kBackground = 0x3e8;
        public const int kDefault = 0x7d0;
        public const int kGeometry = 0x7d0;
        public const int kOverlay = 0xfa0;
        public const int kTransparent = 0xbb8;
        private static readonly char[] signChars = new char[] { '-', '+' };

        public static bool Equals(string queue1, string queue2)
        {
            return (ToInt32(queue1) == ToInt32(queue2));
        }

        public static int ToInt32(string queue)
        {
            int num;
            if ((queue == null) || (queue.Length == 0))
            {
                return 0x7d0;
            }
            int length = queue.IndexOfAny(signChars);
            if (length != -1)
            {
                queue = queue.Substring(0, length);
                num = int.Parse(queue.Substring(length));
            }
            else
            {
                num = 0;
            }
            string key = (queue = queue.Trim()).ToLowerInvariant();
            if (key != null)
            {
                int num3;
                if (<>f__switch$map6 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                    dictionary.Add("geometry", 0);
                    dictionary.Add("alphatest", 1);
                    dictionary.Add("transparent", 2);
                    dictionary.Add("background", 3);
                    dictionary.Add("overlay", 4);
                    <>f__switch$map6 = dictionary;
                }
                if (<>f__switch$map6.TryGetValue(key, out num3))
                {
                    switch (num3)
                    {
                        case 0:
                            return (0x7d0 + num);

                        case 1:
                            return (0x992 + num);

                        case 2:
                            return (0xbb8 + num);

                        case 3:
                            return (0x3e8 + num);

                        case 4:
                            return (0xfa0 + num);
                    }
                }
            }
            return (!int.TryParse(queue, out num) ? 0x7d0 : num);
        }
    }

    public enum Replacement
    {
        Include,
        Queue,
        Define
    }
}

