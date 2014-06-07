using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public sealed class DeathTransfer : IDLocalCharacterAddon, IInterpTimedEventReceiver
{
    [NonSerialized]
    private GameObject _ragdollInstance;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map3;
    [NonSerialized]
    private QueuedShotDeathInfo deathShot;
    private const IDLocalCharacterAddon.AddonFlags DeathTransferAddonFlags = 0;

    public DeathTransfer() : this(0)
    {
    }

    protected DeathTransfer(IDLocalCharacterAddon.AddonFlags addonFlags) : base(addonFlags)
    {
    }

    [RPC]
    protected void Client_OnKilled(NetworkMessageInfo info)
    {
        this.Client_OnKilledShared(false, null, ref info);
    }

    [RPC]
    protected void Client_OnKilledBy(NetworkViewID attackerViewID, NetworkMessageInfo info)
    {
        NetworkView view = NetworkView.Find(attackerViewID);
        if (view == null)
        {
            this.Client_OnKilledShared(false, null, ref info);
        }
        else
        {
            Character component = view.GetComponent<Character>();
            this.Client_OnKilledShared((bool) component, component, ref info);
        }
    }

    private void Client_OnKilledShared(bool killedBy, Character attacker, ref NetworkMessageInfo info)
    {
        Controllable controllable;
        bool flag2;
        AudioClipArray death = base.GetTrait<CharacterSoundsTrait>().death;
        death[Random.Range(0, death.Length)].Play(base.transform.position, (float) 1f, (float) 1f, (float) 10f);
        bool localControlled = base.localControlled;
        if (localControlled)
        {
            base.rposLimitFlags = RPOSLimitFlags.HideSprites | RPOSLimitFlags.HideContext | RPOSLimitFlags.HideInventory | RPOSLimitFlags.KeepOff;
            DeathScreen.Show();
        }
        BaseHitBox remote = base.idMain.GetRemote<BaseHitBox>();
        if (remote != null)
        {
            remote.collider.enabled = false;
        }
        if (killedBy && (attacker != null))
        {
            controllable = attacker.controllable;
            flag2 = (controllable != null) && controllable.localPlayerControlled;
        }
        else
        {
            controllable = null;
            flag2 = false;
        }
        base.AdjustClientSideHealth(0f);
        if (!localControlled && !flag2)
        {
            foreach (Collider collider in base.GetComponentsInChildren<Collider>())
            {
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
            InterpTimedEvent.Queue(this, "RAG", ref info);
            NetCull.DontDestroyWithNetwork((MonoBehaviour) this);
        }
        else
        {
            InterpTimedEvent.Queue(this, "ClientLocalDeath", ref info);
            if (localControlled)
            {
                InterpTimedEvent.Clear(true);
            }
            else
            {
                InterpTimedEvent.Remove(this, true);
            }
        }
    }

    [RPC]
    protected void Client_OnKilledShot(Vector3 point, Angle2 normal, byte bodyPart, NetworkMessageInfo info)
    {
        this.Client_ShotShared(ref point, ref normal, bodyPart, ref info);
        this.Client_OnKilled(info);
    }

    [RPC]
    protected void Client_OnKilledShotBy(NetworkViewID attackerViewID, Vector3 point, Angle2 normal, byte bodyPart, NetworkMessageInfo info)
    {
        this.Client_ShotShared(ref point, ref normal, bodyPart, ref info);
        this.Client_OnKilledBy(attackerViewID, info);
    }

    private void Client_ShotShared(ref Vector3 point, ref Angle2 normal, byte bodyPart, ref NetworkMessageInfo info)
    {
        this.deathShot.Set(base.idMain.hitBoxSystem, ref point, ref normal, bodyPart, ref info);
    }

    private void ClientLocalDeath()
    {
        Ragdoll ragdoll = this.DeathRagdoll();
        if (base.localControlled)
        {
            if (!actor.forceThirdPerson)
            {
                CameraMount componentInChildren = base.GetComponentInChildren<CameraMount>();
                if ((componentInChildren != null) && componentInChildren.open)
                {
                    Transform transform;
                    bool flag;
                    RagdollTransferInfoProvider controller = base.controller as RagdollTransferInfoProvider;
                    if (controller != null)
                    {
                        try
                        {
                            flag = controller.RagdollTransferInfo.FindHead(ragdoll.transform, out transform);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception, this);
                            transform = null;
                            flag = false;
                        }
                    }
                    else
                    {
                        transform = null;
                        flag = false;
                    }
                    if (flag)
                    {
                        Vector3 position = transform.InverseTransformPoint(componentInChildren.transform.position);
                        position.y += 0.08f;
                        Vector3 vector2 = transform.TransformPoint(position);
                        ragdoll.origin += vector2 - transform.position;
                        CameraMount.CreateTemporaryCameraMount(componentInChildren, transform).camera.nearClipPlane = 0.02f;
                    }
                    ArmorModelRenderer local = ragdoll.GetLocal<ArmorModelRenderer>();
                    if (local != null)
                    {
                        local.enabled = false;
                    }
                }
                else
                {
                    Debug.Log("No camera?");
                }
            }
            Object.Destroy(base.GetComponent<LocalDamageDisplay>());
        }
    }

    private Ragdoll CreateRagdoll()
    {
        CharacterRagdollTrait trait = base.GetTrait<CharacterRagdollTrait>();
        if (trait == null)
        {
            return null;
        }
        GameObject obj2 = Object.Instantiate(trait.ragdollPrefab, base.transform.position, base.transform.rotation) as GameObject;
        Ragdoll component = obj2.GetComponent<Ragdoll>();
        component.sourceMain = base.idMain;
        this._ragdollInstance = obj2;
        Object.Destroy(obj2, 80f);
        this.deathShot.LinkRagdoll(base.transform, obj2);
        ArmorModelRenderer local = base.GetLocal<ArmorModelRenderer>();
        ArmorModelRenderer renderer2 = component.GetLocal<ArmorModelRenderer>();
        if ((local != null) && (renderer2 != null))
        {
            renderer2.BindArmorModels(local.GetArmorModelMemberMapCopy());
        }
        return component;
    }

    private Ragdoll DeathRagdoll()
    {
        Ragdoll ragdoll = this.CreateRagdoll();
        PlayerProxyTest component = base.GetComponent<PlayerProxyTest>();
        if (component.body != null)
        {
            component.body.SetActive(false);
        }
        return ragdoll;
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$map3 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                dictionary.Add("ClientLocalDeath", 0);
                dictionary.Add("RAG", 1);
                <>f__switch$map3 = dictionary;
            }
            if (<>f__switch$map3.TryGetValue(tag, out num))
            {
                if (num == 0)
                {
                    try
                    {
                        this.ClientLocalDeath();
                    }
                    finally
                    {
                        if (!base.localControlled)
                        {
                            Object.Destroy(base.gameObject);
                        }
                    }
                    return;
                }
                if (num == 1)
                {
                    try
                    {
                        this.DeathRagdoll();
                    }
                    finally
                    {
                        Object.Destroy(base.gameObject);
                    }
                    return;
                }
            }
        }
        InterpTimedEvent.MarkUnhandled();
    }
}

