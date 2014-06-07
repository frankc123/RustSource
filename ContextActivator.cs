using Facepunch;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class ContextActivator : MonoBehaviour, IContextRequestable, IContextRequestableQuick, IContextRequestableStatus, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    [SerializeField]
    private ActivationMode activationMode;
    [SerializeField]
    private SpriteQuickMode defaultSprite;
    [SerializeField]
    private string defaultText;
    [SerializeField]
    private Vector3 defaultTextPoint;
    [SerializeField]
    private Activatable[] extraActions;
    [SerializeField]
    private bool isSwitch;
    private bool isToggle;
    [SerializeField]
    private Activatable mainAction;
    [SerializeField]
    private SpriteQuickMode offSprite;
    [SerializeField]
    private string offText;
    [SerializeField]
    private Vector3 offTextPoint;
    [SerializeField]
    private SpriteQuickMode onSprite;
    [SerializeField]
    private string onText;
    [SerializeField]
    private Vector3 onTextPoint;
    [SerializeField]
    private bool useSpriteTextPoint;
    [SerializeField]
    private bool useTextPoint;

    private ActivationResult ApplyActivatable(Activatable activatable, Character instigator, ulong timestamp, bool extra)
    {
        if (activatable != null)
        {
            switch (this.activationMode)
            {
                case ActivationMode.TurnOn:
                    return activatable.Activate(true, instigator, timestamp);

                case ActivationMode.TurnOff:
                    return activatable.Activate(false, instigator, timestamp);
            }
            return activatable.Activate(instigator, timestamp);
        }
        return ActivationResult.Error_Destroyed;
    }

    bool IContextRequestablePointText.ContextTextPoint(out Vector3 worldPoint)
    {
        if (!this.useTextPoint)
        {
            worldPoint = new Vector3();
            return false;
        }
        if (!this.useSpriteTextPoint || !ContextRequestable.PointUtil.SpriteOrOrigin(this, out worldPoint))
        {
            if (!this.isSwitch)
            {
                worldPoint = this.defaultTextPoint;
            }
            else
            {
                ActivationToggleState toggleState = this.toggleState;
                if (toggleState == ActivationToggleState.On)
                {
                    worldPoint = this.onTextPoint;
                }
                else if (toggleState == ActivationToggleState.Off)
                {
                    worldPoint = this.offTextPoint;
                }
                else
                {
                    worldPoint = this.defaultTextPoint;
                }
            }
            worldPoint = base.transform.TransformPoint(worldPoint);
        }
        return true;
    }

    ContextStatusFlags IContextRequestableStatus.ContextStatusPoll()
    {
        SpriteQuickMode defaultSprite;
        if (!this.isSwitch)
        {
            defaultSprite = this.defaultSprite;
        }
        else
        {
            ActivationToggleState toggleState = this.toggleState;
            if (toggleState == ActivationToggleState.On)
            {
                defaultSprite = this.onSprite;
            }
            else if (toggleState == ActivationToggleState.Off)
            {
                defaultSprite = this.offSprite;
            }
            else
            {
                defaultSprite = this.defaultSprite;
            }
        }
        switch (defaultSprite)
        {
            case SpriteQuickMode.Faded:
                return ContextStatusFlags.SpriteFlag0;

            case SpriteQuickMode.AlwaysVisible:
                return (ContextStatusFlags.SpriteFlag1 | ContextStatusFlags.SpriteFlag0);

            case SpriteQuickMode.NeverVisisble:
                return ContextStatusFlags.SpriteFlag1;
        }
        return 0;
    }

    string IContextRequestableText.ContextText(Controllable localControllable)
    {
        if (this.isSwitch)
        {
            switch (this.toggleState)
            {
                case ActivationToggleState.On:
                    return this.onText;

                case ActivationToggleState.Off:
                    return this.offText;
            }
        }
        return this.defaultText;
    }

    private ActivationToggleState toggleState
    {
        get
        {
            if (this.mainAction == null)
            {
                return ActivationToggleState.Unspecified;
            }
            return this.mainAction.toggleState;
        }
    }

    private enum ActivationMode
    {
        ActivateOrToggle,
        TurnOn,
        TurnOff
    }

    private enum SpriteQuickMode
    {
        Default,
        Faded,
        AlwaysVisible,
        NeverVisisble
    }
}

