using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using uLink;
using UnityEngine;

public abstract class Controller : IDLocalCharacterAddon
{
    [NonSerialized]
    private Controllable _controllable;
    [NonSerialized]
    private bool _doesNotSave;
    [NonSerialized]
    private bool _forwardsPlayerClientInput;
    [NonSerialized]
    private RPOSLimitFlags _rposLimitFlags;
    [NonSerialized]
    protected Commandment commandment;
    [NonSerialized]
    private readonly ControllerFlags controllerFlags;
    [NonSerialized]
    private bool wasSetup;

    protected Controller(ControllerFlags controllerFlags) : this(controllerFlags, 0)
    {
    }

    protected Controller(ControllerFlags controllerFlags, IDLocalCharacterAddon.AddonFlags addonFlags) : base(addonFlags)
    {
        this.controllerFlags = controllerFlags;
    }

    public bool AssignedControlOf(Controllable controllable)
    {
        return this._controllable.AssignedControlOf(controllable);
    }

    public bool AssignedControlOf(Controller controller)
    {
        return this._controllable.AssignedControlOf(controller);
    }

    public bool AssignedControlOf(IDBase idBase)
    {
        return this._controllable.AssignedControlOf(idBase);
    }

    public bool AssignedControlOf(IDMain character)
    {
        return this._controllable.AssignedControlOf(character);
    }

    [Obsolete("Used only by Controllable")]
    internal void ControlCease(int cmd)
    {
        Commandment commandment = this.commandment;
        this.commandment = new Commandment((cmd & 0x7fff) | 0x18000);
        try
        {
            this.OnControlCease();
        }
        finally
        {
            this.commandment = commandment;
        }
    }

    [Obsolete("Used only by Controllable")]
    internal void ControlEngauge(int cmd)
    {
        Commandment commandment = this.commandment;
        this.commandment = new Commandment((cmd & 0x7fff) | 0x10000);
        try
        {
            this.OnControlEngauge();
        }
        finally
        {
            this.commandment = commandment;
        }
    }

    [Obsolete("Used only by Controllable")]
    internal void ControlEnter(int cmd)
    {
        Commandment commandment = this.commandment;
        this.commandment = new Commandment((cmd & 0x7fff) | 0x8000);
        try
        {
            this.OnControlEnter();
        }
        finally
        {
            this.commandment = commandment;
        }
    }

    [Obsolete("Used only by Controllable")]
    internal void ControlExit(int cmd)
    {
        Commandment commandment = this.commandment;
        this.commandment = new Commandment((cmd & 0x7fff) | 0x20000);
        try
        {
            this.OnControlExit();
        }
        finally
        {
            this.commandment = commandment;
        }
    }

