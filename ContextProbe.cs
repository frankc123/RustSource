using Facepunch;
using Facepunch.Cursor;
using System;
using UnityEngine;

public sealed class ContextProbe : IDLocalCharacterAddon
{
    private const IDLocalCharacterAddon.AddonFlags ContextProbeAddonFlags = (IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnWillRemoveAddon);
    [NonSerialized]
    private bool hasHighlight;
    [NonSerialized]
    private MonoBehaviour lastUseHighlight;
    [NonSerialized]
    private float raycastLength;
    private static ContextProbe singleton;

    public ContextProbe() : this(IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake | IDLocalCharacterAddon.AddonFlags.FireOnWillRemoveAddon)
    {
    }

    private ContextProbe(IDLocalCharacterAddon.AddonFlags addonFlags) : base(addonFlags)
    {
    }

    private bool ClientCheckUse(Ray ray, bool press)
    {
        RaycastHit hit;
        MonoBehaviour implementor;
        if (Physics.Raycast(ray, out hit, this.raycastLength, -201523205))
        {
            MonoBehaviour behaviour2;
            NetEntityID yid;
            NetEntityID.Kind kind;
            Transform component = hit.transform;
            for (Transform transform2 = component.parent; (((int) (kind = NetEntityID.Of(component, out yid, out behaviour2))) == 0) && (transform2 != null); transform2 = component.parent)
            {
                component = transform2;
            }
            if (((int) kind) == 0)
            {
                implementor = null;
            }
            else
            {
                Contextual contextual;
                if (Contextual.ContextOf(behaviour2, out contextual))
                {
                    implementor = contextual.implementor;
                    if (press)
                    {
                        Context.BeginQuery(contextual);
                    }
                }
                else
                {
                    implementor = null;
                }
            }
        }
        else
        {
            implementor = null;
        }
        if (implementor != this.lastUseHighlight)
        {
            this.lastUseHighlight = implementor;
            if (implementor != null)
            {
                IContextRequestableText text = implementor as IContextRequestableText;
                if (text != null)
                {
                    RPOS.UseHoverTextSet(base.controllable, text);
                }
                else
                {
                    RPOS.UseHoverTextSet(implementor.name);
                }
            }
            else
            {
                RPOS.UseHoverTextClear();
            }
        }
        return (bool) implementor;
    }

    protected override void OnAddonAwake()
    {
        singleton = this;
        CharacterContextProbeTrait trait = base.GetTrait<CharacterContextProbeTrait>();
        this.raycastLength = trait.rayLength;
    }

    protected override void OnWillRemoveAddon()
    {
        if (singleton == this)
        {
            singleton = null;
        }
        this.hasHighlight = false;
        this.lastUseHighlight = null;
    }

    private void Update()
    {
        if (!base.dead)
        {
            bool buttonDown;
            bool buttonUp;
            if (LockCursorManager.IsLocked())
            {
                buttonDown = Context.ButtonDown;
                buttonUp = Context.ButtonUp;
            }
            else
            {
                buttonDown = false;
                buttonUp = Context.WorkingInMenu && Context.ButtonUp;
            }
            this.hasHighlight = this.ClientCheckUse(base.eyesRay, buttonDown);
            if (Context.ButtonUp)
            {
                Context.EndQuery();
            }
        }
    }

    public static bool aProbeIsHighlighting
    {
        get
        {
            return ((singleton != null) && singleton.hasHighlight);
        }
    }

    public bool isHighlighting
    {
        get
        {
            return this.hasHighlight;
        }
    }
}

