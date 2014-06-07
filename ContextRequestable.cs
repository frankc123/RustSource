using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ContextRequestable
{
    public const ContextExecution AllExecutionFlags = (ContextExecution.Menu | ContextExecution.Quick);

    private static bool UseableForwardFromContext(IContextRequestable requestable, Controllable controllable)
    {
        return UseableForwardFromContext(requestable, controllable, null);
    }

    public static bool UseableForwardFromContext(IContextRequestable requestable, Controllable controllable, Useable useable)
    {
        MonoBehaviour behaviour = requestable as MonoBehaviour;
        if (useable == null)
        {
            useable = behaviour.GetComponent<Useable>();
        }
        Character idMain = controllable.idMain;
        return (((idMain != null) && (useable != null)) && useable.EnterFromContext(idMain).Succeeded());
    }

    public static ContextResponse UseableForwardFromContextRespond(IContextRequestable requestable, Controllable controllable)
    {
        return (!UseableForwardFromContext(requestable, controllable, null) ? ContextResponse.FailBreak : ContextResponse.DoneBreak);
    }

    public static ContextResponse UseableForwardFromContextRespond(IContextRequestable requestable, Controllable controllable, Useable useable)
    {
        return (!UseableForwardFromContext(requestable, controllable, useable) ? ContextResponse.FailBreak : ContextResponse.DoneBreak);
    }

    public static class PointUtil
    {
        public static bool SpriteOrOrigin(Component component, out Vector3 worldPoint)
        {
            ContextSprite sprite;
            if (ContextSprite.FindSprite(component, out sprite))
            {
                worldPoint = sprite.transform.position;
                return true;
            }
            worldPoint = component.transform.position;
            return false;
        }
    }
}

