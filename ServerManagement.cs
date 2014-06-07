using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using uLink;
using UnityEngine;

[RequireComponent(typeof(uLinkNetworkView))]
public class ServerManagement : MonoBehaviour
{
    [NonSerialized]
    protected readonly List<PlayerClient> _playerClientList;
    private static ServerManagement _serverMan;
    protected bool blockFutureConnections;
    [SerializeField]
    protected string defaultPlayerControllableKey;
    private bool hasUnstickPosition;
    private static NetError? kickedNetError;
    [NonSerialized, Obsolete("Use PlayerClient.All")]
    internal readonly LockedList<PlayerClient> lockedPlayerClientList;
    private Vector3 nextUnstickPosition;
    private Transform unstickTransform;

    public ServerManagement() : this(new List<PlayerClient>())
    {
    }

    private ServerManagement(List<PlayerClient> pcList)
    {
        this.defaultPlayerControllableKey = ":player_soldier";
        this.lockedPlayerClientList = new LockedList<PlayerClient>(pcList);
        this._playerClientList = pcList;
    }

    private void AddPlayerClientToList(PlayerClient pc)
    {
        this._playerClientList.Add(pc);
    }

    public virtual void AddPlayerSpawn(GameObject spawn)
    {
    }

    protected void Awake()
    {
        _serverMan = this;
        Object.DontDestroyOnLoad(base.gameObject);
        DestroysOnDisconnect.ApplyToGameObject(base.gameObject);
    }

    [RPC]
    protected void ClientFirstReady(NetworkMessageInfo info)
    {
    }

    [Obsolete("You should be using PlayerClient.FindAllWithName"), DebuggerHidden]
    internal IEnumerable<PlayerClient> FindPlayerClientsByName(string name, StringComparison comparison)
    {
        return new <FindPlayerClientsByName>c__Iterator2D { name = name, comparison = comparison, <$>name = name, <$>comparison = comparison, <>f__this = this, $PC = -2 };
    }

    [DebuggerHidden, Obsolete("You should be using PlayerClient.FindAllWithString")]
    internal IEnumerable<PlayerClient> FindPlayerClientsByString(string name)
    {
        return new <FindPlayerClientsByString>c__Iterator2C { name = name, <$>name = name, <>f__this = this, $PC = -2 };
    }

    public static ServerManagement Get()
    {
        return _serverMan;
    }

    public static NetError GetLastKickReason(bool clear)
    {
        NetError? kickedNetError = ServerManagement.kickedNetError;
        NetError error = !kickedNetError.HasValue ? NetCull.lastError : kickedNetError.Value;
        if (clear)
        {
            ServerManagement.kickedNetError = null;
        }
        return error;
    }

    public static IEnumerable<NetworkPlayer> GetNetworkPlayersByName(string name)
    {
        return GetNetworkPlayersByName(name, StringComparison.InvariantCultureIgnoreCase);
    }

    [DebuggerHidden]
    public static IEnumerable<NetworkPlayer> GetNetworkPlayersByName(string name, StringComparison comparison)
    {
        return new <GetNetworkPlayersByName>c__Iterator2E { name = name, comparison = comparison, <$>name = name, <$>comparison = comparison, $PC = -2 };
    }

    [DebuggerHidden]
    public static IEnumerable<NetworkPlayer> GetNetworkPlayersByString(string partialNameOrIntID)
    {
        return new <GetNetworkPlayersByString>c__Iterator2F { partialNameOrIntID = partialNameOrIntID, <$>partialNameOrIntID = partialNameOrIntID, $PC = -2 };
    }

    public RPCMode GetNetworkPlayersInGroup(string group)
    {
        return RPCMode.Others;
    }

    public RPCMode GetNetworkPlayersInSameZone(PlayerClient client)
    {
        return RPCMode.Others;
    }

