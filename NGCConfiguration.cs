using Facepunch;
using Facepunch.Build;
using Facepunch.Hash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

[UniqueBundleScriptableObject]
public class NGCConfiguration : ScriptableObject
{
    private const string bundledPath = "content/network/NGCConf";
    [SerializeField]
    private PrefabEntry[] entries;

    public void Install()
    {
        foreach (PrefabEntry entry in this.entries)
        {
            if ((entry != null) && entry.ReadyToRegister)
            {
                NGC.Prefab.Register.Add(entry.Path, entry.HashCode, ";" + entry.Name);
            }
        }
    }

    public static NGCConfiguration Load()
    {
        return Bundling.Load<NGCConfiguration>("content/network/NGCConf");
    }

    protected void OnEnable()
    {
        if (this.entries == null)
        {
            this.entries = new PrefabEntry[0];
        }
        else
        {
            HashSet<string> set = new HashSet<string>();
            int newSize = 0;
            for (int i = 0; i < this.entries.Length; i++)
            {
                if (this.entries[i] != null)
                {
                    if (!set.Add(this.entries[i].Name))
                    {
                        Debug.LogWarning(string.Format("Removing duplicate ngc prefab named '{0}' (path:{1})", this.entries[i].Name, this.entries[i].Path));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(this.entries[i].Path))
                        {
                            Debug.LogWarning(string.Format("ngc prefab {0} has no path!", this.entries[i].Name), this);
                        }
                        this.entries[newSize++] = this.entries[i];
                    }
                }
            }
            if (newSize < this.entries.Length)
            {
                Array.Resize<PrefabEntry>(ref this.entries, newSize);
                Debug.LogWarning("The entries of the ngcconfiguration were altered!", this);
            }
        }
    }

    [Serializable]
    public sealed class PrefabEntry
    {
        [NonSerialized]
        private int _hashCode;
        [NonSerialized]
        private bool calculatedHashCode;
        [SerializeField]
        private string guidText = string.Empty;
        [SerializeField]
        private string name = "!unnamed";
        [SerializeField]
        private string path = string.Empty;
        private const uint peSeed = 0x86c08f16;

        public override int GetHashCode()
        {
            return (!this.calculatedHashCode ? (this._hashCode = hash(this.guidText)) : this._hashCode);
        }

        private static int hash(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return 0;
            }
            int current = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            using (IEnumerator<int> enumerator = ParseInts(guid))
            {
                if (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (enumerator.MoveNext())
                    {
                        num2 = enumerator.Current;
                        if (enumerator.MoveNext())
                        {
                            num3 = enumerator.Current;
                            if (enumerator.MoveNext())
                            {
                                num4 = enumerator.Current;
                                if (enumerator.MoveNext())
                                {
                                    num5 = enumerator.Current;
                                    if (enumerator.MoveNext())
                                    {
                                        num6 = enumerator.Current;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            hashwork.guid[0] = current;
            hashwork.guid[1] = num6;
            hashwork.guid[2] = num5;
            hashwork.guid[3] = num3;
            hashwork.guid[4] = num4;
            hashwork.guid[5] = num2;
            return MurmurHash2.SINT(hashwork.guid, hashwork.guid.Length, 0x86c08f16);
        }

        [DebuggerHidden]
        private static IEnumerator<int> ParseInts(string hex)
        {
            return new <ParseInts>c__Iterator2B { hex = hex, <$>hex = hex };
        }

        public override string ToString()
        {
            return string.Format("[PrefabEntry: Name=\"{1}\", HashCode={0:X}, Path=\"{2}\"]", this.HashCode, this.Name, this.Path);
        }

        public int HashCode
        {
            get
            {
                return (!this.calculatedHashCode ? (this._hashCode = hash(this.guidText)) : this._hashCode);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public bool ReadyToRegister
        {
            get
            {
                return ((!string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.Path)) && (this.HashCode != 0));
            }
        }

        [CompilerGenerated]
        private sealed class <ParseInts>c__Iterator2B : IDisposable, IEnumerator, IEnumerator<int>
        {
            internal int $current;
            internal int $PC;
            internal string <$>hex;
            internal int <start>__0;
            internal string hex;

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
                        this.<start>__0 = this.hex.Length;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00BC;

                    default:
                        goto Label_00C3;
                }
                if (this.<start>__0 >= 8)
                {
                    this.<start>__0 -= 8;
                    this.$current = int.Parse(this.hex.Substring(this.<start>__0, 8), NumberStyles.HexNumber);
                    this.$PC = 1;
                    goto Label_00C5;
                }
                if (this.<start>__0 > 0)
                {
                    this.$current = int.Parse(this.hex.Remove(this.<start>__0), NumberStyles.HexNumber);
                    this.$PC = 2;
                    goto Label_00C5;
                }
            Label_00BC:
                this.$PC = -1;
            Label_00C3:
                return false;
            Label_00C5:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            int IEnumerator<int>.Current
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

        private static class hashwork
        {
            public static readonly int[] guid = new int[6];
        }
    }
}

