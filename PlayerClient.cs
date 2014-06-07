using Rust;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[RequireComponent(typeof(uLinkNetworkView))]
public class PlayerClient : IDMain
{
    private Controllable _controllable;
    private int _playerID;
    [NonSerialized]
    private bool boundUserID;
    [NonSerialized]
    public bool firstReady;
    private NetworkMessageInfo instantiationinfo;
    private const ulong kAutoReclockInitialDelay = 0x1f40L;
    private const ulong kAutoReclockInterval = 0x668a0L;
    private const ulong kAutoReclockMS_AddMax = 500L;
    private const ulong kAutoReclockMS_Base = 0xbb8L;
    private int lastInputFrame;
    public static PlayerClient localPlayerClient;
    public NetworkPlayer netPlayer;
    private ulong nextAutoReclockTime;
    public ulong userID;
    public string userName;

    public PlayerClient() : base(IDFlags.Unknown)
    {
        this.lastInputFrame = -2147483648;
    }

    private void Awake()
    {
        this._playerID = NetworkPlayer.unassigned.id;
    }

    protected virtual void ClientInput()
    {
    }

    public static bool Find(NetworkPlayer player, out PlayerClient pc)
    {
        int id = player.id;
        if ((id != NetworkPlayer.unassigned.id) && !(player == NetworkPlayer.server))
        {
            return g.playerIDDict.TryGetValue(id, out pc);
        }
        pc = null;
        return false;
    }

    public static bool Find(NetworkPlayer player, out PlayerClient pc, bool throwIfNotFound)
    {
        if (!throwIfNotFound)
        {
            return Find(player, out pc);
        }
        if (!Find(player, out pc))
        {
            throw new ArgumentException("There was no PlayerClient for that player", "player");
        }
        return true;
    }

    public static IEnumerable<PlayerClient> FindAllWithName(string name)
    {
        return FindAllWithName(name, StringComparison.InvariantCultureIgnoreCase);
    }

    public static IEnumerable<PlayerClient> FindAllWithName(string name, StringComparison comparison)
    {
        ServerManagement management;
        if (!string.IsNullOrEmpty(name) && ((management = ServerManagement.Get()) != null))
        {
            return management.FindPlayerClientsByName(name, comparison);
        }
        return EmptyArray<PlayerClient>.emptyEnumerable;
    }

    public static IEnumerable<PlayerClient> FindAllWithString(string partialNameOrIDInt)
    {
        ServerManagement management = ServerManagement.Get();
        if ((management != null) && !string.IsNullOrEmpty(partialNameOrIDInt))
        {
            return management.FindPlayerClientsByString(partialNameOrIDInt);
        }
        return EmptyArray<PlayerClient>.emptyEnumerable;
    }

    public static bool FindByUserID(ulong userID, out PlayerClient client)
    {
        if (userID == 0)
        {
            client = null;
            return false;
        }
        return g.userIDDict.TryGetValue(userID, out client);
    }

    public static PlayerClient GetLocalPlayer()
    {
        return localPlayerClient;
    }

    public static void InputFunction(GameObject req)
    {
        if ((((req != null) && (localPlayerClient != null)) && ((localPlayerClient._controllable != null) && (localPlayerClient._controllable.gameObject == req))) && (localPlayerClient.lastInputFrame != Time.frameCount))
        {
            localPlayerClient.lastInputFrame = Time.frameCount;
            localPlayerClient.ClientInput();
        }
    }

