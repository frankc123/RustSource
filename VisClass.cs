using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class VisClass : ScriptableObject
{
    [SerializeField]
    private VisClass _super;
    [NonSerialized]
    private VisQuery[] instance;
    [SerializeField]
    private string[] keys;
    [NonSerialized]
    private bool locked;
    [NonSerialized]
    private Dictionary<string, int> members;
    private static readonly VisQuery.Instance[] none = new VisQuery.Instance[0];
    [NonSerialized]
    private bool recurseLock;
    [SerializeField]
    private VisQuery[] values;

    private void BuildMembers(List<VisQuery> list, HashSet<VisQuery> hset)
    {
        if (this._super != null)
        {
            if (this._super.recurseLock)
            {
                Debug.LogError("Recursion in setup hit itself, some VisClass has super set to something which references itself", this._super);
                return;
            }
            this._super.recurseLock = true;
            this._super.BuildMembers(list, hset);
            this._super.recurseLock = false;
        }
        if (this.values != null)
        {
            for (int i = 0; i < this.values.Length; i++)
            {
                if ((this.values[i] != null) && hset.Remove(this.values[i]))
                {
                    list.Add(this.values[i]);
                }
            }
        }
    }

    public void EditorOnly_Add(ref Rep rep, string key, VisQuery value)
    {
        Array.Resize<string>(ref this.keys, this.keys.Length + 1);
        Array.Resize<VisQuery>(ref this.values, this.values.Length + 1);
        this.keys[this.keys.Length - 1] = key;
        this.values[this.values.Length - 1] = value;
        rep = null;
    }

    public bool EditorOnly_Apply(ref Rep rep)
    {
        return ((rep != null) && rep.Apply());
    }

    public void EditorOnly_Rep(ref Rep rep)
    {
        if ((this.keys == null) && (this.values == null))
        {
            this.keys = new string[0];
            this.values = new VisQuery[0];
        }
        Rep.Ref(ref rep, this);
    }

    public bool EditorOnly_SetSuper(ref Rep rep, VisClass _super)
    {
        VisClass class2 = _super;
        int num = 50;
        while (class2 != null)
        {
            if (class2 == this)
            {
                Debug.LogError("Self Reference Detected", this);
                return false;
            }
            class2 = class2._super;
            if (--num <= 0)
            {
                Debug.LogError("Circular Dependancy Detected", this);
                return false;
            }
        }
        rep = null;
        this._super = _super;
        return true;
    }

    private void Setup()
    {
        if (!this.locked)
        {
            if (this.recurseLock)
            {
                Debug.LogError("Recursion in setup hit itself, some VisClass has super set to something which references itself", this);
            }
            else
            {
                this.recurseLock = true;
                List<VisQuery> list = new List<VisQuery>();
                HashSet<VisQuery> hset = new HashSet<VisQuery>();
                Dictionary<string, VisQuery> dictionary = new Dictionary<string, VisQuery>();
                if (this._super != null)
                {
                    this._super.Setup();
                    if (this.keys != null)
                    {
                        for (int i = 0; i < this.keys.Length; i++)
                        {
                            string str = this.keys[i];
                            if (!string.IsNullOrEmpty(str))
                            {
                                int num2;
                                VisQuery query = this.values[i];
                                if (this._super.members.TryGetValue(str, out num2))
                                {
                                    VisQuery item = this._super.instance[num2];
                                    if (item == query)
                                    {
                                        if (item != null)
                                        {
                                            hset.Add(item);
                                            dictionary.Add(str, item);
                                        }
                                    }
                                    else if (query != null)
                                    {
                                        dictionary.Add(str, query);
                                        hset.Add(query);
                                    }
                                }
                                else if (query != null)
                                {
                                    dictionary.Add(str, query);
                                    hset.Add(query);
                                }
                            }
                        }
                    }
                    this.BuildMembers(list, hset);
                }
                else
                {
                    for (int j = 0; j < this.keys.Length; j++)
                    {
                        string str2 = this.keys[j];
                        if (!string.IsNullOrEmpty(str2))
                        {
                            VisQuery query3 = this.values[j];
                            if (query3 != null)
                            {
                                dictionary.Add(str2, query3);
                                if (hset.Add(query3))
                                {
                                    list.Add(query3);
                                }
                            }
                        }
                    }
                }
                this.members = new Dictionary<string, int>(dictionary.Count);
                foreach (KeyValuePair<string, VisQuery> pair in dictionary)
                {
                    this.members.Add(pair.Key, list.IndexOf(pair.Value));
                }
                this.instance = list.ToArray();
                this.recurseLock = false;
                this.locked = true;
            }
        }
    }

    public Handle handle
    {
        get
        {
            if (!this.locked)
            {
                this.Setup();
                if (!this.locked)
                {
                    return new Handle(null);
                }
            }
            return new Handle(this);
        }
    }

    public VisClass superClass
    {
        get
        {
            return this._super;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Handle
    {
        private readonly VisClass klass;
        private readonly VisQuery.Instance[] queries;
        private long bits;
        internal Handle(VisClass klass)
        {
            this.klass = klass;
            this.bits = 0L;
            if (klass != null)
            {
                int bit = 0;
                this.queries = new VisQuery.Instance[klass.instance.Length];
                for (int i = 0; i < this.queries.Length; i++)
                {
                    this.queries[i] = new VisQuery.Instance(klass.instance[i], ref bit);
                }
            }
            else
            {
                this.queries = VisClass.none;
            }
        }

        public bool valid
        {
            get
            {
                return (this.queries != null);
            }
        }
        public int Length
        {
            get
            {
                return this.klass.instance.Length;
            }
        }
        public VisQuery.Instance this[int i]
        {
            get
            {
                return this.queries[i];
            }
        }
        public VisQuery.Instance this[string name]
        {
            get
            {
                return this.queries[this.klass.members[name]];
            }
        }
    }

    public class Rep
    {
        private static bool building;
        public Dictionary<string, Setting> dict;
        internal VisClass klass;
        private HashSet<Setting> modifiedSettings = new HashSet<Setting>();
        internal static VisClass nklass;

        internal bool Apply()
        {
            if (this.modifiedSettings.Count == 0)
            {
                return false;
            }
            foreach (Setting setting in this.modifiedSettings)
            {
                switch (setting.action)
                {
                    case Action.Revert:
                        this.Remove(setting);
                        break;

                    case Action.Value:
                        if ((setting.valueSet == null) && !setting.isOverride)
                        {
                            this.Remove(setting);
                        }
                        else
                        {
                            this.Change(setting);
                        }
                        break;
                }
                setting.action = Action.None;
            }
            return true;
        }

        private void Change(Setting setting)
        {
            if (setting.isInherited)
            {
                VisQuery valueSet = setting.valueSet;
                this.dict[setting.name] = setting = setting.Override(this.klass);
                setting.isInherited = false;
                setting.valueSet = valueSet;
                Array.Resize<string>(ref this.klass.keys, this.klass.keys.Length + 1);
                Array.Resize<VisQuery>(ref this.klass.values, this.klass.values.Length + 1);
                this.klass.keys[this.klass.keys.Length - 1] = setting.name;
                this.klass.values[this.klass.values.Length - 1] = valueSet;
            }
            else
            {
                for (int i = 0; i < this.klass.keys.Length; i++)
                {
                    if (this.klass.keys[i] == setting.name)
                    {
                        this.klass.values[i] = setting.query;
                        break;
                    }
                }
            }
        }

        private static bool MarkModified(Setting setting)
        {
            if (building)
            {
                return false;
            }
            setting.rep.modifiedSettings.Add(setting);
            return true;
        }

        internal static void Recur(ref VisClass.Rep rep, VisClass klass)
        {
            if (klass._super != null)
            {
                Recur(ref rep, klass._super);
                foreach (Setting setting in rep.dict.Values)
                {
                    setting.isInherited = true;
                }
                for (int i = 0; i < klass.keys.Length; i++)
                {
                    string str = klass.keys[i];
                    if (!string.IsNullOrEmpty(str))
                    {
                        Setting setting2;
                        VisQuery query = klass.values[i];
                        if (!rep.dict.TryGetValue(str, out setting2))
                        {
                            if (query == null)
                            {
                                continue;
                            }
                            setting2 = new Setting(str, klass, rep);
                            rep.dict.Add(str, setting2);
                        }
                        else
                        {
                            rep.dict[str] = setting2 = setting2.Override(klass);
                        }
                        setting2.isInherited = false;
                        setting2.query = query;
                    }
                }
            }
            else
            {
                rep = new VisClass.Rep();
                rep.klass = nklass;
                rep.dict = new Dictionary<string, Setting>();
                for (int j = 0; j < klass.keys.Length; j++)
                {
                    string str2 = klass.keys[j];
                    if (!string.IsNullOrEmpty(str2))
                    {
                        VisQuery query2 = klass.values[j];
                        if (query2 != null)
                        {
                            Setting setting3 = new Setting(str2, klass, rep) {
                                query = query2
                            };
                            rep.dict.Add(str2, setting3);
                        }
                    }
                }
            }
        }

        internal static void Ref(ref VisClass.Rep rep, VisClass klass)
        {
            if (rep == null)
            {
                nklass = klass;
                building = true;
                Recur(ref rep, klass);
                building = false;
                nklass = null;
            }
        }

        private void Remove(Setting setting)
        {
            for (int i = 0; i < this.klass.keys.Length; i++)
            {
                if (this.klass.keys[i] == setting.name)
                {
                    int index = i;
                    while (++index < this.klass.keys.Length)
                    {
                        this.klass.keys[index - 1] = this.klass.keys[index];
                        this.klass.values[index - 1] = this.klass.values[index];
                    }
                    Array.Resize<string>(ref this.klass.keys, this.klass.keys.Length - 1);
                    Array.Resize<VisQuery>(ref this.klass.values, this.klass.values.Length - 1);
                    break;
                }
            }
            if (setting.isOverride)
            {
                this.dict[setting.name] = setting.MoveBack();
            }
            else
            {
                this.dict.Remove(setting.name);
            }
        }

        internal enum Action
        {
            None,
            Revert,
            Value
        }

        public class Setting
        {
            private bool _hasSuper;
            private VisClass _inheritedClass;
            private VisClass.Rep.Setting _inheritSetting;
            private bool _isInherited;
            private bool _unchanged;
            private VisQuery _value;
            private VisQuery _valueSet;
            internal VisClass.Rep.Action action;
            private string key;
            internal VisClass.Rep rep;

            internal Setting(string key, VisClass klass, VisClass.Rep rep)
            {
                this.key = key;
                this.rep = rep;
                this._inheritedClass = klass;
            }

            internal VisClass.Rep.Setting MoveBack()
            {
                return this._inheritSetting;
            }

            internal VisClass.Rep.Setting Override(VisClass klass)
            {
                VisClass.Rep.Setting setting = (VisClass.Rep.Setting) base.MemberwiseClone();
                setting._inheritedClass = klass;
                setting._hasSuper = true;
                setting._inheritSetting = this;
                return setting;
            }

            private VisClass inheritedClass
            {
                get
                {
                    return this._inheritedClass;
                }
            }

            public bool isInherited
            {
                get
                {
                    return this._isInherited;
                }
                set
                {
                    if (this._isInherited != value)
                    {
                        this._isInherited = value;
                        if (VisClass.Rep.MarkModified(this))
                        {
                            this.action = VisClass.Rep.Action.Revert;
                        }
                    }
                }
            }

            public bool isOverride
            {
                get
                {
                    return this._hasSuper;
                }
            }

            internal string name
            {
                get
                {
                    return this.key;
                }
            }

            public VisQuery query
            {
                get
                {
                    return this._value;
                }
                set
                {
                    if (this._isInherited)
                    {
                        VisClass.Rep.MarkModified(this);
                    }
                    else if (this._value == value)
                    {
                        return;
                    }
                    if (VisClass.Rep.MarkModified(this))
                    {
                        this.action = VisClass.Rep.Action.Value;
                        this._valueSet = value;
                    }
                    else
                    {
                        this._value = value;
                    }
                }
            }

            public VisQuery superQuery
            {
                get
                {
                    return (!this._hasSuper ? null : this._inheritSetting.query);
                }
            }

            internal VisQuery valueSet
            {
                get
                {
                    return this._valueSet;
                }
                set
                {
                    this._value = value;
                }
            }
        }
    }
}

