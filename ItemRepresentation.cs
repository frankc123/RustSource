using Facepunch;
using Facepunch.Movement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[RequireComponent(typeof(uLinkNetworkView))]
public class ItemRepresentation : IDMain, IInterpTimedEventReceiver
{
    private Character _characterSignalee;
    private HeldItemDataBlock _datablock;
    private InventoryHolder _holder;
    internal ItemModPairArray _itemMods;
    private ViewModel _lastViewModel;
    private ItemModFlags _modFlags;
    private IDMain _parentMain;
    private NetworkView _parentView;
    private NetworkViewID _parentViewID;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$mapB;
    [NonSerialized]
    private ItemModRepresentation destroyingRep;
    public Socket.LocalSpace hand;
    private CharacterStateFlags? lastCharacterStateFlags;
    private bool modLock;
    public Socket.LocalSpace muzzle;
    private readonly CharacterStateSignal stateSignalReceive;
    [SerializeField]
    private GameObject[] visuals;
    [NonSerialized]
    private string worldAnimationGroupNameOverride;
    private bool worldStateDisabled;

    public ItemRepresentation() : base(IDFlags.Item)
    {
        this.stateSignalReceive = new CharacterStateSignal(this.StateSignalReceive);
    }

    public void Action(int number, IEnumerable<NetworkPlayer> targets)
    {
        base.networkView.RPC(ActionRPC(number), targets, new object[0]);
    }

    public void Action(int number, NetworkPlayer target)
    {
        base.networkView.RPC(ActionRPC(number), target, new object[0]);
    }

    public void Action(int number, RPCMode mode)
    {
        base.networkView.RPC(ActionRPC(number), mode, new object[0]);
    }

    public void Action(int number, NetworkPlayer target, params object[] arguments)
    {
        base.networkView.RPC(ActionRPC(number), target, arguments);
    }

    public void Action(int number, IEnumerable<NetworkPlayer> targets, params object[] arguments)
    {
        base.networkView.RPC(ActionRPC(number), targets, arguments);
    }

    public void Action<T>(int number, IEnumerable<NetworkPlayer> targets, T argument)
    {
        base.networkView.RPC<T>(ActionRPC(number), targets, argument);
    }

    public void Action<T>(int number, NetworkPlayer target, T argument)
    {
        base.networkView.RPC<T>(ActionRPC(number), target, argument);
    }

    public void Action(int number, RPCMode mode, params object[] arguments)
    {
        base.networkView.RPC(ActionRPC(number), mode, arguments);
    }

    public void Action<T>(int number, RPCMode mode, T argument)
    {
        base.networkView.RPC<T>(ActionRPC(number), mode, argument);
    }

    [RPC]
    protected void Action1(BitStream stream, NetworkMessageInfo info)
    {
        object[] args = new object[] { stream };
        InterpTimedEvent.Queue(this, "Action1", ref info, args);
    }

    [RPC]
    protected void Action1B(byte[] data, NetworkMessageInfo info)
    {
        this.Action1(new BitStream(data, false), info);
    }

    [RPC]
    protected void Action2(BitStream stream, NetworkMessageInfo info)
    {
        object[] args = new object[] { stream };
        InterpTimedEvent.Queue(this, "Action2", ref info, args);
    }

    [RPC]
    protected void Action2B(byte[] data, NetworkMessageInfo info)
    {
        this.Action2(new BitStream(data, false), info);
    }

    [RPC]
    protected void Action3(BitStream stream, NetworkMessageInfo info)
    {
        object[] args = new object[] { stream };
        InterpTimedEvent.Queue(this, "Action3", ref info, args);
    }

    [RPC]
    protected void Action3B(byte[] data, NetworkMessageInfo info)
    {
        this.Action3(new BitStream(data, false), info);
    }

    private static string ActionRPC(int number)
    {
        switch (number)
        {
            case 1:
                return "Action1";

            case 2:
                return "Action2";

            case 3:
                return "Action3";
        }
        throw new ArgumentOutOfRangeException("number", number, "number must be at or between 1 and 3");
    }

    private static string ActionRPCBitstream(int number)
    {
        switch (number)
        {
            case 1:
                return "Action1B";

            case 2:
                return "Action2B";

            case 3:
                return "Action3B";
        }
        throw new ArgumentOutOfRangeException("number", number, "number must be at or between 1 and 3");
    }