    protected static bool GetOrigin(NetworkPlayer player, bool eyes, out Vector3 origin)
    {
        PlayerClient client;
        ServerManagement management = Get();
        if ((management != null) && management.GetPlayerClient(player, out client))
        {
            Controllable controllable = client.controllable;
            if (controllable != null)
            {
                Transform transform;
                Character component = controllable.GetComponent<Character>();
                if (component != null)
                {
                    transform = (!eyes || (component.eyesTransformReadOnly == null)) ? component.transform : component.eyesTransformReadOnly;
                }
                else
                {
                    transform = controllable.transform;
                }
                origin = transform.position;
                return true;
            }
        }
        origin = new Vector3();
        return false;
    }

    public bool GetPlayerClient(NetworkPlayer player, out PlayerClient playerClient)
    {
        foreach (PlayerClient client in this._playerClientList)
        {
            if (client.netPlayer == player)
            {
                playerClient = client;
                return true;
            }
        }
        playerClient = null;
        return false;
    }

    public bool GetPlayerClient(GameObject go, out PlayerClient playerClient)
    {
        foreach (PlayerClient client in this._playerClientList)
        {
            if ((client.controllable != null) && (client.controllable.gameObject == go))
            {
                playerClient = client;
                return true;
            }
        }
        playerClient = null;
        return false;
    }

    [RPC]
    protected void KP(int err)
    {
        kickedNetError = new NetError?((NetError) err);
    }

    public void LocalClientPoliteReady()
    {
        base.networkView.RPC("ClientFirstReady", RPCMode.Server, new object[0]);
    }

    protected void OnDestroy()
    {
        if (_serverMan == this)
        {
            _serverMan = null;
        }
    }

    private void RemovePlayerClientFromList(PlayerClient pc)
    {
        this._playerClientList.Remove(pc);
    }

    private void RemovePlayerClientFromListByNetworkPlayer(NetworkPlayer np)
    {
        PlayerClient client;
        if (this.GetPlayerClient(np, out client))
        {
            this.RemovePlayerClientFromList(client);
        }
        else
        {
            Debug.Log("Error, could not find PC for NP");
        }
    }

    public virtual void RemovePlayerSpawn(GameObject spawn)
    {
    }

    [RPC]
    protected void RequestRespawn(bool campRequest, NetworkMessageInfo info)
    {
    }

    [RPC]
    protected void RS(float duration)
    {
        NetCull.ResynchronizeClock((double) duration);
    }

    public virtual void TeleportPlayer(NetworkPlayer move, Vector3 worldPoint)
    {
    }

    private void UnstickInvoke()
    {
        if (this.hasUnstickPosition)
        {
            try
            {
                if (this.unstickTransform != null)
                {
                    this.unstickTransform.position = this.nextUnstickPosition;
                    Character component = this.unstickTransform.GetComponent<Character>();
                    if (component != null)
                    {
                        CCMotor ccmotor = component.ccmotor;
                        if (ccmotor != null)
                        {
                            ccmotor.Teleport(this.nextUnstickPosition);
                        }
                    }
                }
            }
            finally
            {
                this.hasUnstickPosition = false;
            }
        }
    }

    [RPC]
    protected void UnstickMove(Vector3 point)
    {
        PlayerClient localPlayerClient = PlayerClient.localPlayerClient;
        if (localPlayerClient != null)
        {
            Controllable controllable = localPlayerClient.controllable;
            if (controllable != null)
            {
                Transform transform;
                Character component = controllable.GetComponent<Character>();
                if (component != null)
                {
                    transform = component.transform;
                }
                else
                {
                    transform = controllable.transform;
                }
                if (transform != null)
                {
                    this.hasUnstickPosition = true;
                    this.nextUnstickPosition = point;
                    this.unstickTransform = transform;
                    this.UnstickInvoke();
                    base.Invoke("UnstickInvoke", 0.25f);
                }
            }
        }
    }