    protected void OnDestroy()
    {
        try
        {
            int id = NetworkPlayer.unassigned.id;
            if (this._playerID != id)
            {
                try
                {
                    PlayerClient objA = g.playerIDDict[this._playerID];
                    if (object.ReferenceEquals(objA, this))
                    {
                        g.playerIDDict.Remove(this._playerID);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
                finally
                {
                    this._playerID = id;
                }
            }
            if (this.boundUserID)
            {
                try
                {
                    PlayerClient client2 = g.userIDDict[this.userID];
                    if (object.ReferenceEquals(client2, this))
                    {
                        g.userIDDict.Remove(this.userID);
                    }
                }
                catch (Exception exception2)
                {
                    Debug.LogException(exception2, this);
                }
                finally
                {
                    this.boundUserID = false;
                }
            }
            if (localPlayerClient == this)
            {
                localPlayerClient = null;
            }
        }
        finally
        {
            base.OnDestroy();
        }
    }

    private void OnDisable()
    {
        if ((this.local && !base.destroying) && !NetInstance.IsCurrentlyDestroying(this))
        {
            Debug.LogWarning("The local player got disabled", this);
        }
    }

    private void OnEnable()
    {
        if (!this.local)
        {
            Debug.LogWarning("Something tried to enable a non local player.. setting enabled to false", this);
            base.enabled = false;
        }
    }

    internal void OnRootControllableEntered(Controllable controllable)
    {
        if (this._controllable != null)
        {
            Debug.LogWarning("There was a controllable for player client already", this);
        }
        this._controllable = controllable;
    }

    internal void OnRootControllableExited(Controllable controllable)
    {
        if (this._controllable != controllable)
        {
            Debug.LogWarning("The controllable exited did not match that of the existing value", this);
        }
        else
        {
            this._controllable = null;
        }
    }

    public void ProcessLocalPlayerPreRender()
    {
        if (this._controllable != null)
        {
            this._controllable.masterControllable.ProcessLocalPlayerPreRender();
        }
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this.netPlayer = info.networkView.owner;
        BitStream initialData = info.networkView.initialData;
        this.userID = initialData.ReadUInt64();
        this.userName = initialData.ReadString();
        string[] textArray1 = new string[] { "Player ", this.userName, " (", this.userID.ToString(), ")" };
        base.name = string.Concat(textArray1);
        this.instantiationinfo = info;
        this._playerID = this.netPlayer.id;
        g.playerIDDict[this._playerID] = this;
        g.userIDDict[this.userID] = this;
        this.boundUserID = true;
        if ((localPlayerClient == null) && base.networkView.isMine)
        {
            localPlayerClient = this;
            base.enabled = true;
            this.nextAutoReclockTime = NetCull.localTimeInMillis + ((ulong) 0x1f40L);
        }
        else
        {
            base.enabled = false;
        }
    }

    private void Update()
    {
        if ((this.lastInputFrame != Time.frameCount) && ((this._controllable == null) || !this._controllable.masterControllable.forwardsPlayerClientInput))
        {
            this.lastInputFrame = Time.frameCount;
            this.ClientInput();
        }
        if (NetCull.isClientRunning && !Globals.isLoading)
        {
            ulong localTimeInMillis = NetCull.localTimeInMillis;
            if (localTimeInMillis >= this.nextAutoReclockTime)
            {
                try
                {
                    ulong num2 = Math.Min(localTimeInMillis - this.nextAutoReclockTime, (ulong) 500L);
                    NetCull.ResynchronizeClock((double) (((double) (((ulong) 0xbb8L) + num2)) / 1000.0));
                    localTimeInMillis += num2;
                }
                finally
                {
                    this.nextAutoReclockTime = localTimeInMillis + ((ulong) 0x668a0L);
                }
            }
        }
    }

    public static LockedList<PlayerClient> All
    {
        get
        {
            ServerManagement management = ServerManagement.Get();
            if (management != null)
            {
                return management.lockedPlayerClientList;
            }
            return LockedList<PlayerClient>.Empty;
        }
    }

    public Controllable controllable
    {
        get
        {
            return this._controllable;
        }
    }

    public double instantiationTimeStamp
    {
        get
        {
            return this.instantiationinfo.timestamp;
        }
    }

    public bool local
    {
        get
        {
            return ((localPlayerClient != null) && (localPlayerClient == this));
        }
    }

    public Controllable rootControllable
    {
        get
        {
            return this._controllable;
        }
    }

    public Controllable topControllable
    {
        get
        {
            return this._controllable;
        }
    }

    private static class g
    {
        public static Dictionary<int, PlayerClient> playerIDDict = new Dictionary<int, PlayerClient>();
        public static Dictionary<ulong, PlayerClient> userIDDict = new Dictionary<ulong, PlayerClient>();
    }
}

