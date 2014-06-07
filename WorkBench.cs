using System;
using System.Collections;
using uLink;
using UnityEngine;

[RequireComponent(typeof(Inventory)), NGCAutoAddScript]
public class WorkBench : IDMain, IUseable, IContextRequestable, IContextRequestableQuick, IContextRequestableText, IComponentInterface<IUseable, MonoBehaviour, Useable>, IComponentInterface<IUseable, MonoBehaviour>, IComponentInterface<IUseable>, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    private NetworkPlayer _currentlyUsingPlayer;
    private static bool _debug_workbench;
    [HideInInspector]
    public Inventory _inventory;
    private double _startTime_network;
    private Useable _useable;
    private float _workDuration;

    public WorkBench() : base(IDFlags.Unknown)
    {
        this._workDuration = -1f;
    }

    protected void Awake()
    {
        this.SharedAwake();
    }

    public void CancelWork()
    {
        IToolItem tool = this.GetTool();
        if (tool != null)
        {
            tool.CancelWork();
        }
        this._inventory.locked = false;
        this._startTime_network = 0.0;
        this._workDuration = -1f;
        base.CancelInvoke("CompleteWork");
        this.SendWorkStatusUpdate();
    }

    public void ClientClosedWorkbenchWindow()
    {
        if (this.IsLocalUsing())
        {
            NetCull.RPC((MonoBehaviour) this, "StopUsing", RPCMode.Server);
        }
    }

    public void CompleteWork()
    {
    }

    public ContextExecution ContextQuery(Controllable controllable, ulong timestamp)
    {
        return (ContextExecution.NotAvailable | ContextExecution.Quick);
    }

    public ContextResponse ContextRespondQuick(Controllable controllable, ulong timestamp)
    {
        return ContextRequestable.UseableForwardFromContextRespond(this, controllable, this._useable);
    }

    public string ContextText(Controllable localControllable)
    {
        if (this._currentlyUsingPlayer == NetworkPlayer.unassigned)
        {
            return "Use";
        }
        if (this._currentlyUsingPlayer != NetCull.player)
        {
            return "Occupied";
        }
        return string.Empty;
    }

    [RPC]
    private void DoAction()
    {
        if (this.IsWorking())
        {
            this.TryCancel();
        }
        else
        {
            this.StartWork();
        }
    }

    public bool EnsureWorkExists()
    {
        IToolItem tool = this.GetTool();
        return ((tool != null) && tool.canWork);
    }

    public float GetFractionComplete()
    {
        if (!this.IsWorking())
        {
            return 0f;
        }
        return (float) (this.GetTimePassed() / ((double) this._workDuration));
    }

    public virtual BlueprintDataBlock GetMatchingDBForItems()
    {
        ArrayList list = new ArrayList();
        foreach (ItemDataBlock block in DatablockDictionary.All)
        {
            if (!(block is BlueprintDataBlock))
            {
                continue;
            }
            BlueprintDataBlock block2 = block as BlueprintDataBlock;
            bool flag = true;
            foreach (BlueprintDataBlock.IngredientEntry entry in block2.ingredients)
            {
                int totalNum = 0;
                if ((this._inventory.FindItem(entry.Ingredient, out totalNum) == null) || (totalNum < entry.amount))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                list.Add(block2);
            }
        }
        if (list.Count <= 0)
        {
            return null;
        }
        BlueprintDataBlock block3 = null;
        int length = -1;
        IEnumerator enumerator = list.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                BlueprintDataBlock current = (BlueprintDataBlock) enumerator.Current;
                if (current.ingredients.Length > length)
                {
                    block3 = current;
                    length = current.ingredients.Length;
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        return block3;
    }

    public double GetTimePassed()
    {
        if (this._workDuration == -1f)
        {
            return -1.0;
        }
        return (NetCull.time - this._startTime_network);
    }

    public virtual IToolItem GetTool()
    {
        return this._inventory.FindItemType<IToolItem>();
    }

    public float GetWorkDuration()
    {
        IToolItem tool = this.GetTool();
        if (tool != null)
        {
            return tool.workDuration;
        }
        return 0f;
    }

    public virtual bool HasTool()
    {
        return (this.GetTool() != null);
    }

    public bool IsLocalUsing()
    {
        return (this._currentlyUsingPlayer == NetCull.player);
    }

    public bool IsWorking()
    {
        return !(this._workDuration == -1f);
    }

    public static void Log<T>(T a)
    {
        if (debug_workbench)
        {
            Debug.Log(a);
        }
    }

    public static void Log<T>(T a, Object b)
    {
        if (debug_workbench)
        {
            Debug.Log(a, b);
        }
    }

    public static void LogError<T>(T a)
    {
        if (debug_workbench)
        {
            Debug.LogError(a);
        }
    }

    public static void LogError<T>(T a, Object b)
    {
        if (debug_workbench)
        {
            Debug.LogError(a, b);
        }
    }

    public static void LogWarning<T>(T a)
    {
        if (debug_workbench)
        {
            Debug.LogWarning(a);
        }
    }

    public static void LogWarning<T>(T a, Object b)
    {
        if (debug_workbench)
        {
            Debug.LogWarning(a, b);
        }
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

    private void SendWorkStatusUpdate()
    {
        if (this._currentlyUsingPlayer != NetworkPlayer.unassigned)
        {
            float num = (float) this._startTime_network;
            NetCull.RPC<float, float>((MonoBehaviour) this, "WorkStatusUpdate", this._currentlyUsingPlayer, num, this._workDuration);
        }
    }

    [RPC]
    private void SetUser(NetworkPlayer ply)
    {
        if (ply == NetCull.player)
        {
            RPOS.OpenWorkbenchWindow(this);
        }
        else if ((this._currentlyUsingPlayer == NetCull.player) && (ply != this._currentlyUsingPlayer))
        {
            this._currentlyUsingPlayer = NetworkPlayer.unassigned;
            RPOS.CloseWorkbenchWindow();
        }
        this._currentlyUsingPlayer = ply;
    }

    private void SharedAwake()
    {
        this._inventory = base.GetComponent<Inventory>();
    }

    private void StartWork()
    {
        if (this.EnsureWorkExists())
        {
            IToolItem tool = this.GetTool();
            if (tool != null)
            {
                this._startTime_network = NetCull.time;
                this._workDuration = this.GetWorkDuration();
                base.Invoke("CompleteWork", this._workDuration);
                this._inventory.locked = true;
                tool.StartWork();
                this.SendWorkStatusUpdate();
            }
        }
    }

    [RPC]
    private void StopUsing(NetworkMessageInfo info)
    {
        if (this._currentlyUsingPlayer == info.sender)
        {
            this._useable.Eject();
        }
    }

    [RPC]
    public void TakeAll()
    {
    }

    public void TryCancel()
    {
        this.CancelWork();
    }

    [RPC]
    private void WorkStatusUpdate(float startTime, float newWorkDuration)
    {
        this._startTime_network = startTime;
        this._workDuration = newWorkDuration;
        RPOS.GetWindowByName("Workbench").GetComponent<RPOSWorkbenchWindow>().BenchUpdate();
    }

    private static bool debug_workbench
    {
        get
        {
            return true;
        }
    }
}

