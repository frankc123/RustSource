using Facepunch;
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

public sealed class Controllable : IDLocalCharacter
{
    [NonSerialized]
    private NetworkMessageInfo __controllerCreateMessageInfo;
    [NonSerialized]
    private NetworkViewID __controllerDriverViewID;
    [NonSerialized]
    private NetworkView __networkViewForControllable;
    [NonSerialized]
    private CL_Binder _binder;
    [NonSerialized]
    private Controller _controller;
    [NonSerialized]
    private int _pendingControlCount;
    [NonSerialized]
    private PlayerClient _playerClient;
    [NonSerialized]
    private int _refreshedControlCount;
    [NonSerialized]
    private List<ulong> _rootCountTimeStamps;
    private const ControlFlags ACTIVE_MASK = ControlFlags.Owned;
    private const ControlFlags ACTIVE_OCCUPIED = ControlFlags.Owned;
    private const ControlFlags ACTIVE_VACANT = 0;
    private const ControlFlags BINDING_MASK = ControlFlags.Strong;
    private const ControlFlags BINDING_STRONG = ControlFlags.Strong;
    private const ControlFlags BINDING_WEAK = 0;
    [NonSerialized]
    private Chain ch;
    [SerializeField]
    private ControllerClass @class;
    private const ControlFlags CONTROLLER_CLIENT = ControlFlags.Player;
    private const ControlFlags CONTROLLER_MASK = ControlFlags.Player;
    private const ControlFlags CONTROLLER_NPC = 0;
    [NonSerialized]
    private ControlFlags F;
    [NonSerialized]
    public bool isInContextQuery;
    private const RPCMode kClearFromChainRPCMode = RPCMode.All;
    private const RPCMode kClearFromChainRPCMode_POST = RPCMode.Others;
    private const string kClearFromChainRPCName = "Controllable:CLR";
    private const RPCMode kClientDeleteRPCMode = RPCMode.Server;
    private const string kClientDeleteRPCName = "Controllable:CLD";
    private const RPCMode kClientRefreshRPCMode = RPCMode.OthersBuffered;
    private const string kClientRefreshRPCName = "Controllable:RFH";
    private const RPCMode kClientSideRootNumberRPCMode = RPCMode.OthersBuffered;
    private const string kControllableRPCPrefix = "Controllable:";
    private const RPCMode kIdleOnRPCMode = RPCMode.AllBuffered;
    private const string kIdleOnRPCName = "Controllable:ID1";
    private const RPCMode kOverrideControlOfRPCMode = RPCMode.AllBuffered;
    private const string kOverrideControlOfRPCName1 = "Controllable:OC1";
    private const string kOverrideControlOfRPCName2 = "Controllable:OC2";
    private const string kRPCCall = "RPC call only. Do not call through script";
    private const bool kRPCCallError = false;
    [NonSerialized]
    private bool lateFinding;
    private static int localPlayerControllableCount;
    private const ControlFlags MASK = (ControlFlags.Strong | ControlFlags.Initialized | ControlFlags.Root | ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags MUTABLE_FLAGS = (ControlFlags.Initialized | ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags NETWORK_MASK = ControlFlags.Local;
    private const ControlFlags NETWORK_MINE = ControlFlags.Local;
    private const ControlFlags NETWORK_PROXY = 0;
    private const ControlFlags OWNER_CLIENT = (ControlFlags.Player | ControlFlags.Owned);
    private const ControlFlags OWNER_MASK = (ControlFlags.Player | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_CLIENT_MINE = (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_CLIENT_PROXY = (ControlFlags.Player | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_MASK = (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_NPC_MINE = (ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_NPC_PROXY = ControlFlags.Owned;
    private const ControlFlags OWNER_NET_TREE_CLIENT_MINE_BRANCH = (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_CLIENT_MINE_TRUNK = (ControlFlags.Root | ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_CLIENT_PROXY_BRANCH = (ControlFlags.Player | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_CLIENT_PROXY_TRUNK = (ControlFlags.Root | ControlFlags.Player | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_MASK = (ControlFlags.Root | ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_NPC_MINE_BRANCH = (ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_NPC_MINE_TRUNK = (ControlFlags.Root | ControlFlags.Local | ControlFlags.Owned);
    private const ControlFlags OWNER_NET_TREE_NPC_PROXY_BRANCH = ControlFlags.Owned;
    private const ControlFlags OWNER_NET_TREE_NPC_PROXY_TRUNK = (ControlFlags.Root | ControlFlags.Owned);
    private const ControlFlags OWNER_NPC = ControlFlags.Owned;
    private const ControlFlags PERSISTANT_FLAGS = (ControlFlags.Strong | ControlFlags.Root);
    [NonSerialized]
    private int RT;
    private const int RT_DEMOTED_ONCE = 0x100;
    private const int RT_DESTROY_LOCK = 0x20;
    private const int RT_DESTROY_STATE = 0xc00;
    private const int RT_ENTER_LOCK = 8;
    private const int RT_ENTERED = 1;
    private const int RT_ENTERED_ONCE = 0x40;
    private const int RT_EXITED_ONCE = 0x200;
    private const int RT_IS_DESTROYED = 0x800;
    private const int RT_ONCE = 960;
    private const int RT_PROMO_LOCK = 0x10;
    private const int RT_PROMOTED = 3;
    private const int RT_PROMOTED_ONCE = 0x80;
    private const int RT_RPC_CONTROL = 0x3000;
    private const int RT_RPC_CONTROL_0 = 0x1000;
    private const int RT_RPC_CONTROL_1 = 0x2000;
    private const int RT_RPC_CONTROL_2 = 0x3000;
    private const int RT_STATE = 3;
    private const int RT_WILL_DESTROY = 0x400;
    private const ControlFlags SETUP_INITIALIZED = ControlFlags.Initialized;
    private const ControlFlags SETUP_MASK = ControlFlags.Initialized;
    private const ControlFlags SETUP_UNINITIALIZED = 0;
    private const ControlFlags TRANSFERED_FLAGS = (ControlFlags.Player | ControlFlags.Local);
    private const ControlFlags TREE_BRANCH = 0;
    private const ControlFlags TREE_MASK = ControlFlags.Root;
    private const ControlFlags TREE_TRUNK = ControlFlags.Root;

    public static  event DestroyInContextQuery onDestroyInContextQuery;

    public bool AssignedControlOf(Controllable controllable)
    {
        return (this.ch.vl && (this == controllable));
    }

    public bool AssignedControlOf(Controller controller)
    {
        return ((this.ch.vl && (this._controller == controller)) && ((bool) this._controller));
    }

    public bool AssignedControlOf(IDBase idBase)
    {
        return ((this.ch.vl && (idBase != null)) && (base.idMain == idBase.idMain));
    }

    public bool AssignedControlOf(IDMain character)
    {
        return (this.ch.vl && (base.idMain == character));
    }

    private static int CAP_DEMOTE(int cmd, int RT, ControlFlags F)
    {
        cmd = CAP_THIS(cmd, RT, F);
        if ((RT & 0x100) == 0x100)
        {
            cmd = (cmd & -1025) | 0x400;
            return cmd;
        }
        cmd = (cmd & -1025) | 0;
        return cmd;
    }

    private static int CAP_ENTER(int cmd, int RT, ControlFlags F)
    {
        cmd = CAP_THIS(cmd, RT, F);
        if ((RT & 0x40) == 0x40)
        {
            cmd |= (cmd & -1025) | 0x400;
            return cmd;
        }
        cmd |= (cmd & -1025) | 0;
        return cmd;
    }

    private static int CAP_EXIT(int cmd, int RT, ControlFlags F)
    {
        if ((RT & 0x200) == 0x200)
        {
            cmd |= (cmd & -1025) | 0x400;
            return cmd;
        }
        cmd |= (cmd & -1025) | 0;
        return cmd;
    }

    private static int CAP_PROMOTE(int cmd, int RT, ControlFlags F)
    {
        cmd = CAP_THIS(cmd, RT, F);
        if ((RT & 0x80) == 0x80)
        {
            cmd |= (cmd & -1025) | 0x400;
            return cmd;
        }
        cmd |= (cmd & -1025) | 0;
        return cmd;
    }

    private static int CAP_THIS(int cmd, int RT, ControlFlags F)
    {
        cmd &= -30721;
        if ((F & ControlFlags.Strong) == 0)
        {
            cmd |= 0;
        }
        else if ((cmd & 0x20) == 0x20)
        {
            cmd |= 0x1001;
        }
        else
        {
            cmd |= 0x1000;
        }
        if ((F & (ControlFlags.Player | ControlFlags.Owned)) == ControlFlags.Owned)
        {
            cmd |= 0;
        }
        else if ((F & (ControlFlags.Player | ControlFlags.Owned)) == (ControlFlags.Player | ControlFlags.Owned))
        {
            cmd |= 0x2000;
        }
        if ((F & ControlFlags.Local) == ControlFlags.Local)
        {
            cmd |= 0x4000;
        }
        else
        {
            cmd |= 0;
        }
        if ((F & ControlFlags.Root) == ControlFlags.Root)
        {
            cmd |= 0x800;
        }
        else
        {
            cmd |= 0;
        }
        if (((RT & 0xc00) != 0) || ((cmd & 0x1020) == 0x1020))
        {
            cmd |= 1;
        }
        return cmd;
    }

    private void CL_Clear()
    {
        this.ClearBinder();
    }

    private void CL_OverideControlOf(NetworkViewID rootViewID, NetworkViewID parentViewID, ref NetworkMessageInfo info)
    {
        this.ClearBinder();
        this._binder = new CL_Binder(this, rootViewID, parentViewID, ref info);
        if (this._binder.CanLink())
        {
            this._binder.Link();
        }
    }

    private void CL_Refresh(int top)
    {
        this._refreshedControlCount = top;
        if (this._pendingControlCount > this._refreshedControlCount)
        {
            if (this._rootCountTimeStamps != null)
            {
                this._rootCountTimeStamps.RemoveRange(top, this._rootCountTimeStamps.Count - top);
            }
            this._pendingControlCount = top;
        }
        if (this.ch.su == top)
        {
            this.ch.RefreshEngauge();
        }
        else
        {
            CL_Binder.StaticLink(this);
        }
    }

    private void CL_RootControlCountSet(int count, ref NetworkMessageInfo info)
    {
        if (this._rootCountTimeStamps == null)
        {
            this._rootCountTimeStamps = new List<ulong>();
        }
        int num = this._rootCountTimeStamps.Count;
        if (num < count)
        {
            while (num++ < (count - 1))
            {
                this._rootCountTimeStamps.Add(ulong.MaxValue);
            }
            this._rootCountTimeStamps.Add(info.timestampInMillis);
        }
        else
        {
            if (num > count)
            {
                this._rootCountTimeStamps.RemoveRange(count, num - count);
            }
            this._rootCountTimeStamps[count - 1] = info.timestampInMillis;
        }
        this._pendingControlCount = count;
    }

    [RPC, Obsolete("RPC call only. Do not call through script", false)]
    private void CLD(NetworkMessageInfo info)
    {
    }

    private void ClearBinder()
    {
        if (this._binder != null)
        {
            this._binder.Dispose();
        }
    }

    public void ClientExit()
    {
        if (this.ch.vl)
        {
            if (this.ch.vl && (this.ch.bt == this.ch.it))
            {
                Debug.LogWarning("You cannot exit the root controllable", this);
            }
            else
            {
                if (!this.localControlled)
                {
                    throw new InvalidOperationException("Cannot exit other owned controllables");
                }
                base.networkView.RPC("Controllable:CLD", RPCMode.Server, new object[0]);
            }
        }
    }

    [Obsolete("RPC call only. Do not call through script", false), RPC]
    private void CLR(NetworkMessageInfo info)
    {
        this.ch.Delete();
        this.SharedPostCLR();
    }

    private void ControlCease(int cmd)
    {
        this._controller.ControlCease(cmd);
    }

    private void ControlEngauge(int cmd)
    {
        this._controller.ControlEngauge(cmd);
    }

    private void ControlEnter(int cmd)
    {
        try
        {
            this._controller.ControlEnter(cmd);
        }
        finally
        {
            if ((this.F & (ControlFlags.Root | ControlFlags.Player | ControlFlags.Owned)) == (ControlFlags.Root | ControlFlags.Player | ControlFlags.Owned))
            {
                try
                {
                    this._playerClient.OnRootControllableEntered(this);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception, this);
                }
                if ((this.F & ControlFlags.Local) == ControlFlags.Local)
                {
                    localPlayerControllableCount++;
                    LocalOnly.rootLocalPlayerControllables.Add(this);
                }
            }
        }
    }

    private void ControlExit(int cmd)
    {
        try
        {
            this._controller.ControlExit(cmd);
        }
        finally
        {
            if ((this.F & (ControlFlags.Root | ControlFlags.Player | ControlFlags.Owned)) == (ControlFlags.Root | ControlFlags.Player | ControlFlags.Owned))
            {
                if (this._playerClient != null)
                {
                    try
                    {
                        this._playerClient.OnRootControllableExited(this);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception, this);
                    }
                }
                if ((this.F & ControlFlags.Local) == ControlFlags.Local)
                {
                    localPlayerControllableCount--;
                    LocalOnly.rootLocalPlayerControllables.Remove(this);
                }
            }
        }
    }

    public bool ControlOverriddenBy(Character character)
    {
        Controllable controllable;
        return ((((this.ch.vl && (this.ch.ln > 0)) && ((character != null) && ((controllable = character.controllable) != null))) && (controllable.ch.vl && (this.ch.ln > controllable.ch.ln))) && (this.ch.bt == controllable.ch.bt));
    }

    public bool ControlOverriddenBy(Controllable controllable)
    {
        return ((((this.ch.vl && (this.ch.ln > 0)) && ((controllable != null) && controllable.ch.vl)) && (this.ch.ln > controllable.ch.ln)) && (this.ch.bt == controllable.ch.bt));
    }

    public bool ControlOverriddenBy(Controller controller)
    {
        Controllable controllable;
        return ((((this.ch.vl && (this.ch.ln > 0)) && ((controller != null) && ((controllable = controller.controllable) != null))) && (controllable.ch.vl && (this.ch.ln > controllable.ch.ln))) && (this.ch.bt == controllable.ch.bt));
    }

    public bool ControlOverriddenBy(IDBase idBase)
    {
        return (((this.ch.vl && (this.ch.ln != 0)) && (idBase != null)) && this.ControlOverriddenBy(idBase.idMain));
    }

    public bool ControlOverriddenBy(IDLocalCharacter idLocal)
    {
        return (((this.ch.vl && (this.ch.ln != 0)) && (idLocal != null)) && this.ControlOverriddenBy(idLocal.idMain));
    }

    public bool ControlOverriddenBy(IDMain main)
    {
        return (((this.ch.vl && (this.ch.ln != 0)) && (main is Character)) && this.ControlOverriddenBy((Character) main));
    }

    [DebuggerHidden]
    public static IEnumerable<Controllable> CurrentControllers(IEnumerable<PlayerClient> playerClients)
    {
        return new <CurrentControllers>c__Iterator22 { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    private static void DO_DEMOTE(int cmd, Controllable citr)
    {
        if ((citr.RT & 0x10) != 0x10)
        {
            citr.RT |= 0x10;
            citr.ControlCease(cmd);
            citr.RT = (citr.RT & -20) | 0x101;
        }
    }

    private static void DO_ENTER(int cmd, Controllable citr)
    {
        if ((citr.RT & 8) != 8)
        {
            citr.RT |= 8;
            citr.ControlEnter(cmd);
            citr.RT = (citr.RT & -12) | 0x41;
        }
    }

    private static void DO_EXIT(int cmd, Controllable citr)
    {
        if ((citr.RT & 8) != 8)
        {
            citr.RT |= 8;
            citr.ControlExit(cmd);
            citr.RT = (citr.RT & -12) | 0x200;
        }
    }

    private static void DO_PROMOTE(int cmd, Controllable citr)
    {
        if ((citr.RT & 0x10) != 0x10)
        {
            citr.RT |= 0x10;
            citr.ControlEngauge(cmd);
            citr.RT = (citr.RT & -20) | 0x83;
        }
    }

    private void DoDestroy()
    {
        this.CL_Clear();
        try
        {
            this.RT |= 0x20;
            if ((this.RT & 3) != 0)
            {
                this.ch.Delete();
            }
        }
        finally
        {
            this.RT &= -33;
        }
    }

    private bool EnsureControllee(NetworkPlayer player)
    {
        if (!this.controlled)
        {
            return false;
        }
        if (player.isClient)
        {
            if (!this.playerControlled || ((this.playerClient != null) && (this.playerClient.netPlayer != player)))
            {
                Debug.LogWarning("player was not the controllee of this player controlled controlable", this);
                return false;
            }
        }
        else if (this.playerControlled)
        {
            Debug.LogWarning("this player controlled controlable is not server owned", this);
            return false;
        }
        return true;
    }

    internal void FreshInitializeController()
    {
        if (this.__controllerDriverViewID == NetworkViewID.unassigned)
        {
            if ((this.F & ControlFlags.Initialized) == ControlFlags.Initialized)
            {
                throw new InvalidOperationException("Was already intialized.");
            }
            Chain.ROOT(this);
            this.F = ControlFlags.Root;
            this.InitializeController_OnFoundOverriding(null);
        }
        else
        {
            NetworkView driverView = NetworkView.Find(this.__controllerDriverViewID);
            this.F |= 0;
            this.InitializeController_OnFoundOverriding(driverView);
        }
    }

    [Conditional("LOG_CONTROL_CHANGE")]
    private static void GuardState(string state, Controllable self)
    {
    }

    [RPC, Obsolete("RPC call only. Do not call through script", false)]
    private void ID1()
    {
        this.SetIdle(true);
    }

    private void InitializeController_OnFoundOverriding(NetworkView driverView)
    {
        if ((this.F & ControlFlags.Root) == 0)
        {
            Character idMain = driverView.idMain as Character;
            Controllable controllable = idMain.controllable;
            this.F = (this.F & (ControlFlags.Strong | ControlFlags.Root)) | (controllable.F & (ControlFlags.Player | ControlFlags.Local));
            this._playerClient = controllable.playerClient;
            controllable.ch.Add(this);
        }
        else
        {
            this.F |= !this.__networkViewForControllable.isMine ? 0 : ControlFlags.Local;
            this.F |= !PlayerClient.Find(this.__networkViewForControllable.owner, out this._playerClient) ? ControlFlags.Owned : (ControlFlags.Player | ControlFlags.Owned);
        }
        this.F |= ControlFlags.Owned;
        string controllerClassName = this.controllerClassName;
        if (string.IsNullOrEmpty(controllerClassName))
        {
            ControlFlags f = this.F;
            this.F = 0;
            throw new ArgumentOutOfRangeException("@class", f, "The ControllerClass did not support given flags");
        }
        Controller controller = null;
        try
        {
            controller = base.AddAddon<Controller>(controllerClassName);
            if (controller == null)
            {
                throw new ArgumentOutOfRangeException("className", controllerClassName, "classname as not a Controller!");
            }
            this._controller = controller;
            Controller controller2 = this._controller;
            try
            {
                try
                {
                    this._controller.ControllerSetup(this, this.__networkViewForControllable, ref this.__controllerCreateMessageInfo);
                }
                catch
                {
                    this._controller = controller2;
                    throw;
                }
            }
            catch
            {
                throw;
            }
            this.F |= ControlFlags.Initialized;
        }
        catch
        {
            if (controller != null)
            {
                Object.Destroy(controller);
            }
            this.ch.Delete();
            throw;
        }
    }

    [Conditional("LOG_CONTROL_CHANGE")]
    private static void LogState(bool guard, string state, Controllable controllable)
    {
        Debug.Log(string.Format("{2}{0}::{1}", controllable.GetType().Name, state, !guard ? "-" : "+"), controllable);
    }

    internal bool MergeClasses(ref ControllerClass.Merge merge)
    {
        return ((this.@class != null) && merge.Add(this.controllable.@class));
    }

    internal static bool MergeClasses(IDMain character, ref ControllerClass.Merge merge)
    {
        Controllable controllable;
        return (((character != null) && ((controllable = character.GetComponent<Controllable>()) != null)) && controllable.MergeClasses(ref merge));
    }

    private void Net_Shutdown_Exit()
    {
    }

    [RPC]
    private void OC1(NetworkViewID rootViewID, NetworkMessageInfo info)
    {
        this.OverrideControlOfHandleRPC(rootViewID, rootViewID, ref info);
    }

    [RPC]
    private void OC2(NetworkViewID rootViewID, NetworkViewID parentViewID, NetworkMessageInfo info)
    {
        this.OverrideControlOfHandleRPC(rootViewID, parentViewID, ref info);
    }

    private void OCO_FOUND(NetworkViewID viewID, ref NetworkMessageInfo info)
    {
        this.SetIdle(false);
        this.__networkViewForControllable = base.networkView;
        this.__controllerDriverViewID = viewID;
        this.__controllerCreateMessageInfo = info;
        this.FreshInitializeController();
    }

    private void ON_CHAIN_ABOLISHED()
    {
    }

    private void ON_CHAIN_ERASE(int cmd)
    {
    }

    private void ON_CHAIN_RENEW()
    {
    }

    private void ON_CHAIN_SUBSCRIBE()
    {
    }

    private void OnDestroy()
    {
        this.CL_Clear();
        if (this.isInContextQuery)
        {
            try
            {
                if (onDestroyInContextQuery != null)
                {
                    onDestroyInContextQuery(this);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception, this);
            }
            finally
            {
                this.isInContextQuery = false;
            }
        }
        this.RT |= 0x800;
        if ((this.RT & 0x420) == 0)
        {
            this.DoDestroy();
        }
    }

    internal void OnInstantiated()
    {
        if ((this.F & ControlFlags.Root) == ControlFlags.Root)
        {
            this.ch.RefreshEngauge();
        }
    }

    private void OverrideControlOfHandleRPC(NetworkViewID rootViewID, NetworkViewID parentViewID, ref NetworkMessageInfo info)
    {
        this.CL_OverideControlOf(rootViewID, parentViewID, ref info);
    }

    public bool OverridingControlOf(Character character)
    {
        Controllable controllable;
        return ((((this.ch.vl && (this.ch.nm > 0)) && ((character != null) && ((controllable = character.controllable) != null))) && (controllable.ch.vl && (this.ch.nm > controllable.ch.nm))) && (this.ch.bt == controllable.ch.bt));
    }

    public bool OverridingControlOf(Controllable controllable)
    {
        return ((((this.ch.vl && (this.ch.nm > 0)) && ((controllable != null) && controllable.ch.vl)) && (this.ch.nm > controllable.ch.nm)) && (this.ch.bt == controllable.ch.bt));
    }

    public bool OverridingControlOf(Controller controller)
    {
        Controllable controllable;
        return ((((this.ch.vl && (this.ch.nm > 0)) && ((controller != null) && ((controllable = controller.controllable) != null))) && (controllable.ch.vl && (this.ch.nm > controllable.ch.nm))) && (this.ch.bt == controllable.ch.bt));
    }

    public bool OverridingControlOf(IDBase idBase)
    {
        return (((this.ch.vl && (this.ch.nm != 0)) && (idBase != null)) && this.OverridingControlOf(idBase.idMain));
    }

    public bool OverridingControlOf(IDLocalCharacter idLocal)
    {
        return (((this.ch.vl && (this.ch.nm != 0)) && (idLocal != null)) && this.OverridingControlOf(idLocal.idMain));
    }

    public bool OverridingControlOf(IDMain main)
    {
        return (((this.ch.vl && (this.ch.nm != 0)) && (main is Character)) && this.OverridingControlOf((Character) main));
    }

    internal void PrepareInstantiate(NetworkView view, ref NetworkMessageInfo info)
    {
        this.__controllerCreateMessageInfo = info;
        this.__networkViewForControllable = view;
        if (this.classFlagsRootControllable || this.classFlagsStandaloneVessel)
        {
            this.__controllerDriverViewID = NetworkViewID.unassigned;
            if (this.classFlagsStandaloneVessel)
            {
                return;
            }
        }
        else if (this.classFlagsDependantVessel || this.classFlagsFreeVessel)
        {
            PlayerClient client;
            if (PlayerClient.Find(view.owner, out client))
            {
                this.__controllerDriverViewID = client.topControllable.networkViewID;
            }
            else
            {
                this.__controllerDriverViewID = NetworkViewID.unassigned;
            }
            if (this.classFlagsFreeVessel)
            {
                return;
            }
            if (this.__controllerDriverViewID == NetworkViewID.unassigned)
            {
                Debug.LogError("NOT RIGHT");
                return;
            }
        }
        this.FreshInitializeController();
    }

    internal void ProcessLocalPlayerPreRender()
    {
        this._controller.ProcessLocalPlayerPreRender();
    }

    public RelativeControl RelativeControlFrom(Character character)
    {
        if (character == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlFrom(character.controllable);
    }

    public RelativeControl RelativeControlFrom(Controllable controllable)
    {
        if ((!this.ch.vl || (controllable == null)) || (!controllable.ch.vl || (controllable.ch.bt != this.ch.bt)))
        {
            return RelativeControl.Incompatible;
        }
        if (this.ch.ln > controllable.ch.ln)
        {
            return RelativeControl.IsOverriding;
        }
        if (this.ch.ln < controllable.ch.ln)
        {
            return RelativeControl.OverriddenBy;
        }
        return RelativeControl.Assigned;
    }

    public RelativeControl RelativeControlFrom(Controller controller)
    {
        Controllable controllable;
        if ((!this.ch.vl || (controller == null)) || ((((controllable = controller.controllable) == null) || controllable.ch.vl) || (controllable.ch.bt != this.ch.bt)))
        {
            return RelativeControl.Incompatible;
        }
        if (this.ch.ln > controllable.ch.ln)
        {
            return RelativeControl.IsOverriding;
        }
        if (this.ch.ln < controllable.ch.ln)
        {
            return RelativeControl.OverriddenBy;
        }
        return RelativeControl.Assigned;
    }

    public RelativeControl RelativeControlFrom(IDBase idBase)
    {
        if (idBase == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlFrom(idBase.idMain as Character);
    }

    public RelativeControl RelativeControlFrom(IDLocalCharacter idLocal)
    {
        if (idLocal == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlFrom(idLocal.idMain.controllable);
    }

    public RelativeControl RelativeControlFrom(IDMain idMain)
    {
        if (idMain is Character)
        {
            return this.RelativeControlFrom((Character) idMain);
        }
        return RelativeControl.Incompatible;
    }

    public RelativeControl RelativeControlTo(Character character)
    {
        if (character == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlTo(character.controllable);
    }

    public RelativeControl RelativeControlTo(Controllable controllable)
    {
        if ((!this.ch.vl || (controllable == null)) || (!controllable.ch.vl || (controllable.ch.bt != this.ch.bt)))
        {
            return RelativeControl.Incompatible;
        }
        if (this.ch.ln > controllable.ch.ln)
        {
            return RelativeControl.OverriddenBy;
        }
        if (this.ch.ln < controllable.ch.ln)
        {
            return RelativeControl.IsOverriding;
        }
        return RelativeControl.Assigned;
    }

    public RelativeControl RelativeControlTo(Controller controller)
    {
        Controllable controllable;
        if ((!this.ch.vl || (controller == null)) || ((((controllable = controller.controllable) == null) || controllable.ch.vl) || (controllable.ch.bt != this.ch.bt)))
        {
            return RelativeControl.Incompatible;
        }
        if (this.ch.ln > controllable.ch.ln)
        {
            return RelativeControl.OverriddenBy;
        }
        if (this.ch.ln < controllable.ch.ln)
        {
            return RelativeControl.IsOverriding;
        }
        return RelativeControl.Assigned;
    }

    public RelativeControl RelativeControlTo(IDBase idBase)
    {
        if (idBase == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlTo(idBase.idMain as Character);
    }

    public RelativeControl RelativeControlTo(IDLocalCharacter idLocal)
    {
        if (idLocal == null)
        {
            return RelativeControl.Incompatible;
        }
        return this.RelativeControlTo(idLocal.idMain.controllable);
    }

    public RelativeControl RelativeControlTo(IDMain idMain)
    {
        if (idMain is Character)
        {
            return this.RelativeControlTo((Character) idMain);
        }
        return RelativeControl.Incompatible;
    }

    [RPC]
    private void RFH(byte top)
    {
        this.CL_Refresh(top);
    }

    private void RN(int n, ref NetworkMessageInfo info)
    {
        this.CL_RootControlCountSet(n, ref info);
    }

    [RPC]
    private void RN0(NetworkMessageInfo info)
    {
        this.RN(0, ref info);
    }

    [RPC]
    private void RN1(NetworkMessageInfo info)
    {
        this.RN(1, ref info);
    }

    [RPC]
    private void RN2(NetworkMessageInfo info)
    {
        this.RN(2, ref info);
    }

    [RPC]
    private void RN3(NetworkMessageInfo info)
    {
        this.RN(3, ref info);
    }

    [RPC]
    private void RN4(NetworkMessageInfo info)
    {
        this.RN(4, ref info);
    }

    [RPC]
    private void RN5(NetworkMessageInfo info)
    {
        this.RN(5, ref info);
    }

    [RPC]
    private void RN6(NetworkMessageInfo info)
    {
        this.RN(6, ref info);
    }

    [RPC]
    private void RN7(NetworkMessageInfo info)
    {
        this.RN(7, ref info);
    }

    [DebuggerHidden]
    public static IEnumerable<Controllable> RootControllers(IEnumerable<PlayerClient> playerClients)
    {
        return new <RootControllers>c__Iterator21 { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    private bool SetIdle(bool idle)
    {
        IDLocalCharacterIdleControl idleControl = base.idMain.idleControl;
        if (idleControl != null)
        {
            try
            {
                return idleControl.SetIdle(idle);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception, idleControl);
                return true;
            }
        }
        return false;
    }

    [Obsolete("Used only by PlayerClient")]
    internal void SetRootPlayer(PlayerClient rootPlayer)
    {
    }

    private void SharedPostCLR()
    {
        if (this._controller != null)
        {
            Object.Destroy(this._controller);
        }
        this.F &= ControlFlags.Strong | ControlFlags.Root;
        this.RT = 0;
        this._playerClient = null;
        this._controller = null;
        this.SetIdle(true);
    }

    [Conditional("LOG_CONTROL_CHANGE")]
    private static void UnguardState(string state, Controllable self)
    {
    }

    public bool aiControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Owned)) == ControlFlags.Owned);
        }
    }

    public Controllable aiControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Owned)) != ControlFlags.Owned) ? null : this);
        }
    }

    public Controller aiControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Owned)) != ControlFlags.Owned) ? null : this._controller);
        }
    }

    public bool assignedControl
    {
        get
        {
            return this.ch.vl;
        }
    }

    internal bool classAssigned
    {
        get
        {
            return (bool) this.@class;
        }
    }

    internal bool classFlagsAISupport
    {
        get
        {
            return ((this.@class != null) && this.@class.DefinesClass(false));
        }
    }

    internal bool classFlagsDependantVessel
    {
        get
        {
            return ((this.@class != null) && this.@class.vesselDependant);
        }
    }

    internal bool classFlagsFreeVessel
    {
        get
        {
            return ((this.@class != null) && this.@class.vesselFree);
        }
    }

    internal bool classFlagsPlayerSupport
    {
        get
        {
            return ((this.@class != null) && this.@class.DefinesClass(true));
        }
    }

    internal bool classFlagsRootControllable
    {
        get
        {
            return ((this.@class != null) && this.@class.root);
        }
    }

    internal bool classFlagsStandaloneVessel
    {
        get
        {
            return ((this.@class != null) && this.@class.vesselStandalone);
        }
    }

    internal bool classFlagsStaticGroup
    {
        get
        {
            return ((this.@class != null) && this.@class.staticGroup);
        }
    }

    internal bool classFlagsVessel
    {
        get
        {
            return ((this.@class != null) && this.@class.vessel);
        }
    }

    public int controlCount
    {
        get
        {
            return this.ch.su;
        }
    }

    public int controlDepth
    {
        get
        {
            return this.ch.id;
        }
    }

    public Controllable controllable
    {
        get
        {
            return this;
        }
    }

    public bool controlled
    {
        get
        {
            return ((this.F & ControlFlags.Owned) == ControlFlags.Owned);
        }
    }

    public Controllable controlledControllable
    {
        get
        {
            return (((this.F & ControlFlags.Owned) != ControlFlags.Owned) ? null : this);
        }
    }

    public Controller controlledController
    {
        get
        {
            return (((this.F & ControlFlags.Owned) != ControlFlags.Owned) ? null : this._controller);
        }
    }

    public Controller controller
    {
        get
        {
            return this._controller;
        }
    }

    public string controllerClassName
    {
        get
        {
            return ((this.@class == null) ? null : this.@class.GetClassName((this.F & (ControlFlags.Player | ControlFlags.Owned)) == (ControlFlags.Player | ControlFlags.Owned), (this.F & ControlFlags.Local) == ControlFlags.Local));
        }
    }

    public bool controlOverridden
    {
        get
        {
            return (this.ch.vl && (this.ch.ln > 0));
        }
    }

    public bool core
    {
        get
        {
            return ((this.F & ControlFlags.Root) == ControlFlags.Root);
        }
    }

    public bool doesNotSave
    {
        get
        {
            return ((this._controller == null) || this._controller.doesNotSave);
        }
    }

    public bool forwardsPlayerClientInput
    {
        get
        {
            return ((this._controller != null) && this._controller.forwardsPlayerClientInput);
        }
    }

    public bool localAIControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) == (ControlFlags.Local | ControlFlags.Owned));
        }
    }

    public Controllable localAIControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Local | ControlFlags.Owned)) ? null : this);
        }
    }

    public Controller localAIControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Local | ControlFlags.Owned)) ? null : this._controller);
        }
    }

    public bool localControlled
    {
        get
        {
            return ((this.F & ControlFlags.Local) == ControlFlags.Local);
        }
    }

    public static Controllable localPlayerControllable
    {
        get
        {
            switch (localPlayerControllableCount)
            {
                case 0:
                    return null;

                case 1:
                    return LocalOnly.rootLocalPlayerControllables[0];
            }
            return LocalOnly.rootLocalPlayerControllables[localPlayerControllableCount - 1];
        }
    }

    public static bool localPlayerControllableExists
    {
        get
        {
            return (localPlayerControllableCount > 0);
        }
    }

    public bool localPlayerControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) == (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned));
        }
    }

    public Controllable localPlayerControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) ? null : this);
        }
    }

    public Controller localPlayerControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) ? null : this._controller);
        }
    }

    public Character masterCharacter
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.tp.idMain);
        }
    }

    public Controllable masterControllable
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.tp);
        }
    }

    public Controller masterController
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.tp._controller);
        }
    }

    public NetworkPlayer netPlayer
    {
        get
        {
            return ((this._playerClient == null) ? NetworkPlayer.unassigned : this._playerClient.netPlayer);
        }
    }

    public Character nextCharacter
    {
        get
        {
            return ((!this.ch.vl || !this.ch.up.vl) ? null : this.ch.up.it.idMain);
        }
    }

    public Controllable nextControllable
    {
        get
        {
            return ((!this.ch.vl || !this.ch.up.vl) ? null : this.ch.up.it);
        }
    }

    public Controller nextController
    {
        get
        {
            return ((!this.ch.vl || !this.ch.up.vl) ? null : this.ch.up.it._controller);
        }
    }

    public string npcName
    {
        get
        {
            return ((this.@class == null) ? null : this.@class.npcName);
        }
    }

    public bool overridingControl
    {
        get
        {
            return (this.ch.vl && (this.ch.nm > 0));
        }
    }

    public PlayerClient playerClient
    {
        get
        {
            return this._playerClient;
        }
    }

    public bool playerControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Owned)) == (ControlFlags.Player | ControlFlags.Owned));
        }
    }

    public Controllable playerControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Owned)) ? null : this);
        }
    }

    public Controller playerControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Owned)) ? null : this._controller);
        }
    }

    public static IEnumerable<Controllable> PlayerCurrentControllables
    {
        get
        {
            return new <>c__Iterator20 { $PC = -2 };
        }
    }

    public static IEnumerable<Controllable> PlayerRootControllables
    {
        get
        {
            return new <>c__Iterator1F { $PC = -2 };
        }
    }

    public Character previousCharacter
    {
        get
        {
            return ((!this.ch.vl || !this.ch.dn.vl) ? null : this.ch.dn.it.idMain);
        }
    }

    public Controllable previousControllable
    {
        get
        {
            return ((!this.ch.vl || !this.ch.dn.vl) ? null : this.ch.dn.it);
        }
    }

    public Controller previousController
    {
        get
        {
            return ((!this.ch.vl || !this.ch.dn.vl) ? null : this.ch.dn.it._controller);
        }
    }

    public bool remoteAIControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) == ControlFlags.Owned);
        }
    }

    public Controllable remoteAIControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != ControlFlags.Owned) ? null : this);
        }
    }

    public Controller remoteAIControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != ControlFlags.Owned) ? null : this._controller);
        }
    }

    public bool remoteControlled
    {
        get
        {
            return ((this.F & ControlFlags.Local) == 0);
        }
    }

    public bool remotePlayerControlled
    {
        get
        {
            return ((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) == (ControlFlags.Player | ControlFlags.Owned));
        }
    }

    public Controllable remotePlayerControlledControllable
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Owned)) ? null : this);
        }
    }

    public Controller remotePlayerControlledController
    {
        get
        {
            return (((this.F & (ControlFlags.Player | ControlFlags.Local | ControlFlags.Owned)) != (ControlFlags.Player | ControlFlags.Owned)) ? null : this._controller);
        }
    }

    public Character rootCharacter
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.bt.idMain);
        }
    }

    public Controllable rootControllable
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.bt);
        }
    }

    public Controller rootController
    {
        get
        {
            return (!this.ch.vl ? null : this.ch.bt._controller);
        }
    }

    public RPOSLimitFlags rposLimitFlags
    {
        get
        {
            return ((this._controller == null) ? ((RPOSLimitFlags) (-1)) : this._controller.rposLimitFlags);
        }
    }

    public bool vessel
    {
        get
        {
            return ((this.F & ControlFlags.Root) == 0);
        }
    }

    [CompilerGenerated]
    private sealed class <>c__Iterator1F : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controllable>, IEnumerator<Controllable>
    {
        internal Controllable $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_199>__0;
        internal Controllable <controllable>__2;
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
                        this.<$s_199>__0.Dispose();
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
                    this.<$s_199>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C4;
            }
            try
            {
                while (this.<$s_199>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_199>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2;
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
                this.<$s_199>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C4:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controllable> IEnumerable<Controllable>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controllable.<>c__Iterator1F();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controllable>.GetEnumerator();
        }

        Controllable IEnumerator<Controllable>.Current
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
    private sealed class <>c__Iterator20 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controllable>, IEnumerator<Controllable>
    {
        internal Controllable $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_200>__0;
        internal Controllable <controllable>__2;
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
                        this.<$s_200>__0.Dispose();
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
                    this.<$s_200>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C4;
            }
            try
            {
                while (this.<$s_200>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_200>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2;
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
                this.<$s_200>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C4:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controllable> IEnumerable<Controllable>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controllable.<>c__Iterator20();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controllable>.GetEnumerator();
        }

        Controllable IEnumerator<Controllable>.Current
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
    private sealed class <CurrentControllers>c__Iterator22 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controllable>, IEnumerator<Controllable>
    {
        internal Controllable $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_202>__0;
        internal Controllable <controllable>__2;
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
                        if (this.<$s_202>__0 == null)
                        {
                        }
                        this.<$s_202>__0.Dispose();
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
                    this.<$s_202>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C9;
            }
            try
            {
                while (this.<$s_202>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_202>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2;
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
                if (this.<$s_202>__0 == null)
                {
                }
                this.<$s_202>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controllable> IEnumerable<Controllable>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controllable.<CurrentControllers>c__Iterator22 { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controllable>.GetEnumerator();
        }

        Controllable IEnumerator<Controllable>.Current
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
    private sealed class <RootControllers>c__Iterator21 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Controllable>, IEnumerator<Controllable>
    {
        internal Controllable $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_201>__0;
        internal Controllable <controllable>__2;
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
                        if (this.<$s_201>__0 == null)
                        {
                        }
                        this.<$s_201>__0.Dispose();
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
                    this.<$s_201>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C9;
            }
            try
            {
                while (this.<$s_201>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_201>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2;
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
                if (this.<$s_201>__0 == null)
                {
                }
                this.<$s_201>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Controllable> IEnumerable<Controllable>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Controllable.<RootControllers>c__Iterator21 { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Controllable>.GetEnumerator();
        }

        Controllable IEnumerator<Controllable>.Current
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

    [StructLayout(LayoutKind.Sequential)]
    private struct Chain
    {
        public Controllable it;
        public Controllable bt;
        public Controllable tp;
        public Controllable.Link dn;
        public Controllable.Link up;
        public byte nm;
        public byte ln;
        public bool vl;
        public bool iv;
        public int id
        {
            get
            {
                return (!this.vl ? -1 : this.nm);
            }
        }
        public int su
        {
            get
            {
                return (!this.vl ? -1 : ((1 + this.nm) + this.ln));
            }
        }
        public static void ROOT(Controllable root)
        {
            root.ch.it = root.ch.bt = root.ch.tp = root;
            root.ch.vl = true;
            root.ch.dn.vl = root.ch.up.vl = false;
            root.ch.dn.it = (Controllable) (root.ch.up.it = null);
            root.ch.nm = (byte) (root.ch.ln = 0);
            root.ch.iv = true;
        }

        private bool Add(ref Controllable.Chain nw, Controllable ct)
        {
            if (!this.vl || nw.vl)
            {
                return false;
            }
            nw.it = ct;
            nw.it.ON_CHAIN_RENEW();
            this.tp.ch.up.vl = true;
            this.tp.ch.up.it = nw.it;
            nw.dn.vl = true;
            nw.dn.it = this.tp;
            nw.nm = this.tp.ch.nm;
            nw.nm = (byte) (nw.nm + 1);
            nw.ln = 0;
            nw.up.vl = false;
            nw.up.it = null;
            nw.tp = nw.it;
            nw.bt = this.tp.ch.bt;
            nw.vl = true;
            Controllable.Link dn = nw.dn;
            nw.iv = true;
            do
            {
                dn.it.ch.tp = nw.tp;
                dn.it.ch.ln = (byte) (dn.it.ch.ln + 1);
                dn.it.ch.iv = true;
                dn = dn.it.ch.dn;
            }
            while (dn.vl);
            nw.it.ON_CHAIN_SUBSCRIBE();
            return true;
        }

        public bool Add(Controllable vessel)
        {
            return ((vessel != null) && this.Add(ref vessel.ch, vessel));
        }

        public bool RefreshEngauge()
        {
            int num;
            if (!this.vl)
            {
                return false;
            }
            if (!this.tp.ch.iv)
            {
                goto Label_01CC;
            }
            if (!this.bt.ch.up.vl)
            {
                num = 0;
                goto Label_00FC;
            }
            Controllable bt = this.bt;
            num = 0x80;
        Label_0049:
            bt.ch.iv = false;
            switch ((bt.RT & 3))
            {
                case 0:
                    Controllable.DO_ENTER(Controllable.CAP_ENTER(num, bt.RT, bt.F), bt);
                    break;

                case 3:
                    Controllable.DO_DEMOTE(Controllable.CAP_DEMOTE(num, bt.RT, bt.F), bt);
                    break;
            }
            num |= 0x300;
            if (bt.ch.up.vl)
            {
                bt = bt.ch.up.it;
                goto Label_0049;
            }
        Label_00FC:
            this.tp.ch.iv = false;
            switch ((this.tp.RT & 3))
            {
                case 0:
                    Controllable.DO_ENTER(Controllable.CAP_ENTER(num & -129, this.tp.RT, this.tp.F), this.tp);
                    Controllable.DO_PROMOTE(Controllable.CAP_PROMOTE(num & -129, this.tp.RT, this.tp.F), this.tp);
                    break;

                case 1:
                    Controllable.DO_PROMOTE(Controllable.CAP_PROMOTE(num & -129, this.tp.RT, this.tp.F), this.tp);
                    break;
            }
        Label_01CC:
            return true;
        }

        public bool RefreshEnter()
        {
            int num;
            if (!this.vl)
            {
                return false;
            }
            if (!this.tp.ch.iv)
            {
                goto Label_014F;
            }
            if (!this.bt.ch.up.vl)
            {
                num = 0;
                goto Label_00F0;
            }
            Controllable bt = this.bt;
            num = 0x80;
        Label_0049:
            switch ((bt.RT & 3))
            {
                case 0:
                    Controllable.DO_ENTER(Controllable.CAP_ENTER(num, bt.RT, bt.F), bt);
                    break;

                case 3:
                    Controllable.DO_DEMOTE(Controllable.CAP_DEMOTE(num, bt.RT, bt.F), bt);
                    break;
            }
            num |= 0x300;
            if (bt.ch.up.vl)
            {
                bt = bt.ch.up.it;
                goto Label_0049;
            }
        Label_00F0:
            switch ((this.tp.RT & 3))
            {
                case 0:
                    Controllable.DO_ENTER(Controllable.CAP_ENTER(num, this.tp.RT, this.tp.F), this.tp);
                    break;
            }
        Label_014F:
            return true;
        }

        public override string ToString()
        {
            if (!this.vl)
            {
                return "invalid";
            }
            StringBuilder builder = new StringBuilder();
            for (Controllable controllable = this.bt; controllable != null; controllable = !controllable.ch.up.vl ? null : controllable.ch.up.it)
            {
                if (controllable == this.it)
                {
                    builder.Append("-->");
                }
                else
                {
                    builder.Append("   ");
                }
                builder.AppendLine(controllable.name);
            }
            return builder.ToString();
        }

        public void Delete()
        {
            if (this.vl)
            {
                int cmd = Controllable.CAP_THIS(0x10, this.it.RT, this.it.F);
                if (this.up.vl)
                {
                    int ln = this.ln;
                    int num3 = (cmd & 0x91) << 1;
                    if (!this.dn.vl)
                    {
                        num3 |= (cmd & 0x91) << 2;
                    }
                    do
                    {
                        int num4;
                        Controllable controllable = this.tp.ch.dn.it;
                        Controllable tp = this.tp;
                        switch ((tp.RT & 3))
                        {
                            case 1:
                                Controllable.DO_EXIT(num4 = Controllable.CAP_EXIT(num3, tp.RT, tp.F), tp);
                                break;

                            case 3:
                                num4 = Controllable.CAP_EXIT(num3, tp.RT, tp.F);
                                Controllable.DO_DEMOTE(Controllable.CAP_DEMOTE(num4, tp.RT, tp.F), tp);
                                Controllable.DO_EXIT(num4, tp);
                                break;

                            default:
                                num4 = Controllable.CAP_THIS(num3, tp.RT, tp.F);
                                break;
                        }
                        tp.ON_CHAIN_ERASE(num4);
                        Controllable.Chain chain = new Controllable.Chain();
                        tp.ch = chain;
                        tp.ON_CHAIN_ABOLISHED();
                        this.tp = controllable;
                        Controllable.Link link3 = new Controllable.Link();
                        this.tp.ch.up = link3;
                        this.tp.ch.ln = (byte) (this.tp.ch.ln - 1);
                        this.tp.ch.tp = this.tp;
                        Controllable.Link link = this.tp.ch.dn;
                        byte num5 = this.tp.ch.ln;
                        while (link.vl)
                        {
                            Controllable controllable3 = link.it;
                            link = controllable3.ch.dn;
                            controllable3.ch.tp = this.tp;
                            controllable3.ch.ln = num5 = (byte) (num5 - 1);
                        }
                    }
                    while (--ln > 0);
                }
                switch ((this.it.RT & 3))
                {
                    case 1:
                        Controllable.DO_EXIT(Controllable.CAP_EXIT(cmd, this.it.RT, this.it.F), this.it);
                        break;

                    case 3:
                        Controllable.DO_DEMOTE(Controllable.CAP_DEMOTE(cmd, this.it.RT, this.it.F), this.it);
                        Controllable.DO_EXIT(Controllable.CAP_EXIT(cmd, this.it.RT, this.it.F), this.it);
                        break;
                }
                Controllable it = this.it;
                it.ON_CHAIN_ERASE(cmd);
                Controllable.Link dn = this.dn;
                it.ch = this = new Controllable.Chain();
                if (dn.vl)
                {
                    Controllable controllable5 = dn.it;
                    controllable5.ch.up = new Controllable.Link();
                    int num6 = 0;
                    do
                    {
                        Controllable controllable6 = dn.it;
                        dn = controllable6.ch.dn;
                        controllable6.ch.iv = true;
                        controllable6.ch.tp = controllable5;
                        controllable6.ch.ln = (byte) num6++;
                    }
                    while (dn.vl);
                }
                it.ON_CHAIN_ABOLISHED();
            }
        }
    }

    private class CL_Binder : IDisposable
    {
        private NetworkMessageInfo _info;
        private Search _parent;
        private Search _root;
        private static int binderCount;
        private bool disposed;
        private static Controllable.CL_Binder first;
        private static Controllable.CL_Binder last;
        private Controllable.CL_Binder next;
        private readonly Controllable owner;
        private Controllable.CL_Binder prev;
        private readonly bool sameSearch;

        public CL_Binder(Controllable owner, NetworkViewID rootID, NetworkViewID parentID, ref NetworkMessageInfo info)
        {
            this._root.id = rootID;
            this._parent.id = parentID;
            this._info = info;
            this.owner = owner;
            this.sameSearch = this._root.id == this._parent.id;
            if (binderCount++ == 0)
            {
                first = last = this;
            }
            else
            {
                this.prev = last;
                this.prev.next = this;
                last = this;
            }
        }

        public bool CanLink()
        {
            if (this._root.Find() && (this._root.controllable._rootCountTimeStamps != null))
            {
                int introduced1 = this.CountValidate(this._root.controllable._rootCountTimeStamps, this._root.controllable._rootCountTimeStamps.Count);
                return (introduced1 == this._root.controllable._pendingControlCount);
            }
            return false;
        }

        protected int CountValidate(List<ulong> ts, int tsCount)
        {
            if (this.Find())
            {
                Controllable controllable = !this.sameSearch ? this._parent.controllable : this._root.controllable;
                if (this.sameSearch)
                {
                    if ((tsCount > 1) && (ts[1] <= this._info.timestampInMillis))
                    {
                        return 2;
                    }
                    return -1;
                }
                if (controllable._binder != null)
                {
                    int num = controllable._binder.CountValidate(ts, tsCount);
                    if ((tsCount > num) && (ts[num] <= this._info.timestampInMillis))
                    {
                        return (num + 1);
                    }
                }
            }
            return -1;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if ((this.owner != null) && (this.owner._binder == this))
                {
                    this.owner._binder = null;
                }
                if (--binderCount == 0)
                {
                    first = last = this.next = (Controllable.CL_Binder) (this.prev = null);
                }
                else
                {
                    if (first == this)
                    {
                        first = this.next;
                        this.next.prev = null;
                    }
                    else if (last == this)
                    {
                        last = this.prev;
                        this.prev.next = null;
                    }
                    else
                    {
                        this.next.prev = this.prev;
                        this.prev.next = this.next;
                    }
                    this.next = (Controllable.CL_Binder) (this.prev = null);
                }
            }
        }

        public bool Find()
        {
            return (this._root.Find() && (this.sameSearch || this._parent.Find()));
        }

        public void Link()
        {
            this.PreLink();
            if (this._root.controllable._pendingControlCount == this._root.controllable._refreshedControlCount)
            {
                this._root.controllable.ch.RefreshEngauge();
            }
            else
            {
                this._root.controllable.ch.RefreshEnter();
            }
        }

        private void PreLink()
        {
            Controllable controllable = !this.sameSearch ? this._parent.controllable : this._root.controllable;
            if ((controllable.F & Controllable.ControlFlags.Root) == 0)
            {
                controllable._binder.PreLink();
            }
            if ((this.owner.F & Controllable.ControlFlags.Initialized) == 0)
            {
                this.owner.OCO_FOUND(controllable.networkViewID, ref this._info);
            }
        }

        public static void StaticLink(Controllable root)
        {
            Controllable.CL_Binder last = Controllable.CL_Binder.last;
            for (int i = binderCount - 1; i >= 0; i--)
            {
                Controllable.CL_Binder binder2 = last;
                last = last.prev;
                if ((binder2.Find() && (binder2._root.controllable == root)) && (binder2.CountValidate(root._rootCountTimeStamps, root._rootCountTimeStamps.Count) == root._refreshedControlCount))
                {
                    binder2.Link();
                    return;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Search
        {
            private NetworkViewID _id;
            private NetworkView _view;
            private Controllable _controllable;
            public NetworkViewID id
            {
                get
                {
                    return this._id;
                }
                set
                {
                    this._id = value;
                    this._view = null;
                    this._controllable = null;
                }
            }
            public NetworkView view
            {
                get
                {
                    return this._view;
                }
            }
            public Controllable controllable
            {
                get
                {
                    return this._controllable;
                }
            }
            public bool Find()
            {
                if (this._controllable != null)
                {
                    return true;
                }
                if (this._view == null)
                {
                    this._view = NetworkView.Find(this._id);
                    if (this._view == null)
                    {
                        return false;
                    }
                }
                Character idMain = this._view.idMain as Character;
                if (idMain == null)
                {
                    return false;
                }
                return (bool) (this._controllable = idMain.controllable);
            }
        }
    }

    [Flags]
    private enum ControlFlags
    {
        Initialized = 0x10,
        Local = 2,
        Owned = 1,
        Player = 4,
        Root = 8,
        Strong = 0x20
    }

    public delegate void DestroyInContextQuery(Controllable controllable);

    [StructLayout(LayoutKind.Sequential)]
    private struct Link
    {
        public Controllable it;
        public bool vl;
    }

    private static class LocalOnly
    {
        public static readonly List<Controllable> rootLocalPlayerControllables = new List<Controllable>();
    }
}