    public void ActionStream(int number, IEnumerable<NetworkPlayer> targets, BitStream stream)
    {
        base.networkView.RPC<byte[]>(ActionRPCBitstream(number), targets, stream.GetDataByteArray());
    }

    public void ActionStream(int number, NetworkPlayer target, BitStream stream)
    {
        base.networkView.RPC<byte[]>(ActionRPCBitstream(number), target, stream.GetDataByteArray());
    }

    public void ActionStream(int number, RPCMode mode, BitStream stream)
    {
        base.networkView.RPC<byte[]>(ActionRPCBitstream(number), mode, stream.GetDataByteArray());
    }

    protected void Awake()
    {
    }

    private void BindModAsLocal(ref ItemModPair pair, ref ModViewModelAddArgs a)
    {
        if (((int) pair.bindState) == 2)
        {
            this.UnBindModAsProxy(ref pair);
        }
        if ((((int) pair.bindState) == 1) || (((int) pair.bindState) == 3))
        {
            a.modRep = pair.representation;
            pair.dataBlock.BindAsLocal(ref a);
            pair.bindState = BindState.Local;
        }
    }

    private void BindModAsProxy(ref ItemModPair pair)
    {
        if (((int) pair.bindState) == 1)
        {
            pair.dataBlock.BindAsProxy(pair.representation);
            pair.bindState = BindState.World;
        }
    }

    internal void BindViewModel(ViewModel vm, IHeldItem item)
    {
        this.RunViewModelAdd(vm, item, false);
        this._lastViewModel = vm;
    }

    protected bool CheckParent()
    {
        if (this._parentView != null)
        {
            return true;
        }
        if (this._parentViewID != NetworkViewID.unassigned)
        {
            this._parentView = NetworkView.Find(this._parentViewID);
            if (this._parentView != null)
            {
                this._parentMain = null;
                Socket.LocalSpace itemAttachment = this._parentView.GetComponent<PlayerAnimation>().itemAttachment;
                if (itemAttachment != null)
                {
                    Vector3 offset;
                    Quaternion quaternion;
                    if ((this.hand.parent != null) && (this.hand.parent != base.transform))
                    {
                        offset = base.transform.InverseTransformPoint(this.hand.position);
                        Quaternion rotation = this.hand.rotation;
                        Vector3 direction = (Vector3) (rotation * Vector3.forward);
                        Vector3 vector3 = (Vector3) (rotation * Vector3.up);
                        direction = base.transform.InverseTransformDirection(direction);
                        vector3 = base.transform.InverseTransformDirection(vector3);
                        quaternion = Quaternion.LookRotation(direction, vector3);
                    }
                    else
                    {
                        offset = this.hand.offset;
                        quaternion = Quaternion.Euler(this.hand.eulerRotate);
                    }
                    itemAttachment.AddChildWithCoords(base.transform, offset, quaternion);
                }
                if (base.networkView.isMine)
                {
                    this.worldModels = actor.forceThirdPerson;
                }
                this.FindSignalee();
                return true;
            }
        }
        this.ClearSignals();
        return false;
    }

    private void ClearModPair(ref ItemModPair pair)
    {
        this.KillModRep(ref pair.representation, false);
        this.EraseModDatablock(ref pair.dataBlock);
        pair = new ItemModPair();
    }

    private bool ClearMods()
    {
        bool modLock = this.modLock;
        if (this.modLock)
        {
            return false;
        }
        this._modFlags = ItemModFlags.Other;
        try
        {
            this.modLock = true;
            for (int i = 0; i < 5; i++)
            {
                this._itemMods.ClearModPair(i, this);
            }
        }
        finally
        {
            this.modLock = modLock;
        }
        return true;
    }

    private void ClearSignals()
    {
        if (this._characterSignalee != null)
        {
            this._characterSignalee.signal_state -= this.stateSignalReceive;
        }
        if (this._holder != null)
        {
            this._holder.ClearItemRepresentation(this);
            this._holder = null;
        }
        this._characterSignalee = null;
    }

    private void EraseModDatablock(ref ItemModDataBlock block)
    {
        block = null;
    }

    private void FindSignalee()
    {
        this._parentMain = this._parentView.idMain;
        if (this._parentMain is Character)
        {
            Character signalee = (Character) this._parentMain;
            this.SetSignalee(signalee);
            this._holder = signalee.GetLocal<InventoryHolder>();
            if (this._holder != null)
            {
                this._holder.SetItemRepresentation(this);
            }
        }
        else
        {
            this._holder = null;
            this.ClearSignals();
        }
    }

