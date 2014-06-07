using System;
using System.Runtime.InteropServices;
using UnityEngine;

[NGCAutoAddScript]
public class SleepingBag : DeployedRespawn, IContextRequestable, IContextRequestableQuick, IContextRequestableStatus, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    public string giveItemName;

    public ContextExecution ContextQuery(Controllable controllable, ulong timestamp)
    {
        return (ContextExecution.NotAvailable | ContextExecution.Quick);
    }

    public ContextResponse ContextRespondQuick(Controllable controllable, ulong timestamp)
    {
        this.PlayerUse(controllable);
        return ContextResponse.DoneBreak;
    }

    public ContextStatusFlags ContextStatusPoll()
    {
        PlayerClient localPlayerClient = PlayerClient.localPlayerClient;
        if ((localPlayerClient != null) && (localPlayerClient.userID == base.creatorID))
        {
            return 0;
        }
        return ContextStatusFlags.SpriteFlag1;
    }

    public string ContextText(Controllable localControllable)
    {
        PlayerClient playerClient = localControllable.playerClient;
        if ((playerClient != null) && (playerClient.userID == base.creatorID))
        {
            return "Pick Up";
        }
        return string.Empty;
    }

    bool IContextRequestablePointText.ContextTextPoint(out Vector3 worldPoint)
    {
        ContextRequestable.PointUtil.SpriteOrOrigin(this, out worldPoint);
        return true;
    }

    public void PlayerUse(Controllable controllable)
    {
        if ((base.BelongsTo(controllable) && this.IsValidToSpawn()) && (controllable.GetComponent<Inventory>().AddItemAmount(DatablockDictionary.GetByName(this.giveItemName), 1) != 1))
        {
        }
    }
}