    [CompilerGenerated]
    private sealed class <FindPlayerClientsByName>c__Iterator2D : IDisposable, IEnumerator, IEnumerable, IEnumerable<PlayerClient>, IEnumerator<PlayerClient>
    {
        internal PlayerClient $current;
        internal int $PC;
        internal StringComparison <$>comparison;
        internal string <$>name;
        internal List<PlayerClient>.Enumerator <$s_295>__0;
        internal ServerManagement <>f__this;
        internal PlayerClient <client>__1;
        internal StringComparison comparison;
        internal string name;

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
                        this.<$s_295>__0.Dispose();
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
                    this.<$s_295>__0 = this.<>f__this._playerClientList.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00CF;
            }
            try
            {
                while (this.<$s_295>__0.MoveNext())
                {
                    this.<client>__1 = this.<$s_295>__0.Current;
                    if (string.Equals(this.<client>__1.userName, this.name, this.comparison))
                    {
                        this.$current = this.<client>__1;
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
                this.<$s_295>__0.Dispose();
            }
            this.$PC = -1;
        Label_00CF:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<PlayerClient> IEnumerable<PlayerClient>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ServerManagement.<FindPlayerClientsByName>c__Iterator2D { <>f__this = this.<>f__this, name = this.<$>name, comparison = this.<$>comparison };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<PlayerClient>.GetEnumerator();
        }

        PlayerClient IEnumerator<PlayerClient>.Current
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
    private sealed class <FindPlayerClientsByString>c__Iterator2C : IDisposable, IEnumerator, IEnumerable, IEnumerable<PlayerClient>, IEnumerator<PlayerClient>
    {
        internal PlayerClient $current;
        internal int $PC;
        internal string <$>name;
        internal List<PlayerClient>.Enumerator <$s_292>__2;
        internal List<PlayerClient>.Enumerator <$s_293>__4;
        internal List<PlayerClient>.Enumerator <$s_294>__6;
        internal ServerManagement <>f__this;
        internal PlayerClient <client>__3;
        internal PlayerClient <client>__5;
        internal PlayerClient <client>__7;
        internal int <iFound>__0;
        internal ulong <iUserID>__1;
        internal string name;

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
                        this.<$s_292>__2.Dispose();
                    }
                    break;

                case 2:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_293>__4.Dispose();
                    }
                    break;