    protected CharacterStateFlags GetCharacterStateFlags()
    {
        if (this.CheckParent() && (this._parentMain is Character))
        {
            CharacterStateFlags stateFlags = ((Character) this._parentMain).stateFlags;
            this.lastCharacterStateFlags = new CharacterStateFlags?(stateFlags);
            return stateFlags;
        }
        CharacterStateFlags? lastCharacterStateFlags = this.lastCharacterStateFlags;
        return (!lastCharacterStateFlags.HasValue ? new CharacterStateFlags() : lastCharacterStateFlags.Value);
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        this.OnInterpTimedEvent();
    }

    private void InstallMod(ref ItemModPair to, int slot, ItemModDataBlock datablock, CharacterStateFlags flags)
    {
        to.dataBlock = datablock;
        if (to.representation != null)
        {
            this.KillModRep(ref to.representation, false);
        }
        if (to.dataBlock.hasModRepresentation && to.dataBlock.AddModRepresentationComponent(base.gameObject, out to.representation))
        {
            to.bindState = BindState.None;
            to.representation.Initialize(this, slot, flags);
            if (to.representation != null)
            {
                if (this.worldModels)
                {
                    this._itemMods.BindAsProxy(slot, this);
                }
            }
            else
            {
                to.bindState = BindState.Vacant;
                to.representation = null;
            }
        }
    }

    [RPC]
    protected void InterpDestroy(NetworkMessageInfo info)
    {
        if ((base.networkView != null) && base.networkView.isMine)
        {
            InterpTimedEvent.Remove(this, true);
        }
        else
        {
            InterpTimedEvent.Queue(this, "InterpDestroy", ref info);
            NetCull.DontDestroyWithNetwork((MonoBehaviour) this);
        }
    }

    internal void ItemModRepresentationDestroyed(ItemModRepresentation rep)
    {
        if (!this.modLock && (this.destroyingRep != rep))
        {
            this._itemMods.KillModForRep(rep, this, true);
        }
    }

    private void KillModRep(ref ItemModRepresentation rep, bool fromCallback)
    {
        if (!fromCallback && (rep != null))
        {
            ItemModRepresentation destroyingRep = this.destroyingRep;
            try
            {
                this.destroyingRep = rep;
                Object.Destroy(rep);
            }
            finally
            {
                this.destroyingRep = destroyingRep;
            }
        }
        rep = null;
    }

    [RPC]
    protected void Mods(byte[] data)
    {
        this.ClearMods();
        BitStream stream = new BitStream(data, false);
        byte num = stream.ReadByte();
        if (num > 0)
        {
            CharacterStateFlags characterStateFlags = this.GetCharacterStateFlags();
            for (int i = 0; i < num; i++)
            {
                ItemModDataBlock byUniqueID = (ItemModDataBlock) DatablockDictionary.GetByUniqueID(stream.ReadInt32());
                this._itemMods.InstallMod(i, this, byUniqueID, characterStateFlags);
                this._modFlags |= byUniqueID.modFlag;
            }
        }
    }

    protected void OnDestroy()
    {
        try
        {
            InterpTimedEvent.Remove(this, true);
            this.ClearMods();
        }
        finally
        {
            this._parentViewID = NetworkViewID.unassigned;
            this.ClearSignals();
            base.OnDestroy();
        }
    }

    protected void OnDrawGizmosSelected()
    {
        this.muzzle.DrawGizmos("muzzle");
    }

