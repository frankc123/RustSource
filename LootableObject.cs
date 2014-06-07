using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[NGCAutoAddScript, RequireComponent(typeof(Inventory))]
public class LootableObject : IDLocal, IUseable, IContextRequestable, IContextRequestableQuick, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IUseable, MonoBehaviour, Useable>, IComponentInterface<IUseable, MonoBehaviour>, IComponentInterface<IUseable>, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    protected NetworkPlayer _currentlyUsingPlayer;
    [PrefetchComponent]
    public Inventory _inventory;
    [SerializeField]
    private LootSpawnListReference _lootSpawnListName;
    private Useable _useable;
    [NonSerialized]
    public bool accessLocked;
    public bool destroyOnEmpty;
    private const string kAnimation_Close = "close";
    private const string kAnimation_CloseIdle = "closed idle";
    private const string kAnimation_Open = "open";
    private const string kAnimation_OpenIdle = "opened idle";
    public bool lateSized;
    public float lifeTime;
    public float LootCycle;
    public RPOSLootWindow lootWindowOverride;
    public int NumberOfSlots = 12;
    protected string occupierText;
    private NetworkPlayer sentLooter;
    private bool sentSetLooter;
    private bool thisClientIsInWindow;

    public void CancelInvokes()
    {
        if (this.LootCycle > 0f)
        {
            base.CancelInvoke("TryAddLoot");
        }
        if (this.lifeTime > 0f)
        {
            base.CancelInvoke("DelayedDestroy");
        }
    }

    [RPC]
    protected void ClearLooter()
    {
        this.occupierText = null;
        this._currentlyUsingPlayer = NetworkPlayer.unassigned;
        if (this.thisClientIsInWindow)
        {
            try
            {
                RPOS.CloseLootWindow();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
            finally
            {
                this.thisClientIsInWindow = false;
            }
        }
    }

    public void ClientClosedLootWindow()
    {
        try
        {
            if (this.IsLocalLooting())
            {
                NetCull.RPC((MonoBehaviour) this, "StopLooting", RPCMode.Server);
            }
        }
        finally
        {
            if (this.thisClientIsInWindow)
            {
                this.thisClientIsInWindow = false;
            }
        }
    }

    protected virtual ContextResponse ContextRespond_OpenLoot(Controllable controllable, ulong timestamp)
    {
        return ContextRequestable.UseableForwardFromContextRespond(this, controllable, this._useable);
    }

    public virtual string ContextText(Controllable localControllable)
    {
        if (this._currentlyUsingPlayer == NetworkPlayer.unassigned)
        {
            return "Search";
        }
        if (this.occupierText == null)
        {
            PlayerClient client;
            if (!PlayerClient.Find(this._currentlyUsingPlayer, out client))
            {
                this.occupierText = "Occupied";
            }
            else
            {
                this.occupierText = string.Format("Occupied by {0}", client.userName);
            }
        }
        return this.occupierText;
    }

    public virtual bool ContextTextPoint(out Vector3 worldPoint)
    {
        if (ContextRequestable.PointUtil.SpriteOrOrigin(base.transform, out worldPoint))
        {
            worldPoint.y += 0.15f;
            return true;
        }
        return true;
    }

    public bool IsLocalLooting()
    {
        return (this.thisClientIsInWindow || ((this._currentlyUsingPlayer == NetCull.player) && (this._currentlyUsingPlayer != NetworkPlayer.unassigned)));
    }

    protected void OnDestroy()
    {
        UseableUtility.OnDestroy(this, this._useable);
    }

    public void OnUseEnter(Useable use)
    {
    }

    public void OnUseExit(Useable use, UseExitReason reason)
    {
    }

    public void RadialCheck()
    {
        if ((this._useable.user != null) && (Vector3.Distance(this._useable.user.transform.position, base.transform.position) > 5f))
        {
            this._useable.Eject();
            base.CancelInvoke("RadialCheck");
        }
    }

    [RPC]
    protected void SetLooter(NetworkPlayer ply)
    {
        this.occupierText = null;
        if (ply == NetworkPlayer.unassigned)
        {
            this.ClearLooter();
        }
        else
        {
            if (ply == NetCull.player)
            {
                if (!this.thisClientIsInWindow)
                {
                    try
                    {
                        this._currentlyUsingPlayer = ply;
                        RPOS.OpenLootWindow(this);
                        this.thisClientIsInWindow = true;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception, this);
                        NetCull.RPC((MonoBehaviour) this, "StopLooting", RPCMode.Server);
                        this.thisClientIsInWindow = false;
                        ply = NetworkPlayer.unassigned;
                    }
                }
            }
            else if ((this._currentlyUsingPlayer == NetCull.player) && (NetCull.player != NetworkPlayer.unassigned))
            {
                this.ClearLooter();
            }
            this._currentlyUsingPlayer = ply;
        }
    }

    [RPC]
    protected void StopLooting(NetworkMessageInfo info)
    {
        if (this._currentlyUsingPlayer == info.sender)
        {
            this._useable.Eject();
        }
    }

    [RPC]
    protected void TakeAll()
    {
    }

    public LootSpawnList _spawnList
    {
        get
        {
            return this._lootSpawnListName.list;
        }
        set
        {
            this._lootSpawnListName.list = value;
        }
    }
}

