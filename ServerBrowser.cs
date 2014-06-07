using POSIX;
using Rust.Steam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class ServerBrowser : MonoBehaviour
{
    [CompilerGenerated]
    private static Comparison<Server> <>f__am$cache16;
    [CompilerGenerated]
    private static Comparison<Server> <>f__am$cache17;
    [CompilerGenerated]
    private static Comparison<Server> <>f__am$cache18;
    [CompilerGenerated]
    private static Comparison<Server> <>f__am$cache19;
    private funcServerAdd AddServerCallback;
    private GCHandle AddServerGC;
    public ServerCategory[] categoryButtons;
    public string currentServerChecksum;
    public dfRichTextLabel detailsLabel;
    private funcServerFinish FinServerCallback;
    private bool firstOpened;
    private bool needsServerListUpdate;
    private int orderType = 2;
    private int pageNumber;
    public Pagination pagination;
    private int playerCount;
    [NonSerialized]
    public Queue<GameObject> pooledServerItems = new Queue<GameObject>();
    public dfControl refreshButton;
    private GCHandle RefreshFinishedGC;
    public dfPanel serverContainer;
    private int serverCount;
    public GameObject serverItem;
    public const int ServerItemHeight = 0x22;
    private IntPtr serverRefresh;
    [NonSerialized]
    public List<Server>[] servers = new List<Server>[6];
    [NonSerialized]
    public int serverType;
    public const int SERVERTYPE_COMMUNITY = 1;
    public const int SERVERTYPE_FRIENDS = 5;
    public const int SERVERTYPE_HISTORY = 4;
    public const int SERVERTYPE_MODDED = 2;
    public const int SERVERTYPE_OFFICIAL = 0;
    public const int SERVERTYPE_WHITELIST = 3;
    private int slotCount;

    private void Add_Server(int iMaxPlayers, int iCurrentPlayers, int iPing, uint iLastPlayed, [In, MarshalAs(UnmanagedType.LPStr)] string strHostname, [In, MarshalAs(UnmanagedType.LPStr)] string strAddress, int iPort, int iQueryPort, [In, MarshalAs(UnmanagedType.LPStr)] string tags, bool bPassworded, int iType)
    {
        string strName = strAddress + ":" + iPort.ToString();
        Server item = new Server {
            name = strHostname,
            address = strAddress,
            maxplayers = iMaxPlayers,
            currentplayers = iCurrentPlayers,
            ping = iPing,
            lastplayed = iLastPlayed,
            port = iPort,
            queryport = iQueryPort,
            fave = FavouriteList.Contains(strName)
        };
        if (item.name.Length > 0x40)
        {
            item.name = item.name.Substring(0, 0x40);
        }
        this.playerCount += iCurrentPlayers;
        this.serverCount++;
        this.slotCount += iMaxPlayers;
        this.needsServerListUpdate = true;
        int num = (int) ((((float) this.playerCount) / ((float) this.slotCount)) * 100f);
        string[] textArray1 = new string[] { "Found ", this.playerCount.ToString(), " players on ", this.serverCount.ToString(), " servers. We are at ", num.ToString(), "% capacity." };
        this.detailsLabel.Text = string.Concat(textArray1);
        if (iType == 3)
        {
            this.servers[5].Add(item);
            this.categoryButtons[5].UpdateServerCount(this.servers[5].Count);
        }
        else if (iType == 4)
        {
            int num2 = (int) Time.ElapsedSecondsSince((int) item.lastplayed);
            string str2 = string.Empty;
            if (num2 < 60)
            {
                str2 = num2.ToString() + " seconds ago";
            }
            else if (num2 < 0xe10)
            {
                str2 = ((num2 / 60)).ToString() + " minutes ago";
            }
            else if (num2 < 0x2a300)
            {
                str2 = (((num2 / 60) / 60)).ToString() + " hours ago";
            }
            else
            {
                str2 = ((((num2 / 60) / 60) / 0x18)).ToString() + " days ago";
            }
            item.name = item.name + " (" + str2 + ")";
            this.servers[4].Add(item);
            this.categoryButtons[4].UpdateServerCount(this.servers[4].Count);
        }
        else if (tags.Contains("official"))
        {
            this.servers[0].Add(item);
            this.categoryButtons[0].UpdateServerCount(this.servers[0].Count);
        }
        else
        {
            char[] separator = new char[] { ',' };
            foreach (string str3 in tags.Split(separator))
            {
                ulong num4;
                if ((!str3.StartsWith("mp") && !str3.StartsWith("cp")) && (str3.StartsWith("sg:") && ulong.TryParse(str3.Substring(3), NumberStyles.HexNumber, null, out num4)))
                {
                    if (SteamGroups.MemberOf(num4))
                    {
                        this.servers[3].Add(item);
                        this.categoryButtons[3].UpdateServerCount(this.servers[3].Count);
                    }
                    return;
                }
            }
            if (tags.Contains("modded"))
            {
                this.servers[2].Add(item);
                this.categoryButtons[2].UpdateServerCount(this.servers[2].Count);
            }
            else if (!strHostname.Contains("oxide", true) && !strHostname.Contains("rust++", true))
            {
                this.servers[1].Add(item);
                this.categoryButtons[1].UpdateServerCount(this.servers[1].Count);
            }
        }
    }

    public void ChangeOrder(int iType)
    {
        this.orderType = iType;
        this.UpdateServerList();
    }

    public void ClearList()
    {
        this.pageNumber = 0;
        foreach (List<Server> list in this.servers)
        {
            list.Clear();
        }
        foreach (ServerCategory category in this.categoryButtons)
        {
            if (category != null)
            {
                category.UpdateServerCount(0);
            }
        }
        this.ClearServers();
        this.playerCount = 0;
        this.serverCount = 0;
        this.slotCount = 0;
        this.detailsLabel.Text = "...";
    }

    public void ClearServers()
    {
        foreach (ServerItem item in this.serverContainer.GetComponentsInChildren<ServerItem>())
        {
            item.gameObject.GetComponent<dfControl>().Hide();
            this.pooledServerItems.Enqueue(item.gameObject);
        }
    }

    private int GetMaxServers()
    {
        int height = (int) this.serverContainer.Height;
        return (height / 0x22);
    }

    private GameObject NewServerItem()
    {
        if (this.pooledServerItems.Count > 0)
        {
            return this.pooledServerItems.Dequeue();
        }
        GameObject obj2 = (GameObject) Object.Instantiate(this.serverItem);
        dfControl component = obj2.GetComponent<dfControl>();
        this.serverContainer.AddControl(component);
        return obj2;
    }

    private void OnEnable()
    {
        base.StartCoroutine(this.ServerListUpdater());
    }

    public void OnFirstOpen()
    {
        if (!this.firstOpened && base.GetComponent<dfPanel>().IsVisible)
        {
            this.firstOpened = true;
            this.RefreshServerList();
        }
    }

    public void OnPageSwitched(int iNewPage)
    {
        this.pageNumber = iNewPage;
        this.UpdateServerList();
    }

    public void OrderByName()
    {
        this.ChangeOrder(0);
    }

    public void OrderByPing()
    {
        this.ChangeOrder(2);
    }

    public void OrderByPlayers()
    {
        this.ChangeOrder(1);
    }

    private void RefreshFinished()
    {
        this.refreshButton.IsEnabled = true;
        this.refreshButton.Opacity = 1f;
    }

    public void RefreshServerList()
    {
        this.refreshButton.IsEnabled = false;
        this.refreshButton.Opacity = 0.2f;
        SteamClient.Needed();
        this.ClearList();
        this.detailsLabel.Text = "Updating..";
        this.serverRefresh = SteamServers_Fetch(0x42d, this.AddServerCallback, this.FinServerCallback);
        if (this.serverRefresh == IntPtr.Zero)
        {
            Debug.Log("Error! Couldn't refresh servers!!");
        }
    }

    [DebuggerHidden]
    private IEnumerator ServerListUpdater()
    {
        return new <ServerListUpdater>c__Iterator35 { <>f__this = this };
    }

    private void Start()
    {
        for (int i = 0; i < this.servers.Length; i++)
        {
            this.servers[i] = new List<Server>();
        }
        this.AddServerCallback = new funcServerAdd(this.Add_Server);
        this.AddServerGC = GCHandle.Alloc(this.AddServerCallback);
        this.FinServerCallback = new funcServerFinish(this.RefreshFinished);
        this.RefreshFinishedGC = GCHandle.Alloc(this.FinServerCallback);
        base.BroadcastMessage("CategoryChanged", this.serverType);
        this.pagination.OnPageSwitch += new Pagination.SwitchToPage(this.OnPageSwitched);
        for (int j = 0; j < 50; j++)
        {
            this.NewServerItem();
        }
        this.ClearServers();
    }

    [DllImport("librust")]
    public static extern void SteamServers_Destroy(IntPtr ptr);
    [DllImport("librust")]
    public static extern IntPtr SteamServers_Fetch(int serverVersion, funcServerAdd fnc, funcServerFinish fnsh);
    public void SwitchCategory(int catID)
    {
        if (this.serverType != catID)
        {
            this.pageNumber = 0;
            this.currentServerChecksum = string.Empty;
            this.serverType = catID;
            base.BroadcastMessage("CategoryChanged", this.serverType);
            this.ClearServers();
            this.UpdateServerList();
        }
    }

    public void UpdateServerList()
    {
        this.needsServerListUpdate = false;
        int maxServers = this.GetMaxServers();
        maxServers = Math.Min(this.servers[this.serverType].Count, maxServers);
        int index = this.pageNumber * maxServers;
        if (((this.servers[this.serverType].Count != 0) && (index >= 0)) && (index <= this.servers[this.serverType].Count))
        {
            int iPages = (int) Mathf.Ceil(((float) this.servers[this.serverType].Count) / ((float) maxServers));
            if (this.serverType == 4)
            {
                if (<>f__am$cache16 == null)
                {
                    <>f__am$cache16 = (x, y) => (x.lastplayed == y.lastplayed) ? ((Comparison<Server>) string.Compare(x.name, y.name)) : ((Comparison<Server>) y.lastplayed.CompareTo(x.lastplayed));
                }
                this.servers[this.serverType].Sort(<>f__am$cache16);
            }
            else
            {
                if (this.orderType == 0)
                {
                    if (<>f__am$cache17 == null)
                    {
                        <>f__am$cache17 = (x, y) => (x.fave == y.fave) ? ((Comparison<Server>) string.Compare(x.name, y.name)) : ((Comparison<Server>) y.fave.CompareTo(x.fave));
                    }
                    this.servers[this.serverType].Sort(<>f__am$cache17);
                }
                if (this.orderType == 1)
                {
                    if (<>f__am$cache18 == null)
                    {
                        <>f__am$cache18 = (x, y) => (x.fave == y.fave) ? ((Comparison<Server>) ((x.currentplayers == y.currentplayers) ? ((Comparison<Server>) string.Compare(x.name, y.name)) : ((Comparison<Server>) y.currentplayers.CompareTo(x.currentplayers)))) : ((Comparison<Server>) y.fave.CompareTo(x.fave));
                    }
                    this.servers[this.serverType].Sort(<>f__am$cache18);
                }
                if (this.orderType == 2)
                {
                    if (<>f__am$cache19 == null)
                    {
                        <>f__am$cache19 = (x, y) => (x.fave == y.fave) ? ((Comparison<Server>) ((x.ping == y.ping) ? ((Comparison<Server>) string.Compare(x.name, y.name)) : ((Comparison<Server>) x.ping.CompareTo(y.ping)))) : ((Comparison<Server>) y.fave.CompareTo(x.fave));
                    }
                    this.servers[this.serverType].Sort(<>f__am$cache19);
                }
            }
            if ((index + maxServers) > this.servers[this.serverType].Count)
            {
                maxServers = this.servers[this.serverType].Count - index;
            }
            List<Server> range = this.servers[this.serverType].GetRange(index, maxServers);
            this.pagination.Setup(iPages, this.pageNumber);
            string str = string.Empty;
            foreach (Server server in range)
            {
                str = str + server.address;
            }
            if (str != this.currentServerChecksum)
            {
                this.ClearServers();
                Vector3 vector = new Vector3(0f, 0f, 0f);
                this.currentServerChecksum = str;
                bool fave = false;
                foreach (Server server2 in range)
                {
                    Server s = server2;
                    if (fave && !server2.fave)
                    {
                        vector.y -= 2f;
                    }
                    fave = server2.fave;
                    GameObject obj2 = this.NewServerItem();
                    obj2.GetComponent<ServerItem>().Init(ref s);
                    dfControl component = obj2.GetComponent<dfControl>();
                    component.Width = this.serverContainer.Width;
                    component.Position = vector;
                    component.Show();
                    vector.y -= 34f;
                }
                this.serverContainer.Invalidate();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <ServerListUpdater>c__Iterator35 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ServerBrowser <>f__this;

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
                    break;

                case 1:
                    break;
                    this.$PC = -1;
                    goto Label_0064;

                default:
                    goto Label_0064;
            }
            if (this.<>f__this.needsServerListUpdate)
            {
                this.<>f__this.UpdateServerList();
            }
            this.$current = new WaitForSeconds(0.2f);
            this.$PC = 1;
            return true;
        Label_0064:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
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

    public delegate void funcServerAdd(int iMaxPlayers, int iCurrentPlayers, int iPing, uint iLastPlayed, [In, MarshalAs(UnmanagedType.LPStr)] string strHostname, [In, MarshalAs(UnmanagedType.LPStr)] string strAddress, int iPort, int iQueryPort, [In, MarshalAs(UnmanagedType.LPStr)] string tags, bool bPassworded, int iType);

    public delegate void funcServerFinish();

    public class Server
    {
        public string address;
        public int currentplayers;
        public bool fave;
        public uint lastplayed;
        public int maxplayers;
        public string name;
        public bool passworded;
        public int ping;
        public int port;
        public int queryport;
    }
}

