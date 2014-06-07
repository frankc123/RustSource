using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public abstract class AuthorCreation : AuthorShared
{
    [SerializeField]
    private Object _output;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map0;
    [SerializeField]
    private List<AuthorPeice> allPeices;
    protected int creationSeperatorHeight;
    private static readonly AuthorShared.PeiceCommand[] NoCommand = new AuthorShared.PeiceCommand[0];
    protected static readonly AuthorPalletObject[] NoPalletObjects = new AuthorPalletObject[0];
    protected static readonly AuthorPeice[] NoPeices = new AuthorPeice[0];
    [NonSerialized]
    public readonly Type outputType;
    protected int palletLabelHeight;
    protected int palletPanelWidth;
    [NonSerialized]
    private List<AuthorPeice> selected;
    protected int sideBarWidth;

    private AuthorCreation()
    {
        this.creationSeperatorHeight = 300;
        this.sideBarWidth = 200;
        this.palletLabelHeight = 0x30;
        this.palletPanelWidth = 0x60;
    }

    protected AuthorCreation(Type outputType) : this()
    {
        this.outputType = outputType;
    }

    public bool Contains(AuthorPeice comp)
    {
        if (this.allPeices != null)
        {
            foreach (AuthorPeice peice in this.allPeices)
            {
                if ((peice != null) && (peice == comp))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Contains(string peiceID)
    {
        if (this.allPeices != null)
        {
            foreach (AuthorPeice peice in this.allPeices)
            {
                if ((peice != null) && (peice.peiceID == peiceID))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public TPeice CreatePeice<TPeice>(string id, params Type[] additionalComponents) where TPeice: AuthorPeice
    {
        Type[] destinationArray = new Type[additionalComponents.Length + 1];
        Array.Copy(additionalComponents, 0, destinationArray, 1, additionalComponents.Length);
        destinationArray[0] = typeof(TPeice);
        GameObject obj2 = new GameObject(id, destinationArray);
        TPeice component = obj2.GetComponent<TPeice>();
        if ((component != null) && this.RegisterPeice(component, id))
        {
            return component;
        }
        Object.DestroyImmediate(obj2);
        return null;
    }

    protected virtual bool DefaultApply()
    {
        return false;
    }

    [DebuggerHidden]
    private IEnumerable<AuthorPeice> DoGUIPeiceInspector(List<AuthorPeice> peices)
    {
        return new <DoGUIPeiceInspector>c__Iterator3 { peices = peices, <$>peices = peices, $PC = -2 };
    }

    [DebuggerHidden]
    private IEnumerable<AuthorShared.PeiceCommand> DoGUIPeiceList(List<AuthorPeice> peices)
    {
        return new <DoGUIPeiceList>c__Iterator4 { peices = peices, <$>peices = peices, $PC = -2 };
    }

    [DebuggerHidden]
    public virtual IEnumerable<AuthorPeice> DoSceneView()
    {
        return new <DoSceneView>c__Iterator5 { <>f__this = this, $PC = -2 };
    }

    protected virtual IEnumerable<AuthorPalletObject> EnumeratePalletObjects()
    {
        return NoPalletObjects;
    }

    protected IEnumerable<AuthorPeice> EnumeratePeices()
    {
        return (((this.allPeices != null) && (this.allPeices.Count != 0)) ? ((IEnumerable<AuthorPeice>) new List<AuthorPeice>(this.allPeices)) : ((IEnumerable<AuthorPeice>) NoPeices));
    }

    internal IEnumerable<AuthorPeice> EnumeratePeices(bool selectedOnly)
    {
        return (!selectedOnly ? this.EnumeratePeices() : this.EnumerateSelectedPeices());
    }

    protected IEnumerable<AuthorPeice> EnumerateSelectedPeices()
    {
        return (((this.selected != null) && (this.selected.Count != 0)) ? ((IEnumerable<AuthorPeice>) new List<AuthorPeice>(this.selected)) : ((IEnumerable<AuthorPeice>) NoPeices));
    }

    public virtual void ExecuteCommand(AuthorShared.PeiceCommand cmd)
    {
        Debug.Log(cmd.action, cmd.peice);
        switch (cmd.action)
        {
            case AuthorShared.PeiceAction.AddToSelection:
            {
                Object selectReference = cmd.peice.selectReference;
                Object[] allSelectedObjects = AuthorShared.GetAllSelectedObjects();
                Array.Resize<Object>(ref allSelectedObjects, allSelectedObjects.Length + 1);
                allSelectedObjects[allSelectedObjects.Length - 1] = selectReference;
                AuthorShared.SetAllSelectedObjects(allSelectedObjects);
                break;
            }
            case AuthorShared.PeiceAction.RemoveFromSelection:
            {
                Object obj3 = cmd.peice.selectReference;
                Object[] array = AuthorShared.GetAllSelectedObjects();
                int newSize = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if ((array[i] != obj3) && (array[i] != cmd.peice))
                    {
                        array[newSize++] = array[i];
                    }
                }
                if (newSize < array.Length)
                {
                    Array.Resize<Object>(ref array, newSize);
                    AuthorShared.SetAllSelectedObjects(array);
                }
                break;
            }
            case AuthorShared.PeiceAction.SelectSolo:
            {
                Object[] objects = new Object[] { cmd.peice.selectReference };
                AuthorShared.SetAllSelectedObjects(objects);
                break;
            }
            case AuthorShared.PeiceAction.Delete:
            {
                bool? nullable = AuthorShared.Ask(string.Concat(new object[] { "You want to delete ", cmd.peice.peiceID, "? (", cmd.peice, ")" }));
                if (!nullable.HasValue ? false : nullable.Value)
                {
                    cmd.peice.Delete();
                }
                break;
            }
            case AuthorShared.PeiceAction.Dirty:
                AuthorShared.SetDirty(cmd.peice);
                break;

            case AuthorShared.PeiceAction.Ping:
                AuthorShared.PingObject(cmd.peice);
                break;
        }
    }

    protected Stream GetStream(bool write, string filepath, out AuthorCreationProject proj)
    {
        proj = AuthorCreationProject.current;
        if (proj == null)
        {
            throw new InvalidOperationException("Theres no project loaded");
        }
        if (proj.FindAuthorCreationInScene() != this)
        {
            throw new InvalidOperationException("The current project is not for this creation");
        }
        return proj.GetStream(write, filepath);
    }

    public bool GUICreationSettings()
    {
        return this.OnGUICreationSettings();
    }

    public bool GUIPalletObjects(params GUILayoutOption[] options)
    {
        return this.GUIPalletObjects(GUI.skin.button, options);
    }

    public bool GUIPalletObjects(GUIStyle buttonStyle, params GUILayoutOption[] options)
    {
        bool enabled = GUI.enabled;
        bool flag2 = false;
        IEnumerator<AuthorPalletObject> enumerator = this.EnumeratePalletObjects().GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                AuthorPeice peice;
                AuthorPalletObject current = enumerator.Current;
                if (current.guiContent == null)
                {
                    current.guiContent = new GUIContent(current.ToString());
                }
                GUI.enabled = enabled && current.Validate(this);
                if (GUILayout.Button(current.guiContent, buttonStyle, options) && current.Create(this, out peice))
                {
                    if (!this.RegisterPeice(peice))
                    {
                        Object.DestroyImmediate(peice.gameObject);
                    }
                    else
                    {
                        flag2 = true;
                    }
                }
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
        GUI.enabled = enabled;
        return flag2;
    }

    public IEnumerable<AuthorPeice> GUIPeiceInspector()
    {
        if ((this.selected != null) && (this.selected.Count != 0))
        {
            return this.DoGUIPeiceInspector(this.selected);
        }
        return NoPeices;
    }

    public IEnumerable<AuthorShared.PeiceCommand> GUIPeiceList()
    {
        if ((this.allPeices != null) && (this.allPeices.Count != 0))
        {
            return this.DoGUIPeiceList(this.allPeices);
        }
        return NoCommand;
    }

    protected bool LoadSettings()
    {
        AuthorCreationProject project;
        Stream stream = this.GetStream(true, "dat.asc", out project);
        if (stream != null)
        {
            try
            {
                using (JSONStream stream2 = JSONStream.CreateWriter(stream))
                {
                    while (stream2.Read())
                    {
                        if (stream2.token == JSONToken.ObjectStart)
                        {
                            string str;
                            while (stream2.ReadNextProperty(out str))
                            {
                                string key = str;
                                if (key != null)
                                {
                                    int num;
                                    if (<>f__switch$map0 == null)
                                    {
                                        Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                                        dictionary.Add("project", 0);
                                        dictionary.Add("settings", 1);
                                        <>f__switch$map0 = dictionary;
                                    }
                                    if (<>f__switch$map0.TryGetValue(key, out num))
                                    {
                                        if (num == 0)
                                        {
                                            stream2.ReadSkip();
                                        }
                                        else if (num == 1)
                                        {
                                            this.LoadSettings(stream2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            finally
            {
                stream.Dispose();
            }
        }
        return false;
    }

    protected abstract void LoadSettings(JSONStream stream);
    protected virtual bool OnGUICreationSettings()
    {
        return false;
    }

    protected virtual void OnSelectionChange()
    {
    }

    protected virtual void OnUnregisteredPeice(AuthorPeice peice)
    {
    }

    protected virtual void OnWillUnregisterPeice(AuthorPeice peice)
    {
    }

    protected virtual bool RegisterPeice(AuthorPeice peice)
    {
        if (this.allPeices == null)
        {
            this.allPeices = new List<AuthorPeice>();
            this.allPeices.Add(peice);
        }
        else if (!this.allPeices.Contains(peice))
        {
            this.allPeices.Add(peice);
        }
        else
        {
            return false;
        }
        peice.Registered(this);
        return true;
    }

    private bool RegisterPeice(AuthorPeice peice, string id)
    {
        peice.peiceID = id;
        return this.RegisterPeice(peice);
    }

    public virtual string RootBonePath(AuthorPeice callingPeice, Transform bone)
    {
        return AuthorShared.CalculatePath(bone, bone.root);
    }

    protected bool SaveSettings()
    {
        AuthorCreationProject project;
        Stream stream = this.GetStream(true, "dat.asc", out project);
        if (stream != null)
        {
            try
            {
                using (JSONStream stream2 = JSONStream.CreateWriter(stream))
                {
                    stream2.WriteObjectStart();
                    stream2.WriteObjectStart("project");
                    stream2.WriteText("guid", AuthorShared.PathToGUID(AuthorShared.GetAssetPath(project)));
                    stream2.WriteText("name", project.project);
                    stream2.WriteText("author", project.authorName);
                    stream2.WriteText("scene", project.scene);
                    stream2.WriteText("folder", project.folder);
                    stream2.WriteObjectEnd();
                    stream2.WriteProperty("settings");
                    this.SaveSettings(stream2);
                    stream2.WriteObjectEnd();
                }
                return true;
            }
            finally
            {
                stream.Dispose();
            }
        }
        return false;
    }

    protected abstract void SaveSettings(JSONStream stream);
    public bool SetSelection(Object[] objects)
    {
        List<AuthorPeice> collection = null;
        foreach (Object obj2 in objects)
        {
            if ((obj2 is AuthorPeice) && (obj2 != null))
            {
                if (collection == null)
                {
                    collection = new List<AuthorPeice> {
                        (AuthorPeice) obj2
                    };
                }
                else if (!collection.Contains((AuthorPeice) obj2))
                {
                    collection.Add((AuthorPeice) obj2);
                }
            }
        }
        bool flag = false;
        try
        {
            if (collection == null)
            {
                if (this.selected != null)
                {
                    flag = this.selected.Count > 0;
                    this.selected.Clear();
                }
                return flag;
            }
            if (this.allPeices != null)
            {
                collection.Sort((Comparison<AuthorPeice>) ((x, y) => this.allPeices.IndexOf(x).CompareTo(this.allPeices.IndexOf(y))));
            }
            if ((this.selected == null) || (this.selected.Count != collection.Count))
            {
                return true;
            }
            using (List<AuthorPeice>.Enumerator enumerator = this.selected.GetEnumerator())
            {
                using (List<AuthorPeice>.Enumerator enumerator2 = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext() && enumerator2.MoveNext())
                    {
                        if (enumerator.Current != enumerator2.Current)
                        {
                            return true;
                        }
                    }
                    return flag;
                }
            }
        }
        finally
        {
            if (flag)
            {
                if (this.selected != null)
                {
                    this.selected.Clear();
                    if (collection != null)
                    {
                        this.selected.AddRange(collection);
                    }
                }
                else if (collection != null)
                {
                    this.selected = collection;
                }
                this.OnSelectionChange();
            }
        }
        return flag;
    }

    internal void UnregisterPeice(AuthorPeice peice)
    {
        if ((this.allPeices != null) && (this.allPeices.IndexOf(peice) != -1))
        {
            this.OnWillUnregisterPeice(peice);
            this.allPeices.Remove(peice);
            if (this.selected != null)
            {
                this.selected.Remove(peice);
            }
            this.OnUnregisteredPeice(peice);
            if (!Application.isPlaying)
            {
                AuthorShared.SetDirty(this);
            }
        }
    }

    protected Object output
    {
        get
        {
            return this._output;
        }
    }

    public int palletContentHeight
    {
        get
        {
            return this.palletLabelHeight;
        }
    }

    public int palletWidth
    {
        get
        {
            return this.palletPanelWidth;
        }
    }

    public int rightPanelWidth
    {
        get
        {
            return this.sideBarWidth;
        }
    }

    public int settingsHeight
    {
        get
        {
            return this.creationSeperatorHeight;
        }
    }

    [CompilerGenerated]
    private sealed class <DoGUIPeiceInspector>c__Iterator3 : IDisposable, IEnumerable<AuthorPeice>, IEnumerator<AuthorPeice>, IEnumerator, IEnumerable
    {
        internal AuthorPeice $current;
        internal int $PC;
        internal List<AuthorPeice> <$>peices;
        internal List<AuthorPeice>.Enumerator <$s_20>__0;
        internal bool <b>__2;
        internal AuthorPeice <peice>__1;
        internal List<AuthorPeice> peices;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_20>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<$s_20>__0 = this.peices.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00D6;
            }
            try
            {
                while (this.<$s_20>__0.MoveNext())
                {
                    this.<peice>__1 = this.<$s_20>__0.Current;
                    AuthorShared.BeginVertical(AuthorShared.Styles.gradientOutline, new GUILayoutOption[0]);
                    this.<b>__2 = this.<peice>__1.PeiceInspectorGUI();
                    AuthorShared.EndVertical();
                    if (this.<b>__2)
                    {
                        this.$current = this.<peice>__1;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_20>__0.Dispose();
            }
            this.$PC = -1;
        Label_00D6:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<AuthorPeice> IEnumerable<AuthorPeice>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AuthorCreation.<DoGUIPeiceInspector>c__Iterator3 { peices = this.<$>peices };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<AuthorPeice>.GetEnumerator();
        }

        AuthorPeice IEnumerator<AuthorPeice>.Current
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

    [CompilerGenerated]
    private sealed class <DoGUIPeiceList>c__Iterator4 : IDisposable, IEnumerator, IEnumerable, IEnumerable<AuthorShared.PeiceCommand>, IEnumerator<AuthorShared.PeiceCommand>
    {
        internal AuthorShared.PeiceCommand $current;
        internal int $PC;
        internal List<AuthorPeice> <$>peices;
        internal List<AuthorPeice>.Enumerator <$s_21>__0;
        internal AuthorShared.PeiceAction <action>__2;
        internal AuthorPeice <peice>__1;
        internal List<AuthorPeice> peices;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_21>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<$s_21>__0 = this.peices.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00EE;
            }
            try
            {
                while (this.<$s_21>__0.MoveNext())
                {
                    this.<peice>__1 = this.<$s_21>__0.Current;
                    AuthorShared.BeginVertical(new GUILayoutOption[0]);
                    this.<action>__2 = this.<peice>__1.PeiceListGUI();
                    AuthorShared.EndVertical();
                    if (this.<action>__2 != AuthorShared.PeiceAction.None)
                    {
                        AuthorShared.PeiceCommand command = new AuthorShared.PeiceCommand {
                            peice = this.<peice>__1,
                            action = this.<action>__2
                        };
                        this.$current = command;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_21>__0.Dispose();
            }
            this.$PC = -1;
        Label_00EE:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<AuthorShared.PeiceCommand> IEnumerable<AuthorShared.PeiceCommand>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AuthorCreation.<DoGUIPeiceList>c__Iterator4 { peices = this.<$>peices };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<AuthorShared.PeiceCommand>.GetEnumerator();
        }

        AuthorShared.PeiceCommand IEnumerator<AuthorShared.PeiceCommand>.Current
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

    [CompilerGenerated]
    private sealed class <DoSceneView>c__Iterator5 : IDisposable, IEnumerable<AuthorPeice>, IEnumerator<AuthorPeice>, IEnumerator, IEnumerable
    {
        internal AuthorPeice $current;
        internal int $PC;
        internal List<AuthorPeice>.Enumerator <$s_22>__3;
        internal AuthorCreation <>f__this;
        internal bool <change>__5;
        internal Color <color>__1;
        internal bool <lighting>__2;
        internal Matrix4x4 <mat>__0;
        internal AuthorPeice <peice>__4;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_22>__3.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    if (this.<>f__this.selected == null)
                    {
                        goto Label_0126;
                    }
                    this.<mat>__0 = AuthorShared.Scene.matrix;
                    this.<color>__1 = AuthorShared.Scene.color;
                    this.<lighting>__2 = AuthorShared.Scene.lighting;
                    this.<$s_22>__3 = this.<>f__this.selected.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_012D;
            }
            try
            {
                while (this.<$s_22>__3.MoveNext())
                {
                    this.<peice>__4 = this.<$s_22>__3.Current;
                    if (this.<peice>__4 != null)
                    {
                        try
                        {
                            this.<change>__5 = this.<peice>__4.OnSceneView();
                        }
                        finally
                        {
                            AuthorShared.Scene.matrix = this.<mat>__0;
                            AuthorShared.Scene.color = this.<color>__1;
                            AuthorShared.Scene.lighting = this.<lighting>__2;
                        }
                        if (this.<change>__5)
                        {
                            this.$current = this.<peice>__4;
                            this.$PC = 1;
                            flag = true;
                            return true;
                        }
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_22>__3.Dispose();
            }
        Label_0126:
            this.$PC = -1;
        Label_012D:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<AuthorPeice> IEnumerable<AuthorPeice>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new AuthorCreation.<DoSceneView>c__Iterator5 { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<AuthorPeice>.GetEnumerator();
        }

        AuthorPeice IEnumerator<AuthorPeice>.Current
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
}

