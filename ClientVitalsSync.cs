using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public sealed class ClientVitalsSync : IDLocalCharacterAddon, IInterpTimedEventReceiver
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map2;
    private const IDLocalCharacterAddon.AddonFlags ClientVitalsSyncAddonFlags = (IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake | IDLocalCharacterAddon.AddonFlags.PrerequisitCheck);
    private static HUDDirectionalDamage hudDamagePrefab;
    [NonSerialized]
    private HumanBodyTakeDamage humanBodyTakeDamage;

    public ClientVitalsSync() : this(IDLocalCharacterAddon.AddonFlags.FireOnAddonPostAwake | IDLocalCharacterAddon.AddonFlags.PrerequisitCheck)
    {
    }

    protected ClientVitalsSync(IDLocalCharacterAddon.AddonFlags addonFlags) : base(addonFlags)
    {
    }

    protected override bool CheckPrerequesits()
    {
        this.humanBodyTakeDamage = base.takeDamage as HumanBodyTakeDamage;
        return ((this.humanBodyTakeDamage != null) && base.networkViewOwner.isClient);
    }

    public void ClientHealthChange(float amount, GameObject attacker)
    {
        float health = base.health;
        base.AdjustClientSideHealth(amount);
        float num2 = amount;
        float num3 = Mathf.Abs((float) (num2 - health));
        bool flag = amount < health;
        float healthFraction = base.healthFraction;
        if (base.localControlled && (num3 >= 1f))
        {
            base.GetComponent<LocalDamageDisplay>().SetNewHealthPercent(healthFraction, attacker);
        }
        if ((((attacker != null) && flag) && (num3 >= 1f)) && ((hudDamagePrefab != null) || Bundling.Load<HUDDirectionalDamage>("content/hud/DirectionalDamage", out hudDamagePrefab)))
        {
            Vector3 vector;
            Character character;
            if (IDBase.GetMain<Character>(attacker, out character))
            {
                vector = base.eyesOrigin - character.eyesOrigin;
            }
            else
            {
                vector = base.origin - attacker.transform.position;
            }
            HUDDirectionalDamage.CreateIndicator(vector, (double) amount, NetCull.time, 1.6000000238418579, hudDamagePrefab);
        }
        RPOS.HealthUpdate(amount);
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$map2 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                dictionary.Add("DMG", 0);
                <>f__switch$map2 = dictionary;
            }
            if (<>f__switch$map2.TryGetValue(tag, out num) && (num == 0))
            {
                this.ClientHealthChange(InterpTimedEvent.Argument<float>(0), InterpTimedEvent.Argument<GameObject>(1));
                return;
            }
        }
        InterpTimedEvent.MarkUnhandled();
    }

    [RPC]
    public void Local_BleedChange(float amount)
    {
        if (this.humanBodyTakeDamage != null)
        {
            this.humanBodyTakeDamage._bleedingLevel = amount;
        }
        if (base.localControlled)
        {
            RPOS.SetPlaqueActive("PlaqueBleeding", this.humanBodyTakeDamage._bleedingLevel > 0f);
        }
    }

    [RPC]
    public void Local_HealthChange(float amount, NetworkViewID attackerID, NetworkMessageInfo info)
    {
        GameObject gameObject;
        NetworkView view;
        if ((attackerID != NetworkViewID.unassigned) && ((view = NetworkView.Find(attackerID)) != null))
        {
            gameObject = view.gameObject;
        }
        else
        {
            gameObject = null;
        }
        object[] args = new object[] { amount, gameObject };
        InterpTimedEvent.Queue(this, "DMG", ref info, args);
    }

    protected override void OnAddonPostAwake()
    {
    }

    public bool bleeding
    {
        get
        {
            return ((this.humanBodyTakeDamage != null) && this.humanBodyTakeDamage.IsBleeding());
        }
    }
}