    internal void ControllerSetup(Controllable controllable, NetworkView view, ref NetworkMessageInfo info)
    {
        bool flag;
        if (this.wasSetup)
        {
            throw new InvalidOperationException("Already was setup");
        }
        this.wasSetup = true;
        switch ((this.controllerFlags & ControllerFlags.DontMessWithEnabled))
        {
            case ControllerFlags.AlwaysSavedAsDisabled:
                flag = false;
                if (base.enabled)
                {
                    base.enabled = false;
                    Debug.LogError("this was not saved as enabled", this);
                }
                break;

            case ControllerFlags.AlwaysSavedAsEnabled:
                flag = false;
                if (!base.enabled)
                {
                    base.enabled = true;
                    Debug.LogError("this was not saved as disabled", this);
                }
                break;

            case ControllerFlags.DontMessWithEnabled:
                flag = true;
                break;

            default:
                flag = false;
                break;
        }
        this._controllable = controllable;
        if (this.playerControlled)
        {
            if (this.localPlayerControlled)
            {
                if ((this.controllerFlags & ControllerFlags.IncompatibleAsLocalPlayer) == ControllerFlags.IncompatibleAsLocalPlayer)
                {
                    throw new NotSupportedException();
                }
            }
            else if ((this.controllerFlags & ControllerFlags.IncompatibleAsRemotePlayer) == ControllerFlags.IncompatibleAsRemotePlayer)
            {
                throw new NotSupportedException();
            }
        }
        else if (this.localAIControlled)
        {
            if ((this.controllerFlags & ControllerFlags.IncompatibleAsLocalAI) == ControllerFlags.IncompatibleAsLocalAI)
            {
                throw new NotSupportedException();
            }
        }
        else if ((this.controllerFlags & ControllerFlags.IncompatibleAsRemoteAI) == ControllerFlags.IncompatibleAsRemoteAI)
        {
            throw new NotSupportedException();
        }
        this.OnControllerSetup(view, ref info);
        if (!flag)
        {
            ControllerFlags enableWhenLocalPlayer;
            ControllerFlags disableWhenLocalPlayer;
            if (this.playerControlled)
            {
                if (this.localPlayerControlled)
                {
                    enableWhenLocalPlayer = ControllerFlags.EnableWhenLocalPlayer;
                    disableWhenLocalPlayer = ControllerFlags.DisableWhenLocalPlayer;
                }
                else
                {
                    enableWhenLocalPlayer = ControllerFlags.EnableWhenRemotePlayer;
                    disableWhenLocalPlayer = ControllerFlags.DisableWhenRemotePlayer;
                }
            }
            else if (this.localAIControlled)
            {
                enableWhenLocalPlayer = ControllerFlags.EnableWhenLocalAI;
                disableWhenLocalPlayer = ControllerFlags.DisableWhenLocalAI;
            }
            else
            {
                enableWhenLocalPlayer = ControllerFlags.EnableWhenRemoteAI;
                disableWhenLocalPlayer = ControllerFlags.DisableWhenRemoteAI;
            }
            if ((this.controllerFlags & enableWhenLocalPlayer) == enableWhenLocalPlayer)
            {
                if ((this.controllerFlags & disableWhenLocalPlayer) == disableWhenLocalPlayer)
                {
                    base.enabled = !base.enabled;
                }
                else
                {
                    base.enabled = true;
                }
            }
            else if ((this.controllerFlags & disableWhenLocalPlayer) == disableWhenLocalPlayer)
            {
                base.enabled = false;
            }
        }
    }

    public bool ControlOverriddenBy(Character character)
    {
        return this._controllable.ControlOverriddenBy(character);
    }

    public bool ControlOverriddenBy(Controllable controllable)
    {
        return this._controllable.ControlOverriddenBy(controllable);
    }

    public bool ControlOverriddenBy(Controller controller)
    {
        return this._controllable.ControlOverriddenBy(controller);
    }

    public bool ControlOverriddenBy(IDBase idBase)
    {
        return this._controllable.ControlOverriddenBy(idBase);
    }

    public bool ControlOverriddenBy(IDLocalCharacter idLocal)
    {
        return this._controllable.ControlOverriddenBy(idLocal);
    }

    public bool ControlOverriddenBy(IDMain main)
    {
        return this._controllable.ControlOverriddenBy(main);
    }

