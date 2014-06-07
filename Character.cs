using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class Character : IDMain
{
    [NonSerialized]
    private NavMeshAgent _agent;
    [NonSerialized]
    private bool _attemptedTraitMapLoad;
    [NonSerialized]
    private CCMotor _ccmotor;
    [SerializeField, PrefetchComponent]
    private Controllable _controllable;
    [PrefetchComponent, SerializeField]
    private Crouchable _crouchable;
    private Angle2 _eyesAngles;
    private Vector3 _eyesOffset;
    [NonSerialized]
    private bool _eyesSetup;
    [SerializeField, PrefetchChildComponent(NameMask="*Eyes")]
    private Transform _eyesTransform;
    [PrefetchChildComponent, SerializeField]
    private HitBoxSystem _hitBoxSystem;
    [PrefetchComponent, SerializeField]
    private IDLocalCharacterIdleControl _idleControl;
    private Vector3 _initialEyesOffset;
    [NonSerialized]
    private CharacterInterpolatorBase _interpolator;
    [SerializeField]
    private float _maxPitch;
    [SerializeField]
    private float _minPitch;
    [NonSerialized]
    private bool _originSetup;
    [NonSerialized]
    private IDLocalCharacterAddon _overlay;
    [SerializeField, PrefetchComponent]
    private RecoilSimulation _recoilSimulation;
    [NonSerialized]
    private bool _signaledDeath;
    [SerializeField, PrefetchComponent]
    private TakeDamage _takeDamage;
    [NonSerialized]
    private CharacterTraitMap _traitMap;
    [NonSerialized]
    private bool _traitMapLoaded;
    [SerializeField]
    private string _traitMapName;
    [SerializeField, PrefetchComponent]
    private VisNode _visNode;
    [NonSerialized]
    private bool didControllableTest;
    [NonSerialized]
    private bool didCrouchableTest;
    [NonSerialized]
    private bool didHitBoxSystemTest;
    [NonSerialized]
    private bool didIdleControlTest;
    [NonSerialized]
    private bool didRecoilSimulationTest;
    [NonSerialized]
    private bool didTakeDamageTest;
    [NonSerialized]
    private bool didVisNodeTest;
    [NonSerialized]
    public bool lockLook;
    [NonSerialized]
    public bool lockMovement;
    [NonSerialized]
    private CharacterDeathSignal signals_death;
    [NonSerialized]
    private CharacterStateSignal signals_state;
    [NonSerialized]
    public CharacterStateFlags stateFlags;

    public event CharacterDeathSignal signal_death
    {
        add
        {
        }
        remove
        {
            if (!this._signaledDeath)
            {
                this.signals_death = (CharacterDeathSignal) Delegate.Remove(this.signals_death, value);
            }
        }
    }

    public event CharacterStateSignal signal_state
    {
        add
        {
            if (!this._signaledDeath)
            {
                this.signals_state = (CharacterStateSignal) Delegate.Combine(this.signals_state, value);
            }
        }
        remove
        {
            if (!this._signaledDeath)
            {
                this.signals_state = (CharacterStateSignal) Delegate.Remove(this.signals_state, value);
            }
        }
    }

    public Character() : this(IDFlags.Character)
    {
    }

    protected Character(IDFlags flags) : base(flags)
    {
        this._traitMapName = "Default";
        this._maxPitch = 89.9f;
        this._minPitch = -89.9f;
    }

    public T AddAddon<T>() where T: IDLocalCharacterAddon, new()
    {
        if (!AddonRegistry<T>.valid)
        {
            throw new ArgumentOutOfRangeException("T");
        }
        T addon = base.GetLocal<T>();
        if (addon == null)
        {
            addon = base.gameObject.AddComponent<T>();
        }
        return (!this.InitAddon(addon) ? null : addon);
    }

    public TBase AddAddon<TBase, T>() where TBase: IDLocalCharacterAddon where T: TBase, new()
    {
        return this.AddAddon<T>();
    }

    public IDLocalCharacterAddon AddAddon(string addonTypeName)
    {
        if (!AddonStringRegistry.Validate(addonTypeName))
        {
            throw new ArgumentOutOfRangeException("addonTypeName", addonTypeName);
        }
        IDLocalCharacterAddon component = (IDLocalCharacterAddon) base.GetComponent(addonTypeName);
        if (component == null)
        {
            component = (IDLocalCharacterAddon) base.gameObject.AddComponent(addonTypeName);
        }
        return (!this.InitAddon(component) ? null : component);
    }

    public TBase AddAddon<TBase>(string addonTypeName) where TBase: IDLocalCharacterAddon
    {
        Type type;
        if (!AddonStringRegistry.Validate<TBase>(addonTypeName, out type))
        {
            throw new ArgumentOutOfRangeException("TBase", addonTypeName);
        }
        TBase addon = base.GetLocal<TBase>();
        if (addon == null)
        {
            addon = (TBase) base.gameObject.AddComponent(addonTypeName);
        }
        else if (!type.IsAssignableFrom(addon.GetType()))
        {
            throw new InvalidOperationException("The existing TBase component was not assignable to addonType");
        }
        return (!this.InitAddon(addon) ? null : addon);
    }

    public IDLocalCharacterAddon AddAddon(Type addonType)
    {
        if (!AddonRegistry.Validate(addonType))
        {
            throw new ArgumentOutOfRangeException("addonType", Convert.ToString(addonType));
        }
        IDLocalCharacterAddon component = (IDLocalCharacterAddon) base.GetComponent(addonType);
        if (component == null)
        {
            component = (IDLocalCharacterAddon) base.gameObject.AddComponent(addonType);
        }
        return (!this.InitAddon(component) ? null : component);
    }

    public TBase AddAddon<TBase>(Type addonType) where TBase: IDLocalCharacterAddon
    {
        if (!typeof(TBase).IsAssignableFrom(addonType))
        {
            throw new ArgumentOutOfRangeException("addonType", Convert.ToString(addonType));
        }
        if (!AddonRegistry.Validate(addonType))
        {
            throw new ArgumentOutOfRangeException("addonType", Convert.ToString(addonType));
        }
        TBase component = base.GetComponent<TBase>();
        if (component == null)
        {
            component = (TBase) base.gameObject.AddComponent(addonType);
        }
        else if (!addonType.IsAssignableFrom(component.GetType()))
        {
            throw new InvalidOperationException("The existing TBase component was not assignable to addonType");
        }
        return (!this.InitAddon(component) ? null : component);
    }

    public IDLocalCharacterAddon AddAddon(string addonTypeName, Type minimumType)
    {
        if (!AddonStringRegistry.Validate(addonTypeName, minimumType))
        {
            throw new ArgumentOutOfRangeException("addonTypeName", addonTypeName);
        }
        IDLocalCharacterAddon component = (IDLocalCharacterAddon) base.GetComponent(addonTypeName);
        if (component == null)
        {
            component = (IDLocalCharacterAddon) base.gameObject.AddComponent(addonTypeName);
        }
        return (!this.InitAddon(component) ? null : component);
    }

    public IDLocalCharacterAddon AddAddon(Type addonType, Type minimumType)
    {
        if (!minimumType.IsAssignableFrom(addonType))
        {
            throw new ArgumentOutOfRangeException("minimumType", Convert.ToString(addonType));
        }
        return this.AddAddon(addonType);
    }

    public void AdjustClientSideHealth(float newHealthValue)
    {
        if (this.takeDamage != null)
        {
            this._takeDamage.health = newHealthValue;
        }
    }

    protected virtual void AlterEyesLocalOrigin(ref Vector3 localPosition)
    {
        if (this.crouchable != null)
        {
            this._crouchable.ApplyCrouch(ref localPosition);
        }
    }

    public void ApplyAdditiveEyeAngles(Angle2 angles)
    {
        float v = this._eyesAngles.pitch + angles.pitch;
        this.ClampPitch(ref v);
        if (angles.yaw != 0f)
        {
            this._eyesAngles.yaw = Mathf.DeltaAngle(0f, this._eyesAngles.yaw + angles.yaw);
            this._eyesAngles.pitch = v;
            this.InvalidateEyesAngles();
        }
        else if (v != angles.pitch)
        {
            this._eyesAngles.pitch = v;
            this.InvalidateEyesAngles();
        }
    }

    public bool AssignedControlOf(Controllable controllable)
    {
        return ((this.controllable != null) && this._controllable.AssignedControlOf(controllable));
    }

    public bool AssignedControlOf(Controller controller)
    {
        return ((this.controllable != null) && this._controllable.AssignedControlOf(controller));
    }

    public bool AssignedControlOf(IDBase idBase)
    {
        return ((this.controllable != null) && this._controllable.AssignedControlOf(idBase));
    }

    public bool AssignedControlOf(IDMain character)
    {
        return ((this.controllable != null) && this._controllable.AssignedControlOf(character));
    }

    public bool AttentionMessage(string message)
    {
        return VisNode.AttentionMessage(this.visNode, message, null);
    }

    public bool AttentionMessage(string message, object arg)
    {
        return VisNode.AttentionMessage(this.visNode, message, arg);
    }

    public bool AudibleMessage(float hearRadius, string message)
    {
        return VisNode.AudibleMessage(this.visNode, hearRadius, message);
    }

    public bool AudibleMessage(float hearRadius, string message, object arg)
    {
        return VisNode.AudibleMessage(this.visNode, hearRadius, message, arg);
    }

    public bool AudibleMessage(Vector3 point, float hearRadius, string message)
    {
        return VisNode.AudibleMessage(this.visNode, point, hearRadius, message);
    }

    public bool AudibleMessage(Vector3 point, float hearRadius, string message, object arg)
    {
        return VisNode.AudibleMessage(this._visNode, point, hearRadius, message, arg);
    }

    protected void Awake()
    {
        if (!this._originSetup)
        {
            this.OriginSetup();
        }
        if (!this._eyesSetup)
        {
            this.EyesSetup();
        }
    }

    public bool CanSee(Character other)
    {
        return ((((this.visNode != null) && (other != null)) && (other.visNode != null)) && this._visNode.CanSee(other._visNode));
    }

    public bool CanSee(IDMain other)
    {
        if (other is Character)
        {
            return this.CanSee((Character) other);
        }
        return ((other != null) && this.CanSee(other.GetLocal<VisNode>()));
    }

    public bool CanSee(VisNode other)
    {
        return ((this.visNode != null) && this._visNode.CanSee(other));
    }

    public bool CanSee(Character other, bool unobstructed)
    {
        return (!unobstructed ? this.CanSee(other) : this.CanSeeUnobstructed(other));
    }

    public bool CanSee(IDMain other, bool unobstructed)
    {
        return (!unobstructed ? this.CanSee(other) : this.CanSeeUnobstructed(other));
    }

    public bool CanSee(VisNode other, bool unobstructed)
    {
        return (!unobstructed ? this.CanSee(other) : this.CanSeeUnobstructed(other));
    }

    public bool CanSeeUnobstructed(Character other)
    {
        return ((((this.visNode != null) && (other != null)) && (other.visNode != null)) && this._visNode.CanSeeUnobstructed(other._visNode));
    }

    public bool CanSeeUnobstructed(IDMain other)
    {
        if (other is Character)
        {
            return this.CanSeeUnobstructed((Character) other);
        }
        return ((other != null) && this.CanSeeUnobstructed(other.GetLocal<VisNode>()));
    }

    public bool CanSeeUnobstructed(VisNode other)
    {
        return ((this.visNode != null) && this._visNode.CanSeeUnobstructed(other));
    }

    public Angle2 ClampPitch(Angle2 v)
    {
        this.ClampPitch(ref v.pitch);
        return v;
    }

    public float ClampPitch(float v)
    {
        return ((v >= this._minPitch) ? ((v <= this._maxPitch) ? v : this._maxPitch) : this._minPitch);
    }

    public void ClampPitch(ref Angle2 v)
    {
        this.ClampPitch(ref v.pitch);
    }

    public void ClampPitch(ref float v)
    {
        if (v < this._minPitch)
        {
            v = this._minPitch;
        }
        else if (v > this._maxPitch)
        {
            v = this._maxPitch;
        }
    }

    public bool ContactMessage(string message)
    {
        return VisNode.ContactMessage(this.visNode, message, null);
    }

    public bool ContactMessage(string message, object arg)
    {
        return VisNode.ContactMessage(this.visNode, message, arg);
    }

    public bool ControlOverriddenBy(Character character)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(character));
    }

    public bool ControlOverriddenBy(Controllable controllable)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(controllable));
    }

    public bool ControlOverriddenBy(Controller controller)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(controller));
    }

    public bool ControlOverriddenBy(IDBase idBase)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(idBase));
    }

    public bool ControlOverriddenBy(IDLocalCharacter idLocal)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(idLocal));
    }

    public bool ControlOverriddenBy(IDMain main)
    {
        return ((this.controllable != null) && this._controllable.ControlOverriddenBy(main));
    }

    public bool CreateCCMotor()
    {
        if (this._ccmotor != null)
        {
            return true;
        }
        CharacterCCMotorTrait trait = this.GetTrait<CharacterCCMotorTrait>();
        CCTotemPole cc = (CCTotemPole) Object.Instantiate(trait.prefab, this.origin, Quaternion.identity);
        this._ccmotor = cc.GetComponent<CCMotor>();
        if (this._ccmotor == null)
        {
            this._ccmotor = cc.gameObject.AddComponent<CCMotor>();
            if (this._ccmotor == null)
            {
                return false;
            }
        }
        this._ccmotor.InitializeSetup(this, cc, trait);
        return (bool) this._ccmotor;
    }

    public bool CreateInterpolator()
    {
        if (this._interpolator != null)
        {
            return true;
        }
        CharacterInterpolatorTrait trait = this.GetTrait<CharacterInterpolatorTrait>();
        if (trait == null)
        {
            return false;
        }
        this._interpolator = this.AddAddon<CharacterInterpolatorBase>(trait.interpolatorComponentTypeName);
        return (bool) this._interpolator;
    }

    public bool CreateNavMeshAgent()
    {
        if (this._agent == null)
        {
            CharacterNavAgentTrait trait = this.GetTrait<CharacterNavAgentTrait>();
            if (trait == null)
            {
                return false;
            }
            this._agent = base.GetComponent<NavMeshAgent>();
            if (this._agent == null)
            {
                this._agent = base.gameObject.AddComponent<NavMeshAgent>();
            }
            trait.CopyTo(this._agent);
        }
        return true;
    }

    public bool CreateOverlay()
    {
        if (this._overlay != null)
        {
            return true;
        }
        CharacterOverlayTrait trait = this.GetTrait<CharacterOverlayTrait>();
        if ((trait == null) || string.IsNullOrEmpty(trait.overlayComponentName))
        {
            return false;
        }
        this._overlay = this.AddAddon(trait.overlayComponentName);
        return (bool) this._overlay;
    }

    [DebuggerHidden]
    public static IEnumerable<Character> CurrentCharacters(IEnumerable<PlayerClient> playerClients)
    {
        return new <CurrentCharacters>c__Iterator1C { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    [DebuggerHidden]
    public static IEnumerable<TCharacter> CurrentCharacters<TCharacter>(IEnumerable<PlayerClient> playerClients) where TCharacter: Character
    {
        return new <CurrentCharacters>c__Iterator1E<TCharacter> { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    public void DestroyCCMotor()
    {
    }

    public void DestroyInterpolator()
    {
        this.RemoveAddon<CharacterInterpolatorBase>(ref this._interpolator);
    }

    public void DestroyNavMeshAgent()
    {
        Object.Destroy(this._agent);
        this._agent = null;
    }

    public void DestroyOverlay()
    {
        this.RemoveAddon<IDLocalCharacterAddon>(ref this._overlay);
    }

    protected virtual void DoDestroy()
    {
    }

    private void EyesSetup()
    {
        if (!this._originSetup)
        {
            this.OriginSetup();
        }
        if ((this._eyesTransform == null) || (this._eyesTransform.parent != base.transform))
        {
            Debug.LogError("eyes Transform is null or it is not a direct child of our transform.", this);
        }
        else
        {
            this._initialEyesOffset = this._eyesOffset = this._eyesTransform.localPosition;
            this._eyesAngles.x = -this._eyesTransform.localEulerAngles.x;
            this._eyesAngles.y = base.transform.localEulerAngles.y;
            this._eyesTransform.localEulerAngles = this._eyesAngles.pitchEulerAngles;
            this._eyesSetup = true;
        }
    }

    public bool GestureMessage(string message)
    {
        return VisNode.GestureMessage(this.visNode, message, null);
    }

    public bool GestureMessage(string message, object arg)
    {
        return VisNode.GestureMessage(this.visNode, message, arg);
    }

    public TCharacterTrait GetTrait<TCharacterTrait>() where TCharacterTrait: CharacterTrait
    {
        return (!this.traitMapLoaded ? null : this._traitMap.GetTrait<TCharacterTrait>());
    }

    public CharacterTrait GetTrait(Type characterTraitType)
    {
        return (!this.traitMapLoaded ? null : this._traitMap.GetTrait(characterTraitType));
    }

    private bool InitAddon(IDLocalCharacterAddon addon)
    {
        byte num = addon.InitializeAddon(this);
        if ((num & 8) == 8)
        {
            return false;
        }
        if ((num & 2) == 2)
        {
            addon.PostInitializeAddon();
        }
        return true;
    }

    protected void InvalidateEyesAngles()
    {
        base.transform.localEulerAngles = this._eyesAngles.yawEulerAngles;
        this._eyesTransform.localEulerAngles = this._eyesAngles.pitchEulerAngles;
    }

    protected internal void InvalidateEyesOffset()
    {
        Vector3 localPosition = this._eyesOffset;
        this.AlterEyesLocalOrigin(ref localPosition);
        this._eyesTransform.localPosition = localPosition;
    }

    private void LoadTraitMap()
    {
        this._traitMapLoaded = TraitMap<CharacterTrait, CharacterTraitMap>.ByName(this._traitMapName, out this._traitMap);
        this._attemptedTraitMapLoad = true;
    }

    protected void LoadTraitMapNonNetworked()
    {
        if (!this._traitMapLoaded)
        {
            this.LoadTraitMap();
        }
    }

    public bool ObliviousMessage(string message)
    {
        return VisNode.ObliviousMessage(this.visNode, message, null);
    }

    public bool ObliviousMessage(string message, object arg)
    {
        return VisNode.ObliviousMessage(this.visNode, message, arg);
    }

    private void OnDestroy()
    {
        try
        {
            this.DoDestroy();
        }
        finally
        {
            if (this.signals_state != null)
            {
                this.signals_state = null;
            }
            base.OnDestroy();
        }
    }

    private void OriginSetup()
    {
        this._originSetup = true;
    }

    public bool OverridingControlOf(Character character)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(character));
    }

    public bool OverridingControlOf(Controllable controllable)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(controllable));
    }

    public bool OverridingControlOf(Controller controller)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(controller));
    }

    public bool OverridingControlOf(IDBase idBase)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(idBase));
    }

    public bool OverridingControlOf(IDLocalCharacter idLocal)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(idLocal));
    }

    public bool OverridingControlOf(IDMain main)
    {
        return ((this.controllable != null) && this._controllable.OverridingControlOf(main));
    }

    public bool PreyMessage(string message)
    {
        return VisNode.PreyMessage(this.visNode, message, null);
    }

    public bool PreyMessage(string message, object arg)
    {
        return VisNode.PreyMessage(this.visNode, message, arg);
    }

    public RelativeControl RelativeControlFrom(Character character)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(character));
    }

    public RelativeControl RelativeControlFrom(Controllable controllable)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(controllable));
    }

    public RelativeControl RelativeControlFrom(Controller controller)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(controller));
    }

    public RelativeControl RelativeControlFrom(IDBase idBase)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(idBase));
    }

    public RelativeControl RelativeControlFrom(IDLocalCharacter idLocal)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(idLocal));
    }

    public RelativeControl RelativeControlFrom(IDMain main)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlFrom(main));
    }

    public RelativeControl RelativeControlTo(Character character)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(character));
    }

    public RelativeControl RelativeControlTo(Controllable controllable)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(controllable));
    }

    public RelativeControl RelativeControlTo(Controller controller)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(controller));
    }

    public RelativeControl RelativeControlTo(IDBase idBase)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(idBase));
    }

    public RelativeControl RelativeControlTo(IDLocalCharacter idLocal)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(idLocal));
    }

    public RelativeControl RelativeControlTo(IDMain main)
    {
        return ((this.controllable == null) ? RelativeControl.Incompatible : this._controllable.RelativeControlTo(main));
    }

    public void RemoveAddon(IDLocalCharacterAddon addon)
    {
        if (addon != null)
        {
            addon.RemoveAddon();
        }
    }

    public void RemoveAddon<T>(ref T addon) where T: IDLocalCharacterAddon
    {
        this.RemoveAddon((T) addon);
        addon = null;
    }

    [DebuggerHidden]
    public static IEnumerable<Character> RootCharacters(IEnumerable<PlayerClient> playerClients)
    {
        return new <RootCharacters>c__Iterator1B { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    [DebuggerHidden]
    public static IEnumerable<TCharacter> RootCharacters<TCharacter>(IEnumerable<PlayerClient> playerClients) where TCharacter: Character
    {
        return new <RootCharacters>c__Iterator1D<TCharacter> { playerClients = playerClients, <$>playerClients = playerClients, $PC = -2 };
    }

    protected static bool SeekComponentInChildren<M, T>(M main, ref T component) where M: IDMain where T: Component
    {
        if (((T) component) == null)
        {
            component = main.GetComponent<T>();
            return (T) component;
        }
        return true;
    }

    protected static bool SeekIDLocalComponentInChildren<M, T>(M main, ref T component) where M: IDMain where T: IDLocal
    {
        if (((T) component) != null)
        {
            if (((T) component).idMain == main)
            {
                return true;
            }
            if (((T) component).idMain == null)
            {
                return true;
            }
        }
        component = main.GetComponent<T>();
        if (((T) component) != null)
        {
            if (((T) component).idMain == main)
            {
                return true;
            }
            if (((T) component).idMain == null)
            {
                return true;
            }
            T[] components = main.GetComponents<T>();
            if (components.Length <= 1)
            {
                component = null;
                return false;
            }
            foreach (T local in components)
            {
                if (local.idMain == main)
                {
                    component = local;
                    return true;
                }
            }
            component = null;
        }
        return false;
    }

    protected static bool SeekIDRemoteComponentInChildren<M, T>(M main, ref T component) where M: IDMain where T: IDRemote
    {
        if (((T) component) != null)
        {
            if (component.idMain == main)
            {
                return true;
            }
            if (component.idMain == null)
            {
                return true;
            }
        }
        component = main.GetComponentInChildren<T>();
        if (((T) component) != null)
        {
            if (component.idMain == main)
            {
                return true;
            }
            if (component.idMain == null)
            {
                return true;
            }
            T[] componentsInChildren = main.GetComponentsInChildren<T>();
            if (componentsInChildren.Length <= 1)
            {
                component = null;
                return false;
            }
            foreach (T local in componentsInChildren)
            {
                if (local.idMain == main)
                {
                    component = local;
                    return true;
                }
            }
            component = null;
        }
        return false;
    }

    private void signal_death_now(CharacterDeathSignalReason reason)
    {
    }

    public void Signal_ServerCharacterDeath()
    {
        this.signal_death_now(CharacterDeathSignalReason.Died);
    }

    public void Signal_ServerCharacterDeathReset()
    {
    }

    public void Signal_State_FlagsChanged(bool asFirst)
    {
        if (this.signals_state != null)
        {
            try
            {
                this.signals_state(this, asFirst);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception, this);
            }
        }
    }

    public bool StealthMessage(string message)
    {
        return VisNode.StealthMessage(this.visNode, message, null);
    }

    public bool StealthMessage(string message, object arg)
    {
        return VisNode.StealthMessage(this.visNode, message, arg);
    }

    public NavMeshAgent agent
    {
        get
        {
            return this._agent;
        }
    }

    public bool aiControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.aiControlled);
        }
    }

    public Controllable aiControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.aiControlled) ? null : this._controllable);
        }
    }

    public Controller aiControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.aiControlledController);
        }
    }

    public bool alive
    {
        get
        {
            return ((this.takeDamage == null) || this._takeDamage.alive);
        }
    }

    public bool assignedControl
    {
        get
        {
            return ((this.controllable != null) && this._controllable.assignedControl);
        }
    }

    public bool blind
    {
        get
        {
            return ((this.visNode == null) || this._visNode.blind);
        }
        set
        {
            if (this.visNode != null)
            {
                this._visNode.blind = value;
            }
            else if (!value)
            {
                Debug.LogError("no visnode", this);
            }
        }
    }

    public CCMotor ccmotor
    {
        get
        {
            return this._ccmotor;
        }
    }

    [Obsolete("this is the character")]
    public Character character
    {
        get
        {
            return this;
        }
    }

    public int controlCount
    {
        get
        {
            return ((this.controllable == null) ? 0 : this._controllable.controlCount);
        }
    }

    public int controlDepth
    {
        get
        {
            return ((this.controllable == null) ? -1 : this._controllable.controlDepth);
        }
    }

    public Controllable controllable
    {
        get
        {
            if (!this.didControllableTest)
            {
                SeekComponentInChildren<Character, Controllable>(this, ref this._controllable);
                this.didControllableTest = true;
            }
            return this._controllable;
        }
    }

    public bool controlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.controlled);
        }
    }

    public Controllable controlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.controlled) ? null : this._controllable);
        }
    }

    public Controller controlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.controlledController);
        }
    }

    public Controller controller
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.controller);
        }
    }

    public string controllerClassName
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.controllerClassName);
        }
    }

    public bool controlOverridden
    {
        get
        {
            return ((this.controllable != null) && this._controllable.controlOverridden);
        }
    }

    public bool core
    {
        get
        {
            return ((this.controllable != null) && this._controllable.core);
        }
    }

    public Crouchable crouchable
    {
        get
        {
            if (!this.didCrouchableTest)
            {
                SeekIDLocalComponentInChildren<Character, Crouchable>(this, ref this._crouchable);
                this.didCrouchableTest = true;
            }
            return this._crouchable;
        }
    }

    public bool crouched
    {
        get
        {
            return ((this.crouchable != null) && this.crouchable.crouched);
        }
    }

    public bool dead
    {
        get
        {
            return ((this.takeDamage != null) && this._takeDamage.dead);
        }
    }

    public bool deaf
    {
        get
        {
            return ((this.visNode == null) || this._visNode.deaf);
        }
        set
        {
            if (this.visNode != null)
            {
                this._visNode.deaf = value;
            }
            else if (!value)
            {
                Debug.LogError("no visnode", this);
            }
        }
    }

    public Angle2 eyesAngles
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesAngles;
        }
        set
        {
            if (!this.lockLook)
            {
                if (!this._eyesSetup)
                {
                    this.EyesSetup();
                }
                if ((this._eyesAngles.x != value.x) || (this._eyesAngles.y != value.y))
                {
                    this._eyesAngles = value;
                    this.InvalidateEyesAngles();
                }
            }
        }
    }

    public Vector3 eyesOffset
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesOffset;
        }
        set
        {
            if (!this.lockLook)
            {
                if (!this._eyesSetup)
                {
                    this.EyesSetup();
                }
                if (this._eyesOffset != value)
                {
                    this._eyesOffset = value;
                    this.InvalidateEyesOffset();
                }
            }
        }
    }

    public Vector3 eyesOrigin
    {
        get
        {
            return this._eyesTransform.position;
        }
    }

    public Vector3 eyesOriginAtInitialOffset
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return base.transform.TransformPoint(this._initialEyesOffset);
        }
    }

    public float eyesPitch
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesAngles.pitch;
        }
        set
        {
            if (!this.lockLook)
            {
                if (!this._eyesSetup)
                {
                    this.EyesSetup();
                }
                if (this._eyesAngles.pitch != value)
                {
                    this._eyesAngles.pitch = value;
                    this.InvalidateEyesAngles();
                }
            }
        }
    }

    public Ray eyesRay
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return new Ray(this._eyesTransform.position, this._eyesTransform.forward);
        }
    }

    public Quaternion eyesRotation
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesAngles.quat;
        }
        set
        {
            this.rotation = value;
            Quaternion rotation = Quaternion.Euler(0f, this._eyesAngles.yaw, 0f);
            Vector3 from = (Vector3) ((value * Quaternion.Inverse(rotation)) * Vector3.forward);
            from.Normalize();
            if (from.y < 0f)
            {
                this.eyesPitch = -Vector3.Angle(from, Vector3.forward);
            }
            else
            {
                this.eyesPitch = Vector3.Angle(from, Vector3.forward);
            }
        }
    }

    public Transform eyesTransformReadOnly
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesTransform;
        }
    }

    public float eyesYaw
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return this._eyesAngles.yaw;
        }
        set
        {
            if (!this.lockLook)
            {
                if (!this._eyesSetup)
                {
                    this.EyesSetup();
                }
                if (this._eyesAngles.yaw != value)
                {
                    this._eyesAngles.yaw = value;
                    this.InvalidateEyesAngles();
                }
            }
        }
    }

    public Vector3 forward
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return (Vector3) (Quaternion.Euler(0f, this._eyesAngles.yaw, 0f) * Vector3.forward);
        }
    }

    public float health
    {
        get
        {
            return ((this.takeDamage == null) ? float.PositiveInfinity : this._takeDamage.health);
        }
    }

    public float healthFraction
    {
        get
        {
            return ((this.takeDamage == null) ? 1f : this._takeDamage.healthFraction);
        }
    }

    public float healthLoss
    {
        get
        {
            return ((this.takeDamage == null) ? 0f : this._takeDamage.healthLoss);
        }
    }

    public float healthLossFraction
    {
        get
        {
            return ((this.takeDamage == null) ? 0f : this._takeDamage.healthLossFraction);
        }
    }

    public HitBoxSystem hitBoxSystem
    {
        get
        {
            if (!this.didHitBoxSystemTest)
            {
                SeekIDRemoteComponentInChildren<Character, HitBoxSystem>(this, ref this._hitBoxSystem);
                this.didHitBoxSystemTest = true;
            }
            return this._hitBoxSystem;
        }
    }

    public bool? idle
    {
        get
        {
            if (this.idleControl != null)
            {
                return new bool?((bool) this._idleControl);
            }
            return null;
        }
    }

    public IDLocalCharacterIdleControl idleControl
    {
        get
        {
            if (!this.didIdleControlTest)
            {
                SeekIDLocalComponentInChildren<Character, IDLocalCharacterIdleControl>(this, ref this._idleControl);
                this.didIdleControlTest = true;
            }
            return this._idleControl;
        }
    }

    public Vector3 initialEyesOffset
    {
        get
        {
            return this._initialEyesOffset;
        }
    }

    public float initialEyesOffsetX
    {
        get
        {
            return this._initialEyesOffset.x;
        }
    }

    public float initialEyesOffsetY
    {
        get
        {
            return this._initialEyesOffset.y;
        }
    }

    public float initialEyesOffsetZ
    {
        get
        {
            return this._initialEyesOffset.z;
        }
    }

    public CharacterInterpolatorBase interpolator
    {
        get
        {
            return this._interpolator;
        }
    }

    public bool localAIControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.localAIControlled);
        }
    }

    public Controllable localAIControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.localAIControlled) ? null : this._controllable);
        }
    }

    public Controller localAIControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.localAIControlledController);
        }
    }

    public bool localControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.localControlled);
        }
    }

    public bool localPlayerControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.localPlayerControlled);
        }
    }

    public Controllable localPlayerControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.localPlayerControlled) ? null : this._controllable);
        }
    }

    public Controller localPlayerControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.localPlayerControlledController);
        }
    }

    public Character masterCharacter
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.masterCharacter);
        }
    }

    public Controllable masterControllable
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.masterControllable);
        }
    }

    public Controller masterController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.masterController);
        }
    }

    public float maxHealth
    {
        get
        {
            return ((this.takeDamage == null) ? float.PositiveInfinity : this._takeDamage.maxHealth);
        }
    }

    public float maxPitch
    {
        get
        {
            return this._maxPitch;
        }
    }

    public float minPitch
    {
        get
        {
            return this._minPitch;
        }
    }

    public bool mute
    {
        get
        {
            return ((this.visNode == null) || this._visNode.mute);
        }
        set
        {
            if (this.visNode != null)
            {
                this._visNode.mute = value;
            }
            else if (!value)
            {
                Debug.LogError("no visnode", this);
            }
        }
    }

    public Character nextCharacter
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.nextCharacter);
        }
    }

    public Controllable nextControllable
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.nextControllable);
        }
    }

    public Controller nextController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.nextController);
        }
    }

    public string npcName
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.npcName);
        }
    }

    public Vector3 origin
    {
        get
        {
            return base.transform.localPosition;
        }
        set
        {
            if (!this.lockMovement)
            {
                base.transform.localPosition = value;
            }
        }
    }

    public IDLocalCharacterAddon overlay
    {
        get
        {
            return this._overlay;
        }
    }

    public bool overridingControl
    {
        get
        {
            return ((this.controllable != null) && this._controllable.overridingControl);
        }
    }

    public PlayerClient playerClient
    {
        get
        {
            return ((this._controllable == null) ? null : this._controllable.playerClient);
        }
    }

    public bool playerControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.playerControlled);
        }
    }

    public Controllable playerControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.playerControlled) ? null : this._controllable);
        }
    }

    public Controller playerControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.playerControlledController);
        }
    }

    public static IEnumerable<Character> PlayerCurrentCharacters
    {
        get
        {
            return new <>c__Iterator1A { $PC = -2 };
        }
    }

    public static IEnumerable<Character> PlayerRootCharacters
    {
        get
        {
            return new <>c__Iterator19 { $PC = -2 };
        }
    }

    public Character previousCharacter
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.previousCharacter);
        }
    }

    public Controllable previousControllable
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.previousControllable);
        }
    }

    public Controller previousController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.previousController);
        }
    }

    public RecoilSimulation recoilSimulation
    {
        get
        {
            if (!this.didRecoilSimulationTest)
            {
                SeekIDLocalComponentInChildren<Character, RecoilSimulation>(this, ref this._recoilSimulation);
                this.didRecoilSimulationTest = true;
            }
            return this._recoilSimulation;
        }
    }

    public bool remoteAIControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.remoteAIControlled);
        }
    }

    public Controllable remoteAIControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.remoteAIControlled) ? null : this._controllable);
        }
    }

    public Controller remoteAIControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.remoteAIControlledController);
        }
    }

    public bool remoteControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.remoteControlled);
        }
    }

    public bool remotePlayerControlled
    {
        get
        {
            return ((this.controllable != null) && this._controllable.remotePlayerControlled);
        }
    }

    public Controllable remotePlayerControlledControllable
    {
        get
        {
            return (((this.controllable == null) || !this._controllable.remotePlayerControlled) ? null : this._controllable);
        }
    }

    public Controller remotePlayerControlledController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.remotePlayerControlledController);
        }
    }

    public Vector3 right
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return (Vector3) (Quaternion.Euler(0f, this._eyesAngles.yaw, 0f) * Vector3.right);
        }
    }

    public Character rootCharacter
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.rootCharacter);
        }
    }

    public Controllable rootControllable
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.rootControllable);
        }
    }

    public Controller rootController
    {
        get
        {
            return ((this.controllable == null) ? null : this._controllable.rootController);
        }
    }

    public Quaternion rotation
    {
        get
        {
            if (!this._eyesSetup)
            {
                this.EyesSetup();
            }
            return Quaternion.Euler(0f, this._eyesAngles.yaw, 0f);
        }
        set
        {
            Vector2 vector2;
            Vector3 vector = (Vector3) (value * Vector3.forward);
            vector2.x = vector.x;
            vector2.y = vector.z;
            if (Mathf.Approximately(vector2.x, 0f) && Mathf.Approximately(vector2.y, 0f))
            {
                vector = (Vector3) (value * Vector3.right);
                vector2.x = -vector.z;
                vector2.y = vector.x;
                if (Mathf.Approximately(vector2.x, 0f) && Mathf.Approximately(vector2.y, 0f))
                {
                    return;
                }
            }
            this.eyesYaw = Mathf.Atan2(-vector2.x, vector2.y) * -57.29578f;
        }
    }

    public bool signaledDeath
    {
        get
        {
            return this._signaledDeath;
        }
    }

    public TakeDamage takeDamage
    {
        get
        {
            if (!this.didTakeDamageTest)
            {
                SeekIDLocalComponentInChildren<Character, TakeDamage>(this, ref this._takeDamage);
                this.didTakeDamageTest = true;
            }
            return this._takeDamage;
        }
    }

    private CharacterTraitMap traitMap
    {
        get
        {
            if (!this._attemptedTraitMapLoad)
            {
                this.LoadTraitMap();
            }
            return this._traitMap;
        }
    }

    private bool traitMapLoaded
    {
        get
        {
            if (!this._attemptedTraitMapLoad)
            {
                this.LoadTraitMap();
            }
            return this._traitMapLoaded;
        }
    }

    public Vis.Mask traitMask
    {
        get
        {
            if (this.visNode != null)
            {
                return this._visNode.traitMask;
            }
            return new Vis.Mask();
        }
        set
        {
            if (this.visNode != null)
            {
                this._visNode.traitMask = value;
            }
            else if (value.data != 0)
            {
                Debug.Log("no visnode", this);
            }
        }
    }

    public Vector3 up
    {
        get
        {
            return Vector3.up;
        }
    }

    public bool vessel
    {
        get
        {
            return ((this.controllable != null) && this._controllable.vessel);
        }
    }

    public Vis.Mask viewMask
    {
        get
        {
            if (this.visNode != null)
            {
                return this._visNode.viewMask;
            }
            return new Vis.Mask();
        }
        set
        {
            if (this.visNode != null)
            {
                this._visNode.viewMask = value;
            }
            else if (value.data != 0)
            {
                Debug.Log("no visnode", this);
            }
        }
    }

    public VisNode visNode
    {
        get
        {
            if (!this.didVisNodeTest)
            {
                SeekIDLocalComponentInChildren<Character, VisNode>(this, ref this._visNode);
                this.didVisNodeTest = true;
            }
            return this._visNode;
        }
    }

    [CompilerGenerated]
    private sealed class <>c__Iterator19 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Character>, IEnumerator<Character>
    {
        internal Character $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_182>__0;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;

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
                        this.<$s_182>__0.Dispose();
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
                    this.<$s_182>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C9;
            }
            try
            {
                while (this.<$s_182>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_182>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2.idMain;
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
                this.<$s_182>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Character> IEnumerable<Character>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<>c__Iterator19();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Character>.GetEnumerator();
        }

        Character IEnumerator<Character>.Current
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
    private sealed class <>c__Iterator1A : IDisposable, IEnumerator, IEnumerable, IEnumerable<Character>, IEnumerator<Character>
    {
        internal Character $current;
        internal int $PC;
        internal List<PlayerClient>.Enumerator <$s_183>__0;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;

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
                        this.<$s_183>__0.Dispose();
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
                    this.<$s_183>__0 = PlayerClient.All.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00C9;
            }
            try
            {
                while (this.<$s_183>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_183>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2.idMain;
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
                this.<$s_183>__0.Dispose();
            }
            this.$PC = -1;
        Label_00C9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Character> IEnumerable<Character>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<>c__Iterator1A();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Character>.GetEnumerator();
        }

        Character IEnumerator<Character>.Current
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
    private sealed class <CurrentCharacters>c__Iterator1C : IDisposable, IEnumerator, IEnumerable, IEnumerable<Character>, IEnumerator<Character>
    {
        internal Character $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_189>__0;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_189>__0 == null)
                        {
                        }
                        this.<$s_189>__0.Dispose();
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
                    this.<$s_189>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00CE;
            }
            try
            {
                while (this.<$s_189>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_189>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2.idMain;
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
                if (this.<$s_189>__0 == null)
                {
                }
                this.<$s_189>__0.Dispose();
            }
            this.$PC = -1;
        Label_00CE:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Character> IEnumerable<Character>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<CurrentCharacters>c__Iterator1C { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Character>.GetEnumerator();
        }

        Character IEnumerator<Character>.Current
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
    private sealed class <CurrentCharacters>c__Iterator1E<TCharacter> : IDisposable, IEnumerator, IEnumerable, IEnumerable<TCharacter>, IEnumerator<TCharacter> where TCharacter: Character
    {
        internal TCharacter $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_191>__0;
        internal TCharacter <character>__3;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_191>__0 == null)
                        {
                        }
                        this.<$s_191>__0.Dispose();
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
                    this.<$s_191>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00F9;
            }
            try
            {
                while (this.<$s_191>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_191>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.controllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<character>__3 = this.<controllable>__2.idMain as TCharacter;
                        if (this.<character>__3 != null)
                        {
                            this.$current = this.<character>__3;
                            this.$PC = 1;
                            flag = true;
                            return true;
                        }
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_191>__0 == null)
                {
                }
                this.<$s_191>__0.Dispose();
            }
            this.$PC = -1;
        Label_00F9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<TCharacter> IEnumerable<TCharacter>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<CurrentCharacters>c__Iterator1E<TCharacter> { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<TCharacter>.GetEnumerator();
        }

        TCharacter IEnumerator<TCharacter>.Current
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
    private sealed class <RootCharacters>c__Iterator1B : IDisposable, IEnumerator, IEnumerable, IEnumerable<Character>, IEnumerator<Character>
    {
        internal Character $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_188>__0;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_188>__0 == null)
                        {
                        }
                        this.<$s_188>__0.Dispose();
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
                    this.<$s_188>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00CE;
            }
            try
            {
                while (this.<$s_188>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_188>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.$current = this.<controllable>__2.idMain;
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
                if (this.<$s_188>__0 == null)
                {
                }
                this.<$s_188>__0.Dispose();
            }
            this.$PC = -1;
        Label_00CE:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Character> IEnumerable<Character>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<RootCharacters>c__Iterator1B { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Character>.GetEnumerator();
        }

        Character IEnumerator<Character>.Current
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
    private sealed class <RootCharacters>c__Iterator1D<TCharacter> : IDisposable, IEnumerator, IEnumerable, IEnumerable<TCharacter>, IEnumerator<TCharacter> where TCharacter: Character
    {
        internal TCharacter $current;
        internal int $PC;
        internal IEnumerable<PlayerClient> <$>playerClients;
        internal IEnumerator<PlayerClient> <$s_190>__0;
        internal TCharacter <character>__3;
        internal Controllable <controllable>__2;
        internal PlayerClient <pc>__1;
        internal IEnumerable<PlayerClient> playerClients;

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
                        if (this.<$s_190>__0 == null)
                        {
                        }
                        this.<$s_190>__0.Dispose();
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
                    this.<$s_190>__0 = this.playerClients.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00F9;
            }
            try
            {
                while (this.<$s_190>__0.MoveNext())
                {
                    this.<pc>__1 = this.<$s_190>__0.Current;
                    this.<controllable>__2 = this.<pc>__1.rootControllable;
                    if (this.<controllable>__2 != null)
                    {
                        this.<character>__3 = this.<controllable>__2.idMain as TCharacter;
                        if (this.<character>__3 != null)
                        {
                            this.$current = this.<character>__3;
                            this.$PC = 1;
                            flag = true;
                            return true;
                        }
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_190>__0 == null)
                {
                }
                this.<$s_190>__0.Dispose();
            }
            this.$PC = -1;
        Label_00F9:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<TCharacter> IEnumerable<TCharacter>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Character.<RootCharacters>c__Iterator1D<TCharacter> { playerClients = this.<$>playerClients };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<TCharacter>.GetEnumerator();
        }

        TCharacter IEnumerator<TCharacter>.Current
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

    private static class AddonRegistry
    {
        private static readonly Dictionary<Type, bool> validatedCache = new Dictionary<Type, bool>();

        public static bool Validate(Type type)
        {
            bool flag;
            if (type == null)
            {
                return false;
            }
            if (!validatedCache.TryGetValue(type, out flag))
            {
                if (!typeof(IDLocalCharacterAddon).IsAssignableFrom(type))
                {
                    Debug.LogError(string.Format("Type {0} is not a valid IDLocalCharacterAddon type", type));
                }
                else if (type.IsAbstract)
                {
                    Debug.LogError(string.Format("Type {0} is abstract, thus not a valid IDLocalCharacterAddon type", type));
                }
                else if (Attribute.IsDefined(type, typeof(RequireComponent), false))
                {
                    Debug.LogWarning(string.Format("Type {0} uses the RequireComponent attribute which could be dangerous with addons", type));
                    flag = true;
                }
                else
                {
                    flag = true;
                }
                validatedCache[type] = flag;
            }
            return flag;
        }
    }

    private static class AddonRegistry<T> where T: IDLocalCharacterAddon, new()
    {
        public static readonly bool valid;

        static AddonRegistry()
        {
            Character.AddonRegistry<T>.valid = Character.AddonRegistry.Validate(typeof(T));
        }
    }

    private static class AddonStringRegistry
    {
        private static readonly string[] assemblyStrings = new string[] { ", Assembly-CSharp-firstpass", ", Assembly-CSharp" };
        private static readonly Dictionary<string, TypePair> validatedCache = new Dictionary<string, TypePair>();

        public static bool Validate(string typeName)
        {
            Type type;
            return Validate(typeName, out type);
        }

        public static bool Validate<TBase>(string typeName)
        {
            Type type;
            return (Validate(typeName, out type) && typeof(TBase).IsAssignableFrom(type));
        }

        private static bool Validate(string typeName, out Type type)
        {
            TypePair pair;
            if (string.IsNullOrEmpty(typeName))
            {
                type = null;
                return false;
            }
            if (validatedCache.TryGetValue(typeName, out pair))
            {
                type = pair.type;
                return pair.valid;
            }
            bool valid = TypeUtility.TryParse(typeName, out type) && Character.AddonRegistry.Validate(type);
            if (!valid)
            {
                foreach (string str in assemblyStrings)
                {
                    if (TypeUtility.TryParse(typeName + str, out type) && Character.AddonRegistry.Validate(type))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    type = null;
                    Debug.LogError(string.Format("Couldnt associate string \"{0}\" with any valid addon type", typeName));
                }
            }
            validatedCache[typeName] = new TypePair(type, valid);
            return valid;
        }

        public static bool Validate(string typeName, Type minimumType)
        {
            Type type;
            return (Validate(typeName, out type) && minimumType.IsAssignableFrom(type));
        }

        public static bool Validate<TBase>(string typeName, out Type type)
        {
            return (Validate(typeName, out type) && typeof(TBase).IsAssignableFrom(type));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypePair
        {
            public readonly Type type;
            public readonly bool valid;
            public TypePair(Type type, bool valid)
            {
                this.type = type;
                this.valid = valid;
            }
        }
    }
}

