using System;
using uLink;
using UnityEngine;

public class CraftingInventory : Inventory
{
    private double _lastThinkTime;
    public float _lastWorkBenchTime;
    protected bool _wasAtWorkbench;
    private const string CancelCraftingRPC = "CRFX";
    private CraftingSession crafting;
    private const string CraftNetworkClearRPC = "CRFC";
    private const string CraftNetworkUpdateRPC = "CRFU";
    private const string StartCraftingRPC = "CRFS";

    public bool AtWorkBench()
    {
        return (this._lastWorkBenchTime < 0f);
    }

    public bool CancelCrafting()
    {
        if (this.crafting.inProgress)
        {
            base.networkView.RPC("CRFX", RPCMode.Server, new object[0]);
            this.crafting.inProgress = false;
            return true;
        }
        return false;
    }

    public void CraftThink()
    {
        if (this.crafting.inProgress)
        {
            double time = NetCull.time;
            float num2 = (float) (time - this._lastThinkTime);
            this.crafting.progressSeconds = Mathf.Clamp(this.crafting.progressSeconds + (this.crafting.progressPerSec * num2), 0f, this.crafting.duration);
            this._lastThinkTime = time;
        }
    }

    [RPC, NGCRPCSkip]
    protected void CRFC()
    {
        this.UpdateCrafting(null, 0, 0f, 0f, 0f, 0f);
    }

    [RPC, NGCRPCSkip]
    protected void CRFS(int amount, int blueprintUID, NetworkMessageInfo info)
    {
    }

    [RPC, NGCRPCSkip]
    protected void CRFU(float start, float dur, float progresspersec, float progress, int blueprintUniqueID, int amount)
    {
        this.UpdateCrafting(FindBlueprint(blueprintUniqueID), amount, start, dur, progress, progresspersec);
    }

    [NGCRPCSkip, RPC]
    protected void CRFX()
    {
    }

    private static BlueprintDataBlock FindBlueprint(int uniqueID)
    {
        if (uniqueID == 0)
        {
            return null;
        }
        return (BlueprintDataBlock) DatablockDictionary.GetByUniqueID(uniqueID);
    }

    public bool StartCrafting(BlueprintDataBlock blueprint, int amount)
    {
        if (blueprint.CanWork(amount, this))
        {
            object[] args = new object[] { amount, blueprint.uniqueID };
            base.networkView.RPC("CRFS", RPCMode.Server, args);
            return true;
        }
        return false;
    }

    protected void UpdateCrafting(BlueprintDataBlock blueprint, int amount, float start, float dur, float progress, float progresspersec)
    {
        Debug.Log(string.Format("Craft network update :{0}:", (blueprint == null) ? "NONE" : blueprint.name), this);
        this._lastThinkTime = NetCull.time;
        this.crafting.blueprint = blueprint;
        this.crafting.inProgress = (bool) blueprint;
        this.crafting.startTime = start;
        this.crafting.duration = dur;
        this.crafting.progressSeconds = progress;
        this.crafting.progressPerSec = progresspersec;
        this.crafting.amount = amount;
        this.Refresh();
    }

    public bool ValidateCraftRequirements(BlueprintDataBlock bp)
    {
        return (!bp.RequireWorkbench || this.AtWorkBench());
    }

    [RPC]
    public void wbi(bool at, NetworkMessageInfo info)
    {
        if (at)
        {
            this._lastWorkBenchTime = float.NegativeInfinity;
        }
        else
        {
            this._lastWorkBenchTime = float.PositiveInfinity;
        }
    }

    public float? craftingCompletePercent
    {
        get
        {
            if (this.crafting.inProgress)
            {
                return new float?((float) this.crafting.percentComplete);
            }
            return null;
        }
    }

    public float? craftingSecondsRemaining
    {
        get
        {
            if (this.crafting.inProgress)
            {
                return new float?(this.crafting.remainingSeconds);
            }
            return null;
        }
    }

    public float craftingSpeedPerSec
    {
        get
        {
            return this.crafting.progressPerSec;
        }
    }

    public bool isCrafting
    {
        get
        {
            return this.crafting.inProgress;
        }
    }

    public bool isCraftingInventory
    {
        get
        {
            return true;
        }
    }
}