    protected virtual void OnInterpTimedEvent()
    {
        BitStream stream;
        NetworkMessageInfo info;
        int number = -1;
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num2;
            if (<>f__switch$mapB == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                dictionary.Add("Action1", 0);
                dictionary.Add("Action2", 1);
                dictionary.Add("Action3", 2);
                dictionary.Add("InterpDestroy", 3);
                <>f__switch$mapB = dictionary;
            }
            if (<>f__switch$mapB.TryGetValue(tag, out num2))
            {
                switch (num2)
                {
                    case 0:
                        number = 1;
                        stream = InterpTimedEvent.Argument<BitStream>(0);
                        goto Label_00C5;

                    case 1:
                        number = 2;
                        stream = InterpTimedEvent.Argument<BitStream>(0);
                        goto Label_00C5;

                    case 2:
                        number = 3;
                        stream = InterpTimedEvent.Argument<BitStream>(0);
                        goto Label_00C5;

                    case 3:
                        Object.Destroy(base.gameObject);
                        return;
                }
            }
        }
        InterpTimedEvent.MarkUnhandled();
        return;
    Label_00C5:
        info = InterpTimedEvent.Info;
        this.RunAction(number, stream, ref info);
    }

    public bool OverrideAnimationGroupName(string newGroupName)
    {
        if (string.IsNullOrEmpty(newGroupName))
        {
            newGroupName = null;
        }
        if (!(this.worldAnimationGroupNameOverride != newGroupName))
        {
            return false;
        }
        if (this._holder != null)
        {
            this._holder.ClearItemRepresentation(this);
            this.worldAnimationGroupNameOverride = newGroupName;
            this._holder.SetItemRepresentation(this);
        }
        else
        {
            this.worldAnimationGroupNameOverride = newGroupName;
        }
        return true;
    }

    public bool PlayWorldAnimation(GroupEvent GroupEvent)
    {
        if (this._characterSignalee != null)
        {
            PlayerAnimation component = this._characterSignalee.GetComponent<PlayerAnimation>();
            if (component != null)
            {
                return component.PlayAnimation(GroupEvent);
            }
        }
        return false;
    }

    public bool PlayWorldAnimation(GroupEvent GroupEvent, float speed)
    {
        if (this._characterSignalee != null)
        {
            PlayerAnimation component = this._characterSignalee.GetComponent<PlayerAnimation>();
            if (component != null)
            {
                return component.PlayAnimation(GroupEvent, speed);
            }
        }
        return false;
    }

    public bool PlayWorldAnimation(GroupEvent GroupEvent, float speed, float animationTime)
    {
        if (this._characterSignalee != null)
        {
            PlayerAnimation component = this._characterSignalee.GetComponent<PlayerAnimation>();
            if (component != null)
            {
                return component.PlayAnimation(GroupEvent, speed, animationTime);
            }
        }
        return false;
    }

    internal void PrepareViewModel(ViewModel vm, IHeldItem item)
    {
        this.RunViewModelAdd(vm, item, true);
        this._lastViewModel = vm;
    }

    private void RunAction(int number, BitStream stream, ref NetworkMessageInfo info)
    {
        switch (number)
        {
            case 1:
                this.datablock.DoAction1(stream, this, ref info);
                break;

            case 2:
                this.datablock.DoAction2(stream, this, ref info);
                break;

            case 3:
                this.datablock.DoAction3(stream, this, ref info);
                break;
        }
    }

    private void RunViewModelAdd(ViewModel vm, IHeldItem item, bool doMeshes)
    {
        ModViewModelAddArgs args = new ModViewModelAddArgs(vm, item, doMeshes);
        for (int i = 0; i < 5; i++)
        {
            this._itemMods.BindAsLocal(i, ref args, this);
        }
    }

    [Obsolete("This is dumb. The datablock shouldnt change")]
    internal void SetDataBlockFromHeldItem<T>(HeldItem<T> item) where T: HeldItemDataBlock
    {
        this._datablock = item.datablock;
    }

    public virtual void SetParent(GameObject parentGameObject)
    {
        Transform parent = parentGameObject.transform;
        if (!base.transform.IsChildOf(parent))
        {
            base.transform.parent = parent;
        }
    }

    private void SetSignalee(Character signalee)
    {
        if (signalee == null)
        {
            this.ClearSignals();
        }
        else if ((this._characterSignalee == null) || (this._characterSignalee != signalee))
        {
            signalee.signal_state += this.stateSignalReceive;
            this._characterSignalee = signalee;
        }
    }

    protected virtual void StateSignalReceive(Character character, bool treatedAsFirst)
    {
        CharacterStateFlags stateFlags = character.stateFlags;
        if (!this.lastCharacterStateFlags.HasValue || !this.lastCharacterStateFlags.Value.Equals(stateFlags))
        {
            this.lastCharacterStateFlags = new CharacterStateFlags?(stateFlags);
            for (int i = 0; i < 5; i++)
            {
                ItemModPair pair = this._itemMods[i];
                if (pair.representation != null)
                {
                    ItemModPair pair2 = this._itemMods[i];
                    pair2.representation.HandleChangedStateFlags(stateFlags, !treatedAsFirst);
                }
            }
        }
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this._parentViewID = info.networkView.initialData.ReadNetworkViewID();
        int uniqueID = info.networkView.initialData.ReadInt32();
        this._datablock = (HeldItemDataBlock) DatablockDictionary.GetByUniqueID(uniqueID);
        if (!this.CheckParent())
        {
            Debug.Log("No parent for item rep (yet)", this);
        }
    }

    private void UnBindModAsLocal(ref ItemModPair pair, ref ModViewModelRemoveArgs a)
    {
        if (((int) pair.bindState) == 3)
        {
            a.modRep = pair.representation;
            pair.dataBlock.UnBindAsLocal(ref a);
            pair.bindState = BindState.None;
        }
    }

    private void UnBindModAsProxy(ref ItemModPair pair)
    {
        if (((int) pair.bindState) == 2)
        {
            pair.dataBlock.UnBindAsProxy(pair.representation);
            pair.bindState = BindState.None;
        }
    }

    internal void UnBindViewModel(ViewModel vm, IHeldItem item)
    {
        ModViewModelRemoveArgs args = new ModViewModelRemoveArgs(vm, item);
        for (int i = 0; i < 5; i++)
        {
            this._itemMods.UnBindAsLocal(i, ref args, this);
        }
        if (this._lastViewModel == vm)
        {
            this._lastViewModel = null;
        }
    }

    public HeldItemDataBlock datablock
    {
        get
        {
            return this._datablock;
        }
    }

    public ItemModFlags modFlags
    {
        get
        {
            return this._modFlags;
        }
    }

    public string worldAnimationGroupName
    {
        get
        {
            if (this.worldAnimationGroupNameOverride == null)
            {
            }
            return this.datablock.animationGroupName;
        }
    }

    public bool worldModels
    {
        get
        {
            return !this.worldStateDisabled;
        }
        set
        {
            if (this.worldStateDisabled == value)
            {
                this.worldStateDisabled = !this.worldStateDisabled;
                if (this.visuals != null)
                {
                    for (int i = 0; i < this.visuals.Length; i++)
                    {
                        if (this.visuals[i] != null)
                        {
                            this.visuals[i].SetActive(value);
                        }
                    }
                }
                if (base.renderer != null)
                {
                    base.renderer.enabled = value;
                }
                if (value)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        this._itemMods.BindAsProxy(j, this);
                    }
                }
                else
                {
                    for (int k = 0; k < 5; k++)
                    {
                        this._itemMods.UnBindAsProxy(k, this);
                    }
                }
            }
        }
    }

    internal enum BindState : sbyte
    {
        Local = 3,
        None = 1,
        Vacant = 0,
        World = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ItemModPair
    {
        public ItemModDataBlock dataBlock;
        public ItemModRepresentation representation;
        public ItemRepresentation.BindState bindState;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ItemModPairArray
    {
        private const int internalPairCount = 5;
        private ItemRepresentation.ItemModPair a;
        private ItemRepresentation.ItemModPair b;
        private ItemRepresentation.ItemModPair c;
        private ItemRepresentation.ItemModPair d;
        private ItemRepresentation.ItemModPair e;
        static ItemModPairArray()
        {
        }

        public ItemRepresentation.ItemModPair this[int slotNumber]
        {
            get
            {
                switch (slotNumber)
                {
                    case 0:
                        return this.a;

                    case 1:
                        return this.b;

                    case 2:
                        return this.c;

                    case 3:
                        return this.d;

                    case 4:
                        return this.e;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (slotNumber)
                {
                    case 0:
                        this.a = value;
                        break;

                    case 1:
                        this.b = value;
                        break;

                    case 2:
                        this.c = value;
                        break;

                    case 3:
                        this.d = value;
                        break;

                    case 4:
                        this.e = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public ItemModDataBlock ItemModDataBlock(int slotNumber)
        {
            switch (slotNumber)
            {
                case 0:
                    return this.a.dataBlock;

                case 1:
                    return this.b.dataBlock;

                case 2:
                    return this.c.dataBlock;

                case 3:
                    return this.d.dataBlock;

                case 4:
                    return this.e.dataBlock;
            }
            throw new IndexOutOfRangeException();
        }

        public void BindAsLocal(int slotNumber, ref ModViewModelAddArgs args, ItemRepresentation itemRep)
        {
            switch (slotNumber)
            {
                case 0:
                    itemRep.BindModAsLocal(ref this.a, ref args);
                    break;

                case 1:
                    itemRep.BindModAsLocal(ref this.b, ref args);
                    break;

                case 2:
                    itemRep.BindModAsLocal(ref this.c, ref args);
                    break;

                case 3:
                    itemRep.BindModAsLocal(ref this.d, ref args);
                    break;

                case 4:
                    itemRep.BindModAsLocal(ref this.e, ref args);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void UnBindAsLocal(int slotNumber, ref ModViewModelRemoveArgs args, ItemRepresentation itemRep)
        {
            switch (slotNumber)
            {
                case 0:
                    itemRep.UnBindModAsLocal(ref this.a, ref args);
                    break;

                case 1:
                    itemRep.UnBindModAsLocal(ref this.b, ref args);
                    break;

                case 2:
                    itemRep.UnBindModAsLocal(ref this.c, ref args);
                    break;

                case 3:
                    itemRep.UnBindModAsLocal(ref this.d, ref args);
                    break;

                case 4:
                    itemRep.UnBindModAsLocal(ref this.e, ref args);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void BindAsProxy(int slotNumber, ItemRepresentation itemRep)
        {
            switch (slotNumber)
            {
                case 0:
                    itemRep.BindModAsProxy(ref this.a);
                    break;

                case 1:
                    itemRep.BindModAsProxy(ref this.b);
                    break;

                case 2:
                    itemRep.BindModAsProxy(ref this.c);
                    break;

                case 3:
                    itemRep.BindModAsProxy(ref this.d);
                    break;

                case 4:
                    itemRep.BindModAsProxy(ref this.e);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void UnBindAsProxy(int slotNumber, ItemRepresentation itemRep)
        {
            switch (slotNumber)
            {
                case 0:
                    itemRep.UnBindModAsProxy(ref this.a);
                    break;

                case 1:
                    itemRep.UnBindModAsProxy(ref this.b);
                    break;

                case 2:
                    itemRep.UnBindModAsProxy(ref this.c);
                    break;

                case 3:
                    itemRep.UnBindModAsProxy(ref this.d);
                    break;

                case 4:
                    itemRep.UnBindModAsProxy(ref this.e);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void ClearModPair(int slotNumber, ItemRepresentation owner)
        {
            switch (slotNumber)
            {
                case 0:
                    owner.ClearModPair(ref this.a);
                    break;

                case 1:
                    owner.ClearModPair(ref this.b);
                    break;

                case 2:
                    owner.ClearModPair(ref this.c);
                    break;

                case 3:
                    owner.ClearModPair(ref this.d);
                    break;

                case 4:
                    owner.ClearModPair(ref this.e);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        private static bool KillModForRep(ref ItemRepresentation.ItemModPair pair, ItemModRepresentation modRep, ItemRepresentation owner, bool fromCallback)
        {
            if (pair.representation == modRep)
            {
                owner.KillModRep(ref pair.representation, fromCallback);
                return true;
            }
            return true;
        }

        public bool KillModForRep(ItemModRepresentation modRep, ItemRepresentation owner, bool fromCallback)
        {
            switch (modRep.modSlot)
            {
                case 0:
                    return KillModForRep(ref this.a, modRep, owner, fromCallback);

                case 1:
                    return KillModForRep(ref this.b, modRep, owner, fromCallback);

                case 2:
                    return KillModForRep(ref this.c, modRep, owner, fromCallback);

                case 3:
                    return KillModForRep(ref this.d, modRep, owner, fromCallback);

                case 4:
                    return KillModForRep(ref this.e, modRep, owner, fromCallback);
            }
            throw new IndexOutOfRangeException();
        }

        public void InstallMod(int slotNumber, ItemRepresentation owner, ItemModDataBlock datablock, CharacterStateFlags flags)
        {
            switch (slotNumber)
            {
                case 0:
                    owner.InstallMod(ref this.a, 0, datablock, flags);
                    break;

                case 1:
                    owner.InstallMod(ref this.b, 1, datablock, flags);
                    break;

                case 2:
                    owner.InstallMod(ref this.c, 2, datablock, flags);
                    break;

                case 3:
                    owner.InstallMod(ref this.d, 3, datablock, flags);
                    break;

                case 4:
                    owner.InstallMod(ref this.e, 4, datablock, flags);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}