                case 3:
                    try
                    {
                    }
                    finally
                    {
                        this.<$s_294>__6.Dispose();
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
                    this.<iFound>__0 = 0;
                    this.<iUserID>__1 = 0L;
                    if (!ulong.TryParse(this.name, out this.<iUserID>__1))
                    {
                        goto Label_0109;
                    }
                    this.<$s_292>__2 = this.<>f__this._playerClientList.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_0122;

                case 3:
                    goto Label_01E1;

                default:
                    goto Label_026A;
            }
            try
            {
                switch (num)
                {
                    case 1:
                        this.<iFound>__0++;
                        goto Label_00F8;
                }
                while (this.<$s_292>__2.MoveNext())
                {
                    this.<client>__3 = this.<$s_292>__2.Current;
                    if (this.<client>__3.userID == this.<iUserID>__1)
                    {
                        this.$current = this.<client>__3;
                        this.$PC = 1;
                        flag = true;
                        goto Label_026C;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_292>__2.Dispose();
            }
        Label_00F8:
            if (this.<iFound>__0 > 0)
            {
                goto Label_026A;
            }
        Label_0109:
            this.<$s_293>__4 = this.<>f__this._playerClientList.GetEnumerator();
            num = 0xfffffffd;
        Label_0122:
            try
            {
                switch (num)
                {
                    case 2:
                        this.<iFound>__0++;
                        break;
                }
                while (this.<$s_293>__4.MoveNext())
                {
                    this.<client>__5 = this.<$s_293>__4.Current;
                    if (string.Equals(this.<client>__5.userName, this.name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.$current = this.<client>__5;
                        this.$PC = 2;
                        flag = true;
                        goto Label_026C;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_293>__4.Dispose();
            }
            if (this.<iFound>__0 > 0)
            {
                goto Label_026A;
            }
            this.<$s_294>__6 = this.<>f__this._playerClientList.GetEnumerator();
            num = 0xfffffffd;
        Label_01E1:
            try
            {
                while (this.<$s_294>__6.MoveNext())
                {
                    this.<client>__7 = this.<$s_294>__6.Current;
                    if (this.<client>__7.userName.StartsWith(this.name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.$current = this.<client>__7;
                        this.$PC = 3;
                        flag = true;
                        goto Label_026C;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<$s_294>__6.Dispose();
            }
            this.$PC = -1;
        Label_026A:
            return false;
        Label_026C:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<PlayerClient> IEnumerable<PlayerClient>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ServerManagement.<FindPlayerClientsByString>c__Iterator2C { <>f__this = this.<>f__this, name = this.<$>name };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<PlayerClient>.GetEnumerator();
        }

        PlayerClient IEnumerator<PlayerClient>.Current
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
    private sealed class <GetNetworkPlayersByName>c__Iterator2E : IDisposable, IEnumerator, IEnumerable, IEnumerable<NetworkPlayer>, IEnumerator<NetworkPlayer>
    {
        internal NetworkPlayer $current;
        internal int $PC;
        internal StringComparison <$>comparison;
        internal string <$>name;
        internal IEnumerator<PlayerClient> <$s_296>__1;
        internal PlayerClient <pc>__2;
        internal ServerManagement <svm>__0;
        internal StringComparison comparison;
        internal string name;

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
                        if (this.<$s_296>__1 == null)
                        {
                        }
                        this.<$s_296>__1.Dispose();
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
                    this.<svm>__0 = ServerManagement.Get();
                    if (this.<svm>__0 == null)
                    {
                        goto Label_00D2;
                    }
                    this.<$s_296>__1 = this.<svm>__0.FindPlayerClientsByName(this.name, this.comparison).GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00D9;
            }
            try
            {
                while (this.<$s_296>__1.MoveNext())
                {
                    this.<pc>__2 = this.<$s_296>__1.Current;
                    this.$current = this.<pc>__2.netPlayer;
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_296>__1 == null)
                {
                }
                this.<$s_296>__1.Dispose();
            }
        Label_00D2:
            this.$PC = -1;
        Label_00D9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<NetworkPlayer> IEnumerable<NetworkPlayer>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ServerManagement.<GetNetworkPlayersByName>c__Iterator2E { name = this.<$>name, comparison = this.<$>comparison };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<uLink.NetworkPlayer>.GetEnumerator();
        }

        NetworkPlayer IEnumerator<NetworkPlayer>.Current
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
    private sealed class <GetNetworkPlayersByString>c__Iterator2F : IDisposable, IEnumerator, IEnumerable, IEnumerable<NetworkPlayer>, IEnumerator<NetworkPlayer>
    {
        internal NetworkPlayer $current;
        internal int $PC;
        internal string <$>partialNameOrIntID;
        internal IEnumerator<PlayerClient> <$s_297>__1;
        internal PlayerClient <pc>__2;
        internal ServerManagement <svm>__0;
        internal string partialNameOrIntID;

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
                        if (this.<$s_297>__1 == null)
                        {
                        }
                        this.<$s_297>__1.Dispose();
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
                    this.<svm>__0 = ServerManagement.Get();
                    if (this.<svm>__0 == null)
                    {
                        goto Label_00CC;
                    }
                    this.<$s_297>__1 = this.<svm>__0.FindPlayerClientsByString(this.partialNameOrIntID).GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00D3;
            }
            try
            {
                while (this.<$s_297>__1.MoveNext())
                {
                    this.<pc>__2 = this.<$s_297>__1.Current;
                    this.$current = this.<pc>__2.netPlayer;
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_297>__1 == null)
                {
                }
                this.<$s_297>__1.Dispose();
            }
        Label_00CC:
            this.$PC = -1;
        Label_00D3:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<NetworkPlayer> IEnumerable<NetworkPlayer>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new ServerManagement.<GetNetworkPlayersByString>c__Iterator2F { partialNameOrIntID = this.<$>partialNameOrIntID };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<uLink.NetworkPlayer>.GetEnumerator();
        }

        NetworkPlayer IEnumerator<NetworkPlayer>.Current
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

