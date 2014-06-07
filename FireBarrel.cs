using RustProto;
using RustProto.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using uLink;
using UnityEngine;

[NGCAutoAddScript]
public class FireBarrel : LootableObject, IServerSaveable, IActivatable, IActivatableToggle, IContextRequestable, IContextRequestableMenu, IContextRequestableQuick, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IActivatable, MonoBehaviour, Activatable>, IComponentInterface<IActivatable, MonoBehaviour>, IComponentInterface<IActivatable>, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    private DeployableObject _deployable;
    public HeatZone _heatZone;
    private float _lightFlickerTarget = 1f;
    private float _lightIntensityInitial = 1f;
    private Vector3 _lightPosCurrent;
    private Vector3 _lightPosInitial;
    private Vector3 _lightPosTarget;
    public static float decayResetRange = 5f;
    public ParticleSystem[] emitters;
    public Light fireLight;
    public bool isOn;
    public int myTemp = 1;
    private static readonly FireBarrelPrototype optionExtinguish = new FireBarrelPrototype(FireBarrelAction.Extinguish);
    private static readonly FireBarrelPrototype optionIgnite = new FireBarrelPrototype(FireBarrelAction.Ignite);
    private static readonly FireBarrelPrototype optionOpen = new FireBarrelPrototype(FireBarrelAction.Open);
    public bool startOn;

    public ActivationToggleState ActGetToggleState()
    {
        return (!this.isOn ? ActivationToggleState.Off : ActivationToggleState.On);
    }

    public ActivationResult ActTrigger(Character instigator, ulong timestamp)
    {
        return this.ActTrigger(instigator, !this.isOn ? ActivationToggleState.On : ActivationToggleState.Off, timestamp);
    }

    public ActivationResult ActTrigger(Character instigator, ActivationToggleState toggleTarget, ulong timestamp)
    {
        ActivationToggleState state = toggleTarget;
        if (state != ActivationToggleState.On)
        {
            if (state != ActivationToggleState.Off)
            {
                return ActivationResult.Fail_BadToggle;
            }
        }
        else
        {
            if (this.isOn)
            {
                return ActivationResult.Fail_Redundant;
            }
            this.PlayerUse(null);
            return (!this.isOn ? ActivationResult.Fail_Busy : ActivationResult.Success);
        }
        if (!this.isOn)
        {
            return ActivationResult.Fail_Redundant;
        }
        this.PlayerUse(null);
        return (!this.isOn ? ActivationResult.Success : ActivationResult.Fail_Busy);
    }

    public void Awake()
    {
        this._lightPosInitial = this.fireLight.transform.localPosition;
        this._lightPosCurrent = this._lightPosInitial;
        this._lightPosTarget = this._lightPosCurrent;
        this._lightIntensityInitial = this.fireLight.intensity;
    }

    [DebuggerHidden]
    protected IEnumerable<ContextActionPrototype> ContextQueryMenu_FireBarrel(Controllable controllable, ulong timestamp)
    {
        return new <ContextQueryMenu_FireBarrel>c__IteratorB { <>f__this = this, $PC = -2 };
    }

    protected ContextResponse ContextRespond_SetFireBarrelOn(Controllable controllable, ulong timestamp, bool turnOn)
    {
        if (this.isOn == turnOn)
        {
            return ContextResponse.DoneBreak;
        }
        if (!this.isOn || this.HasFuel())
        {
            this.TrySetOn(!this.isOn);
            if (this.isOn != turnOn)
            {
                return ContextResponse.DoneBreak;
            }
        }
        return ContextResponse.FailBreak;
    }

    protected ContextResponse ContextRespondMenu_FireBarrel(Controllable controllable, FireBarrelPrototype action, ulong timestamp)
    {
        bool turnOn = action == optionIgnite;
        if (turnOn || (action == optionExtinguish))
        {
            return this.ContextRespond_SetFireBarrelOn(controllable, timestamp, turnOn);
        }
        if (action == optionOpen)
        {
            return this.ContextRespond_OpenLoot(controllable, timestamp);
        }
        return ContextResponse.FailBreak;
    }

    protected ContextResponse ContextRespondQuick_FireBarrel(Controllable controllable, ulong timestamp)
    {
        if (this.isOn)
        {
            return this.ContextRespond_SetFireBarrelOn(controllable, timestamp, false);
        }
        if (this.HasFuel())
        {
            return this.ContextRespond_SetFireBarrelOn(controllable, timestamp, true);
        }
        return this.ContextRespond_OpenLoot(controllable, timestamp);
    }

    public override string ContextText(Controllable localControllable)
    {
        if (base._currentlyUsingPlayer == NetworkPlayer.unassigned)
        {
            return "Use";
        }
        if (base.occupierText == null)
        {
            PlayerClient client;
            if (!PlayerClient.Find(base._currentlyUsingPlayer, out client))
            {
                base.occupierText = "Occupied";
            }
            else
            {
                base.occupierText = string.Format("Occupied by {0}", client.userName);
            }
        }
        return base.occupierText;
    }

    public override bool ContextTextPoint(out Vector3 worldPoint)
    {
        ContextRequestable.PointUtil.SpriteOrOrigin(this, out worldPoint);
        return true;
    }

    public IFlammableItem FindFuel()
    {
        IEnumerator<IFlammableItem> enumerator = base._inventory.FindItems<IFlammableItem>().GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                IFlammableItem current = enumerator.Current;
                if (current.flammable)
                {
                    return current;
                }
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
        return null;
    }

    public void FuelRemoveCheck()
    {
        if (!this.HasFuel())
        {
            this.SetOn(false);
        }
    }

    protected virtual float GetCookDuration()
    {
        return 60f;
    }

    public virtual bool HasFuel()
    {
        return (this.FindFuel() != null);
    }

    public void InvItemAdded()
    {
    }

    public void InvItemRemoved()
    {
    }

    private void NewFlickerTarget()
    {
        this._lightFlickerTarget = this._lightIntensityInitial * Random.Range((float) 0.75f, (float) 1.25f);
    }

    protected void OnDestroy()
    {
        this.TurnOff();
        base.OnDestroy();
    }

    private void PlayerUse(Controllable looter)
    {
        this.TrySetOn(!this.isOn);
    }

    public void ReadObjectSave(ref SavedObject saveobj)
    {
        if (saveobj.HasFireBarrel)
        {
            this.SetOn(saveobj.FireBarrel.OnFire);
        }
    }

    [RPC]
    protected void ReceiveNetState(bool on)
    {
        this.SetOn(on);
    }

    public void SetOn(bool on)
    {
        this.isOn = on;
        if (this.isOn)
        {
            this.TurnOn();
        }
        else
        {
            this.TurnOff();
        }
    }

    public virtual void TrySetOn(bool on)
    {
        this.SetOn(on);
    }

    private void TurnOff()
    {
        if (base.audio != null)
        {
            base.audio.Stop();
        }
        foreach (ParticleSystem system in this.emitters)
        {
            system.Stop();
        }
        if (this.fireLight != null)
        {
            this.fireLight.enabled = false;
            this.fireLight.intensity = 0f;
        }
    }

    private void TurnOn()
    {
        this.fireLight.enabled = true;
        foreach (ParticleSystem system in this.emitters)
        {
            system.Play();
        }
        base.audio.Play();
        this.NewFlickerTarget();
    }

    public void Update()
    {
        if (this.isOn)
        {
            if (this.fireLight.transform.localPosition == this._lightPosTarget)
            {
                this._lightPosTarget = this._lightPosInitial + ((Vector3) (Random.insideUnitSphere * 10f));
                this._lightPosCurrent = this.fireLight.transform.localPosition;
            }
            this.fireLight.intensity = Mathf.Lerp(this.fireLight.intensity, this._lightFlickerTarget, Time.deltaTime * 10f);
            if (Mathf.Abs((float) (this.fireLight.intensity - this._lightFlickerTarget)) < 0.05)
            {
                this.NewFlickerTarget();
            }
        }
    }

    public void WriteObjectSave(ref SavedObject.Builder saveobj)
    {
        using (Recycler<objectFireBarrel, objectFireBarrel.Builder> recycler = objectFireBarrel.Recycler())
        {
            objectFireBarrel.Builder builderForValue = recycler.OpenBuilder();
            builderForValue.SetOnFire(this.isOn);
            saveobj.SetFireBarrel(builderForValue);
        }
    }

    [CompilerGenerated]
    private sealed class <ContextQueryMenu_FireBarrel>c__IteratorB : IDisposable, IEnumerator, IEnumerable, IEnumerable<ContextActionPrototype>, IEnumerator<ContextActionPrototype>
    {
        internal ContextActionPrototype $current;
        internal int $PC;
        internal FireBarrel <>f__this;

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
                    if (!(this.<>f__this._currentlyUsingPlayer == NetworkPlayer.unassigned))
                    {
                        goto Label_00AD;
                    }
                    if (!this.<>f__this.isOn)
                    {
                        if (this.<>f__this.HasFuel())
                        {
                            this.$current = FireBarrel.optionIgnite;
                            this.$PC = 2;
                            goto Label_00B6;
                        }
                        break;
                    }
                    this.$current = FireBarrel.optionExtinguish;
                    this.$PC = 1;
                    goto Label_00B6;

                case 1:
                case 2:
                    break;

                case 3:
                    goto Label_00AD;

                default:
                    goto Label_00B4;
            }
            this.$current = FireBarrel.optionOpen;
            this.$PC = 3;
            goto Label_00B6;
        Label_00AD:
            this.$PC = -1;
        Label_00B4:
            return false;
        Label_00B6:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<ContextActionPrototype> IEnumerable<ContextActionPrototype>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new FireBarrel.<ContextQueryMenu_FireBarrel>c__IteratorB { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<ContextActionPrototype>.GetEnumerator();
        }

        ContextActionPrototype IEnumerator<ContextActionPrototype>.Current
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

    public static class DefaultItems
    {
        public static Datablock.Ident byProduct = "Charcoal";
        public static Datablock.Ident fuel = "Wood";
    }

    protected enum FireBarrelAction
    {
        Ignite,
        Extinguish,
        Open
    }

    protected class FireBarrelPrototype : ContextActionPrototype
    {
        public FireBarrel.FireBarrelAction action;

        public FireBarrelPrototype(FireBarrel.FireBarrelAction action)
        {
            base.name = (int) action;
            base.text = action.ToString();
            this.action = action;
        }
    }
}

