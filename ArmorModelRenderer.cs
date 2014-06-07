using Facepunch.Actor;
using System;
using System.Reflection;
using UnityEngine;

public sealed class ArmorModelRenderer : IDLocalCharacter
{
    [NonSerialized]
    private CharacterArmorTrait armorTrait;
    [NonSerialized]
    private bool awake;
    [SerializeField, PrefetchComponent]
    private BoneStructure boneStructure;
    private static bool censored = true;
    [NonSerialized]
    private ArmorModelMemberMap models;
    [PrefetchChildComponent, SerializeField]
    private SkinnedMeshRenderer originalRenderer;
    private static bool rebindingCensorship;
    [NonSerialized]
    private ArmorModelMemberMap<ActorMeshRenderer> renderers;

    public ArmorModelSlotMask BindArmorGroup(ArmorModelGroup group)
    {
        ArmorModelSlotMask mask = 0;
        if (group != null)
        {
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                ArmorModel model = group[slot];
                if ((model != null) && this.BindArmorModelCheckedNonNull(model))
                {
                    mask |= slot.ToMask();
                }
            }
        }
        return mask;
    }

    public ArmorModelSlotMask BindArmorGroup(ArmorModelGroup group, ArmorModelSlotMask slotMask)
    {
        if (!this.awake)
        {
            if (group == null)
            {
                return 0;
            }
            return this.Initialize(group.armorModelMemberMap, slotMask);
        }
        ArmorModelSlotMask mask = 0;
        foreach (ArmorModelSlot slot in slotMask.EnumerateSlots())
        {
            ArmorModel model = group[slot];
            if ((model != null) && this.BindArmorModelCheckedNonNull(model))
            {
                mask |= slot.ToMask();
            }
        }
        return mask;
    }

    private bool BindArmorModel<TArmorModel>(TArmorModel model) where TArmorModel: ArmorModel, new()
    {
        if (model != null)
        {
            return this.BindArmorModelCheckedNonNull(model);
        }
        ArmorModel model2 = this.defaultArmorModelGroup[ArmorModelSlotUtility.GetArmorModelSlotForClass<TArmorModel>()];
        return ((model2 != null) && this.BindArmorModelCheckedNonNull(model2));
    }

    private bool BindArmorModel(ArmorModel model, ArmorModelSlot slot)
    {
        if (model == null)
        {
            ArmorModel model2 = this.defaultArmorModelGroup[slot];
            return ((model2 != null) && this.BindArmorModelCheckedNonNull(model2));
        }
        if (model.slot != slot)
        {
            Debug.LogError("model.slot != " + slot, model);
            return false;
        }
        return this.BindArmorModelCheckedNonNull(model);
    }

    private bool BindArmorModelCheckedNonNull(ArmorModel model)
    {
        ArmorModel censoredModel;
        ArmorModelSlot slot = model.slot;
        if (!rebindingCensorship)
        {
            ArmorModel model2 = this.models[slot];
            if (model2 == model)
            {
                return false;
            }
        }
        ActorMeshRenderer renderer = this.renderers[slot];
        if (censored)
        {
            censoredModel = model.censoredModel;
            if (censoredModel == null)
            {
                censoredModel = model;
            }
        }
        else
        {
            censoredModel = model;
        }
        if (renderer.actorRig != censoredModel.actorRig)
        {
            return false;
        }
        if (!base.enabled)
        {
            renderer.renderer.enabled = true;
        }
        renderer.Bind(censoredModel.actorMeshInfo, censoredModel.sharedMaterials);
        if (!base.enabled)
        {
            renderer.renderer.enabled = false;
        }
        this.models[slot] = model;
        return true;
    }

    public ArmorModelSlotMask BindArmorModels(ArmorModelMemberMap map)
    {
        if (!this.awake)
        {
            return this.Initialize(map, ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet);
        }
        ArmorModelSlotMask mask = 0;
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            if (this.BindArmorModel(map[slot], slot))
            {
                mask |= slot.ToMask();
            }
        }
        return mask;
    }

    public ArmorModelSlotMask BindArmorModels(ArmorModelMemberMap map, ArmorModelSlotMask slotMask)
    {
        if (!this.awake)
        {
            return this.Initialize(map, slotMask);
        }
        ArmorModelSlotMask mask = 0;
        foreach (ArmorModelSlot slot in slotMask.EnumerateSlots())
        {
            if (this.BindArmorModel(map[slot], slot))
            {
                mask |= slot.ToMask();
            }
        }
        return mask;
    }

    public ArmorModelSlotMask BindDefaultArmorGroup()
    {
        if (this.defaultArmorModelGroup != null)
        {
            return this.BindArmorGroup(this.defaultArmorModelGroup);
        }
        return 0;
    }

    public bool Contains(ArmorModel model)
    {
        if (model == null)
        {
            return false;
        }
        if (!this.awake)
        {
            ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
            return ((defaultArmorModelGroup != null) && (defaultArmorModelGroup[model.slot] == model));
        }
        return (this.models[model.slot] == model);
    }

    public bool Contains<TArmorModel>(TArmorModel model) where TArmorModel: ArmorModel, new()
    {
        if (model == null)
        {
            return false;
        }
        if (!this.awake)
        {
            ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
            return ((defaultArmorModelGroup != null) && (defaultArmorModelGroup.GetArmorModel<TArmorModel>() == model));
        }
        return (this.models.GetArmorModel<TArmorModel>() == model);
    }

    public T GetArmorModel<T>() where T: ArmorModel, new()
    {
        if (this.awake)
        {
            return this.models.GetArmorModel<T>();
        }
        ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
        if (defaultArmorModelGroup != null)
        {
            return defaultArmorModelGroup.GetArmorModel<T>();
        }
        return null;
    }

    public ArmorModelMemberMap GetArmorModelMemberMapCopy()
    {
        if (this.awake)
        {
            return this.models;
        }
        ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
        if (defaultArmorModelGroup == null)
        {
            return new ArmorModelMemberMap();
        }
        return defaultArmorModelGroup.armorModelMemberMap;
    }

    private ArmorModelSlotMask Initialize(ArmorModelMemberMap memberMap, ArmorModelSlotMask memberMask)
    {
        ActorMeshRenderer renderer;
        this.awake = true;
        string rendererName = ArmorModelSlot.Head.GetRendererName();
        ActorRig actorRig = this.defaultArmorModelGroup[ArmorModelSlot.Head].actorRig;
        if (this.originalRenderer != null)
        {
            renderer = ActorMeshRenderer.Replace(this.originalRenderer, actorRig, this.boneStructure.rigOrderedTransformArray, rendererName);
        }
        else
        {
            renderer = ActorMeshRenderer.CreateOn(base.transform, rendererName, actorRig, this.boneStructure.rigOrderedTransformArray, base.gameObject.layer);
        }
        this.renderers[ArmorModelSlot.Head] = renderer;
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ArmorModelSlot.Head; slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            this.renderers[slot] = renderer.CloneBlank(slot.GetRendererName());
        }
        for (ArmorModelSlot slot2 = ArmorModelSlot.Feet; slot2 < ArmorModelSlot.Head; slot2 = (ArmorModelSlot) (((int) slot2) + 1))
        {
            ActorMeshRenderer renderer2 = this.renderers[slot2];
            if (renderer2 != null)
            {
                renderer2.renderer.enabled = base.enabled;
            }
        }
        ArmorModelSlotMask mask = 0;
        ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
        if (defaultArmorModelGroup != null)
        {
            for (ArmorModelSlot slot3 = ArmorModelSlot.Feet; slot3 < ((ArmorModelSlot) 4); slot3 = (ArmorModelSlot) (((int) slot3) + 1))
            {
                if (memberMask.Contains(slot3))
                {
                    ArmorModel armorModel = memberMap.GetArmorModel(slot3);
                    if ((armorModel != null) && this.BindArmorModelCheckedNonNull(armorModel))
                    {
                        mask |= slot3.ToMask();
                        continue;
                    }
                }
                ArmorModel model2 = defaultArmorModelGroup[slot3];
                if (model2 != null)
                {
                    this.BindArmorModelCheckedNonNull(model2);
                }
            }
            return mask;
        }
        foreach (ArmorModelSlot slot4 in memberMask.EnumerateSlots())
        {
            ArmorModel model = memberMap.GetArmorModel(slot4);
            if ((model != null) && this.BindArmorModelCheckedNonNull(model))
            {
                mask |= slot4.ToMask();
            }
        }
        return mask;
    }

    private void OnDestroy()
    {
        if (!this.awake)
        {
            this.awake = true;
        }
        else
        {
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                ActorMeshRenderer renderer = this.renderers[slot];
                if (renderer != null)
                {
                    Object.Destroy(renderer.gameObject);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (this.awake)
        {
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                ActorMeshRenderer renderer = this.renderers[slot];
                if (renderer != null)
                {
                    renderer.renderer.enabled = false;
                }
            }
        }
        else if (this.originalRenderer != null)
        {
            this.originalRenderer.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (this.awake)
        {
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                ActorMeshRenderer renderer = this.renderers[slot];
                if (renderer != null)
                {
                    renderer.renderer.enabled = true;
                }
            }
        }
        else if (this.originalRenderer != null)
        {
            this.originalRenderer.enabled = true;
        }
    }

    private void Start()
    {
        if (!this.awake)
        {
            this.Initialize(new ArmorModelMemberMap(), 0);
        }
    }

    public ActorRig actorRig
    {
        get
        {
            return this.boneStructure.actorRig;
        }
    }

    public static bool Censored
    {
        get
        {
            return censored;
        }
        set
        {
            if (censored != value)
            {
                censored = value;
                try
                {
                    rebindingCensorship = true;
                    foreach (Object obj2 in Object.FindObjectsOfType(typeof(ArmorModelRenderer)))
                    {
                        ArmorModelRenderer renderer = (ArmorModelRenderer) obj2;
                        if (renderer != null)
                        {
                            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
                            {
                                ArmorModel model = renderer[slot];
                                if ((model != null) && model.hasCensoredModel)
                                {
                                    if (!renderer.awake)
                                    {
                                        ArmorModelMemberMap memberMap = new ArmorModelMemberMap();
                                        renderer.Initialize(memberMap, 0);
                                        break;
                                    }
                                    renderer.BindArmorModelCheckedNonNull(model);
                                }
                            }
                        }
                    }
                    SleepingAvatar.RebindAllRenderers();
                }
                finally
                {
                    rebindingCensorship = false;
                }
            }
        }
    }

    public ArmorModelGroup defaultArmorModelGroup
    {
        get
        {
            return ((this.armorTrait == null) ? ((ArmorModelGroup) (this.armorTrait = base.GetTrait<CharacterArmorTrait>())) : ((ArmorModelGroup) this.armorTrait)).defaultGroup;
        }
    }

    public ArmorModel this[ArmorModelSlot slot]
    {
        get
        {
            if (this.awake)
            {
                return this.models[slot];
            }
            ArmorModelGroup defaultArmorModelGroup = this.defaultArmorModelGroup;
            if (defaultArmorModelGroup != null)
            {
                return defaultArmorModelGroup[slot];
            }
            return null;
        }
    }
}