    [DebuggerHidden]
    public static IEnumerable<Controller> CurrentControllers(IEnumerable<PlayerClient> playerClients)
    {
        return new <CurrentControllers>c__Iterator11 { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    [DebuggerHidden]
    public static IEnumerable<TController> CurrentControllers<TController>(IEnumerable<PlayerClient> playerClients) where TController: Controller
    {
        return new <CurrentControllers>c__Iterator13<TController> { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    protected virtual void OnControlCease()
    {
    }

    protected virtual void OnControlEngauge()
    {
    }

    protected virtual void OnControlEnter()
    {
    }

    protected virtual void OnControlExit()
    {
    }

    protected virtual void OnControllerSetup(NetworkView networkView, ref NetworkMessageInfo info)
    {
    }

    protected virtual void OnLocalPlayerInputFrame()
    {
    }

    protected virtual void OnLocalPlayerPreRender()
    {
    }

    public bool OverridingControlOf(Character character)
    {
        return this._controllable.OverridingControlOf(character);
    }

    public bool OverridingControlOf(Controllable controllable)
    {
        return this._controllable.OverridingControlOf(controllable);
    }

    public bool OverridingControlOf(Controller controller)
    {
        return this._controllable.OverridingControlOf(controller);
    }

    public bool OverridingControlOf(IDBase idBase)
    {
        return this._controllable.OverridingControlOf(idBase);
    }

    public bool OverridingControlOf(IDLocalCharacter idLocal)
    {
        return this._controllable.OverridingControlOf(idLocal);
    }

    public bool OverridingControlOf(IDMain main)
    {
        return this._controllable.OverridingControlOf(main);
    }

    internal void ProcessLocalPlayerInput()
    {
        this.OnLocalPlayerInputFrame();
    }

    internal void ProcessLocalPlayerPreRender()
    {
        this.OnLocalPlayerPreRender();
    }

    public RelativeControl RelativeControlFrom(Character character)
    {
        return this._controllable.RelativeControlFrom(character);
    }

    public RelativeControl RelativeControlFrom(Controllable controllable)
    {
        return this._controllable.RelativeControlFrom(controllable);
    }

    public RelativeControl RelativeControlFrom(Controller controller)
    {
        return this._controllable.RelativeControlFrom(controller);
    }

    public RelativeControl RelativeControlFrom(IDBase idBase)
    {
        return this._controllable.RelativeControlFrom(idBase);
    }

    public RelativeControl RelativeControlFrom(IDLocalCharacter idLocal)
    {
        return this._controllable.RelativeControlFrom(idLocal);
    }

    public RelativeControl RelativeControlFrom(IDMain main)
    {
        return this._controllable.RelativeControlFrom(main);
    }

    public RelativeControl RelativeControlTo(Character character)
    {
        return this._controllable.RelativeControlTo(character);
    }

    public RelativeControl RelativeControlTo(Controllable controllable)
    {
        return this._controllable.RelativeControlTo(controllable);
    }

    public RelativeControl RelativeControlTo(Controller controller)
    {
        return this._controllable.RelativeControlTo(controller);
    }

    public RelativeControl RelativeControlTo(IDBase idBase)
    {
        return this._controllable.RelativeControlTo(idBase);
    }

    public RelativeControl RelativeControlTo(IDLocalCharacter idLocal)
    {
        return this._controllable.RelativeControlTo(idLocal);
    }

    public RelativeControl RelativeControlTo(IDMain main)
    {
        return this._controllable.RelativeControlTo(main);
    }

    [DebuggerHidden]
    public static IEnumerable<Controller> RootControllers(IEnumerable<PlayerClient> playerClients)
    {
        return new <RootControllers>c__Iterator10 { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    [DebuggerHidden]
    public static IEnumerable<TController> RootControllers<TController>(IEnumerable<PlayerClient> playerClients) where TController: Controller
    {
        return new <RootControllers>c__Iterator12<TController> { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    public bool aiControlled
    {
        get
        {
            return this._controllable.aiControlled;
        }
    }

    public bool assignedControl
    {
        get
        {
            return this._controllable.assignedControl;
        }
    }

    public int controlCount
    {
        get
        {
            return this._controllable.controlCount;
        }
    }

    public int controlDepth
    {
        get
        {
            return this._controllable.controlDepth;
        }
    }

    public Controllable controllable
    {
        get
        {
            return this._controllable;
        }
    }

    public bool controlled
    {
        get
        {
            return this._controllable.controlled;
        }
    }

    public Controllable controlledControllable
    {
        get
        {
            return (!this._controllable.controlled ? null : this._controllable);
        }
    }

    public Controller controlledController
    {
        get
        {
            return (!this._controllable.controlled ? null : this);
        }
    }

    public Controller controller
    {
        get
        {
            return this;
        }
    }

    public string controllerClassName
    {
        get
        {
            return this._controllable.controllerClassName;
        }
    }

    public bool controlOverridden
    {
        get
        {
            return this._controllable.controlOverridden;
        }
    }

    public bool doesNotSave
    {
        get
        {
            return this._doesNotSave;
        }
        protected set
        {
            this._doesNotSave = value;
        }
    }

    public bool forwardsPlayerClientInput
    {
        get
        {
            return this._forwardsPlayerClientInput;
        }
        protected set
        {
            this._forwardsPlayerClientInput = value;
        }
    }

    public bool localAIControlled
    {
        get
        {
            return this._controllable.localAIControlled;
        }
    }

    public Controllable localAIControlledControllable
    {
        get
        {
            return this._controllable.localAIControlledControllable;
        }
    }

    public Controller localAIControlledController
    {
        get
        {
            return this._controllable.localAIControlledController;
        }
    }

    public bool localControlled
    {
        get
        {
            return this._controllable.localControlled;
        }
    }

    public bool localPlayerControlled
    {
        get
        {
            return this._controllable.localPlayerControlled;
        }
    }

    public Controllable localPlayerControlledControllable
    {
        get
        {
            return this._controllable.localPlayerControlledControllable;
        }
    }

    public Controller localPlayerControlledController
    {
        get
        {
            return this._controllable.localPlayerControlledController;
        }
    }

    public Character masterCharacter
    {
        get
        {
            return this._controllable.masterCharacter;
        }
    }

    public Controllable masterControllable
    {
        get
        {
            return this._controllable.masterControllable;
        }
    }

    public Controller masterController
    {
        get
        {
            return this._controllable.masterController;
        }
    }

    public Character nextCharacter
    {
        get
        {
            return this._controllable.nextCharacter;
        }
    }

    public Controllable nextControllable
    {
        get
        {
            return this._controllable.nextControllable;
        }
    }

    public Controller nextController
    {
        get
        {
            return this._controllable.nextController;
        }
    }

    public string npcName
    {
        get
        {
            return this._controllable.npcName;
        }
    }

    public bool overridingControl
    {
        get
        {
            return this._controllable.overridingControl;
        }
    }

    public PlayerClient playerClient
    {
        get
        {
            return this._controllable.playerClient;
        }
    }

    public bool playerControlled
    {
        get
        {
            return this._controllable.playerControlled;
        }
    }

    public static IEnumerable<Controller> PlayerCurrentControllers
    {
        get
        {
            return new <>c__IteratorF { $PC = -2 };
        }
    }

    public static IEnumerable<Controller> PlayerRootControllers
    {
        get
        {
            return new <>c__IteratorE { $PC = -2 };
        }
    }

    public Character previousCharacter
    {
        get
        {
            return this._controllable.previousCharacter;
        }
    }

    public Controllable previousControllable
    {
        get
        {
            return this._controllable.previousControllable;
        }
    }

    public Controller previousController
    {
        get
        {
            return this._controllable.previousController;
        }
    }

    public bool remoteAIControlled
    {
        get
        {
            return this._controllable.remoteAIControlled;
        }
    }

    public Controllable remoteAIControlledControllable
    {
        get
        {
            return this._controllable.remoteAIControlledControllable;
        }
    }

    public Controller remoteAIControlledController
    {
        get
        {
            return this._controllable.remoteAIControlledController;
        }
    }

    public bool remoteControlled
    {
        get
        {
            return this._controllable.remoteControlled;
        }
    }

    public bool remotePlayerControlled
    {
        get
        {
            return this._controllable.remotePlayerControlled;
        }
    }

    public Controllable remotePlayerControlledControllable
    {
        get
        {
            return this._controllable.remotePlayerControlledControllable;
        }
    }

    public Controller remotePlayerControlledController
    {
        get
        {
            return this._controllable.remotePlayerControlledController;
        }
    }

    public Character rootCharacter
    {
        get
        {
            return this._controllable.rootCharacter;
        }
    }

    public Controllable rootControllable
    {
        get
        {
            return this._controllable.rootControllable;
        }
    }

    public Controller rootController
    {
        get
        {
            return this._controllable.rootController;
        }
    }

    public RPOSLimitFlags rposLimitFlags
    {
        get
        {
            return this._rposLimitFlags;
        }
        protected internal set
        {
            this._rposLimitFlags = value;
        }
    }

    [CompilerGenerated]
    private sealed class <>c__IteratorE : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controller>, IEnumerator<Controller>
    {
        internal Controller $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_131>__0;
        internal Controllable <controllable>__2;
        internal Controller <controller>__3;
        internal PlayerClient <pc>__1;

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
                        this.<$s_131>__0.Dispose();
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
                    this.<$s_131>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00EA;
            }
            try
            {
                while (this.<$s_131>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_131>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controllable>__2.controller;
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
                this.<$s_131>__0.Dispose();
            }
            this.$PC = -1;
        Label_00EA:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controller> IEnumerable<Controller>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<>c__IteratorE();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controller>.GetEnumerator();
        }

        Controller IEnumerator<Controller>.Current
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
    private sealed class <>c__IteratorF : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controller>, IEnumerator<Controller>
    {
        internal Controller $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_132>__0;
        internal Controllable <controllable>__2;
        internal Controller <controller>__3;
        internal PlayerClient <pc>__1;

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
                        this.<$s_132>__0.Dispose();
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
                    this.<$s_132>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00EA;
            }
            try
            {
                while (this.<$s_132>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_132>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controllable>__2.controller;
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
                this.<$s_132>__0.Dispose();
            }
            this.$PC = -1;
        Label_00EA:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controller> IEnumerable<Controller>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<>c__IteratorF();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controller>.GetEnumerator();
        }

        Controller IEnumerator<Controller>.Current
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
    private sealed class <CurrentControllers>c__Iterator11 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controller>, IEnumerator<Controller>
    {
        internal Controller $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_134>__0;
        internal Controllable <controllable>__2;
        internal Controller <controller>__3;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_134>__0 == null)
                        {
                        }
                        this.<$s_134>__0.Dispose();
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
                    this.<$s_134>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00EA;
            }
            try
            {
                while (this.<$s_134>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_134>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controller>__3;
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
                if (this.<$s_134>__0 == null)
                {
                }
                this.<$s_134>__0.Dispose();
            }
            this.$PC = -1;
        Label_00EA:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controller> IEnumerable<Controller>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<CurrentControllers>c__Iterator11 { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controller>.GetEnumerator();
        }

        Controller IEnumerator<Controller>.Current
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
    private sealed class <CurrentControllers>c__Iterator13<TController> : IDisposable, IEnumerator, IEnumerable, IEnumerable<TController>, IEnumerator<TController> where TController: Controller
    {
        internal TController $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_136>__0;
        internal Controllable <controllable>__2;
        internal TController <controller>__3;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_136>__0 == null)
                        {
                        }
                        this.<$s_136>__0.Dispose();
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
                    this.<$s_136>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00F9;
            }
            try
            {
                while (this.<$s_136>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_136>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller as TController;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controller>__3;
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
                if (this.<$s_136>__0 == null)
                {
                }
                this.<$s_136>__0.Dispose();
            }
            this.$PC = -1;
        Label_00F9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<TController> IEnumerable<TController>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<CurrentControllers>c__Iterator13<TController> { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<TController>.GetEnumerator();
        }

        TController IEnumerator<TController>.Current
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
    private sealed class <RootControllers>c__Iterator10 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controller>, IEnumerator<Controller>
    {
        internal Controller $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_133>__0;
        internal Controllable <controllable>__2;
        internal Controller <controller>__3;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_133>__0 == null)
                        {
                        }
                        this.<$s_133>__0.Dispose();
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
                    this.<$s_133>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00EA;
            }
            try
            {
                while (this.<$s_133>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_133>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controller>__3;
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
                if (this.<$s_133>__0 == null)
                {
                }
                this.<$s_133>__0.Dispose();
            }
            this.$PC = -1;
        Label_00EA:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controller> IEnumerable<Controller>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<RootControllers>c__Iterator10 { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controller>.GetEnumerator();
        }

        Controller IEnumerator<Controller>.Current
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
    private sealed class <RootControllers>c__Iterator12<TController> : IDisposable, IEnumerator, IEnumerable, IEnumerable<TController>, IEnumerator<TController> where TController: Controller
    {
        internal TController $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_135>__0;
        internal Controllable <controllable>__2;
        internal TController <controller>__3;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_135>__0 == null)
                        {
                        }
                        this.<$s_135>__0.Dispose();
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
                    this.<$s_135>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00F9;
            }
            try
            {
                while (this.<$s_135>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_135>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<controller>__3 = this.<controllable>__2.controller as TController;
                        if (this.<controller>__3 != null)
                        {
                            this.$current = this.<controller>__3;
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
                if (this.<$s_135>__0 == null)
                {
                }
                this.<$s_135>__0.Dispose();
            }
            this.$PC = -1;
        Label_00F9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<TController> IEnumerable<TController>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controller.<RootControllers>c__Iterator12<TController> { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<TController>.GetEnumerator();
        }

        TController IEnumerator<TController>.Current
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

    [StructLayout(LayoutKind.Sequential, Size=4)]
    internal protected struct Commandment
    {
        private const int B = 1;
        internal const int THIS_TO_BASE = 1;
        internal const int THIS_TO_ROOT = 2;
        internal const int ALL = 0x7fff;
        internal const int ALL_THIS = 0x91;
        internal const int ALL_BASE = 290;
        internal const int ALL_ROOT = 580;
        private readonly int f;
        internal Commandment(int f)
        {
            this.f = f & 0x3ffff;
        }

        public bool thisDestroying
        {
            get
            {
                return ((this.f & 1) == 1);
            }
        }
        public bool baseDestroying
        {
            get
            {
                return ((this.f & 2) == 2);
            }
        }
        public bool rootDestroying
        {
            get
            {
                return ((this.f & 4) == 4);
            }
        }
        public bool baseExit
        {
            get
            {
                return ((this.f & 0x20) == 0x20);
            }
        }
        public bool thisExit
        {
            get
            {
                return ((this.f & 0x10) == 0x10);
            }
        }
        public bool rootExit
        {
            get
            {
                return ((this.f & 0x40) == 0x40);
            }
        }
        public bool networkValid
        {
            get
            {
                return ((this.f & 8) == 0);
            }
        }
        public bool networkInvalid
        {
            get
            {
                return ((this.f & 8) == 8);
            }
        }
        public bool overrideThis
        {
            get
            {
                return ((this.f & 0x80) == 0x80);
            }
        }
        public bool overrideBase
        {
            get
            {
                return ((this.f & 0x100) == 0x100);
            }
        }
        public bool overrideRoot
        {
            get
            {
                return ((this.f & 0x200) == 0x200);
            }
        }
        public bool ownerServer
        {
            get
            {
                return ((this.f & 0x2000) == 0);
            }
        }
        public bool ownerClient
        {
            get
            {
                return ((this.f & 0x2000) == 0x2000);
            }
        }
        public bool runningLocally
        {
            get
            {
                return ((this.f & 0x4000) == 0x4000);
            }
        }
        public bool runningRemotely
        {
            get
            {
                return ((this.f & 0x4000) == 0);
            }
        }
        public bool callFirst
        {
            get
            {
                return ((this.f & 0x400) == 0);
            }
        }
        public bool callAgain
        {
            get
            {
                return ((this.f & 0x400) == 0x400);
            }
        }
        public bool bindWeak
        {
            get
            {
                return ((this.f & 0x1000) == 0);
            }
        }
        public bool bindStrong
        {
            get
            {
                return ((this.f & 0x1000) == 0x1000);
            }
        }
        public bool kindRoot
        {
            get
            {
                return ((this.f & 0x800) == 0x800);
            }
        }
        public bool kindVessel
        {
            get
            {
                return ((this.f & 0x800) == 0);
            }
        }
        public override string ToString()
        {
            if ((this.f & 0x38000) == 0)
            {
                return "INVALID";
            }
            StringBuilder builder = new StringBuilder();
            int num = this.f & 0x70;
            switch (num)
            {
                case 0x10:
                    builder.Append("exit[THIS]");
                    break;

                case 0x12:
                    builder.Append("exit[THIS,BASE]");
                    break;

                case 20:
                    builder.Append("exit[THIS,ROOT]");
                    break;

                default:
                    if (num == 0x20)
                    {
                        builder.Append("exit[BASE]");
                    }
                    else if (num == 0x24)
                    {
                        builder.Append("exit[BASE,ROOT]");
                    }
                    else if (num == 0x40)
                    {
                        builder.Append("exit[ROOT]");
                    }
                    else if (num == 0x70)
                    {
                        builder.Append("exit[ALL]");
                    }
                    break;
            }
            num = this.f & 0x380;
            switch (num)
            {
                case 0x80:
                    builder.Append("override[THIS]");
                    break;

                case 130:
                    builder.Append("override[THIS,BASE]");
                    break;

                case 0x84:
                    builder.Append("override[THIS,ROOT]");
                    break;

                default:
                    if (num == 0x100)
                    {
                        builder.Append("override[BASE]");
                    }
                    else if (num == 260)
                    {
                        builder.Append("override[BASE,ROOT]");
                    }
                    else if (num == 0x200)
                    {
                        builder.Append("override[ROOT]");
                    }
                    else if (num == 0x380)
                    {
                        builder.Append("override[ALL]");
                    }
                    break;
            }
            switch ((this.f & 0x800))
            {
                case 0:
                    builder.Append("kind[VESL]");
                    break;

                case 0x800:
                    builder.Append("kind[ROOT]");
                    break;
            }
            num = this.f & 0x1000;
            if (num == 0)
            {
                builder.Append("bind[WEAK]");
            }
            else if (num == 0x1000)
            {
                builder.Append("bind[STRONG]");
            }
            num = this.f & 0x2000;
            if (num == 0)
            {
                builder.Append("server[");
            }
            else if (num == 0x2000)
            {
                builder.Append("client[");
            }
            num = this.f & 0x4000;
            if (num == 0)
            {
                builder.Append("RMOTE]");
            }
            else if (num == 0x4000)
            {
                builder.Append("LOCAL]");
            }
            switch ((this.f & 8))
            {
                case 0:
                    builder.Append("net[YES]");
                    break;

                case 8:
                    builder.Append("net[NOO]");
                    break;
            }
            switch ((this.f & 7))
            {
                case 1:
                    builder.Append("destroy[THIS]");
                    break;

                case 2:
                    builder.Append("destroy[BASE]");
                    break;

                case 3:
                    builder.Append("destroy[THIS,BASE]");
                    break;

                case 4:
                    builder.Append("destroy[ROOT]");
                    break;

                case 5:
                    builder.Append("destroy[THIS,ROOT]");
                    break;

                case 6:
                    builder.Append("destroy[BASE,ROOT]");
                    break;

                case 7:
                    builder.Append("destroy[ALL]");
                    break;
            }
            switch ((this.f & 0x38000))
            {
                case 0x8000:
                    builder.Append("->ENTR");
                    break;

                case 0x10000:
                    builder.Append("->PRMO");
                    break;

                case 0x18000:
                    builder.Append("->DEMO");
                    break;

                case 0x20000:
                    builder.Append("->EXIT");
                    break;
            }
            if ((this.f & 0x400) == 0)
            {
                builder.Append("(first)");
            }
            return builder.ToString();
        }
        internal static class BINDING
        {
            public const int ALL = 0x1000;
            public const int STRONG = 0x1000;
            public const int WEAK = 0;
        }

        internal static class DESTROY
        {
            public const int ALL = 7;
            public const int BASE = 2;
            public const int NONE = 0;
            public const int ROOT = 4;
            public const int THIS = 1;
        }

        internal static class EVENT
        {
            public const int ALL = 0x38000;
            public const int CEASE = 0x18000;
            public const int ENGAUGE = 0x10000;
            public const int ENTER = 0x8000;
            public const int EXIT = 0x20000;
            public const int NONE = 0;
        }

        internal static class EXIT
        {
            public const int ALL = 0x70;
            public const int BASE = 0x20;
            public const int NONE = 0;
            public const int ROOT = 0x40;
            public const int THIS = 0x10;
        }

        internal static class KIND
        {
            public const int ALL = 0x800;
            public const int ROOT = 0x800;
            public const int VESSEL = 0;
        }

        internal static class NETWORK
        {
            public const int ALL = 8;
            public const int INVALID = 8;
            public const int VALID = 0;
        }

        internal static class ONCE
        {
            public const int ALL = 0x400;
            public const int FALSE = 0;
            public const int TRUE = 0x400;
        }

        internal static class OVERRIDE
        {
            public const int ALL = 0x380;
            public const int BASE = 0x100;
            public const int NONE = 0;
            public const int ROOT = 0x200;
            public const int THIS = 0x80;
        }

        internal static class OWNER
        {
            public const int ALL = 0x2000;
            public const int CLIENT = 0x2000;
            public const int SERVER = 0;
        }

        internal static class PLACE
        {
            public const int ALL = 0x4000;
            public const int ELSEWHERE = 0;
            public const int HERE = 0x4000;
        }
    }

    protected enum ControllerFlags
    {
        AlwaysSavedAsDisabled = 0x100,
        AlwaysSavedAsEnabled = 0x200,
        DisableWhenLocalAI = 0x20,
        DisableWhenLocalPlayer = 0x10,
        DisableWhenRemoteAI = 0x80,
        DisableWhenRemotePlayer = 0x40,
        DontMessWithEnabled = 0x300,
        EnableWhenLocalAI = 2,
        EnableWhenLocalPlayer = 1,
        EnableWhenRemoteAI = 8,
        EnableWhenRemotePlayer = 4,
        IncompatibleAsLocalAI = 0x2000,
        IncompatibleAsLocalPlayer = 0x1000,
        IncompatibleAsRemoteAI = 0x400,
        IncompatibleAsRemotePlayer = 0x800,
        ToggleEnableLocalAI = 0x22,
        ToggleEnableRemoteAI = 0x88,
        ToggleEnableRemotePlayer = 0x44,
        ToggleEnableWhenLocalPlayer = 0x11
    }
}

