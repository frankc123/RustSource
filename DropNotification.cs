using NGUI.MessageUtil;
using System;
using UnityEngine;

public static class DropNotification
{
    public const DropNotificationFlags AltDrop = DropNotificationFlags.AltDrop;
    public const DropNotificationFlags AltDropThenLand = (DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop);
    public const DropNotificationFlags AltLand = DropNotificationFlags.AltLand;
    public const DropNotificationFlags AltLandThenDrop = (DropNotificationFlags.AltReverse | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop);
    public const DropNotificationFlags Disable = 0;
    public const DropNotificationFlags DragDrop = DropNotificationFlags.DragDrop;
    public const DropNotificationFlags DragDropThenLand = (DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop);
    public const DropNotificationFlags DragLand = DropNotificationFlags.DragLand;
    public const DropNotificationFlags DragLandOutside = DropNotificationFlags.DragLandOutside;
    public const DropNotificationFlags DragLandThenDrop = (DropNotificationFlags.DragReverse | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop);
    private static DragEventKind inDrag;
    public const DropNotificationFlags kAltReverseBit = DropNotificationFlags.AltReverse;
    public const DropNotificationFlags kDefault = DropNotificationFlags.DragDrop;
    public const DropNotificationFlags kDragReverseBit = DropNotificationFlags.DragReverse;
    private const DropNotificationFlags kInvalidNeverSet = -2147483648;
    public const DropNotificationFlags kMask_Active = (DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop);
    public const DropNotificationFlags kMask_Alt = (DropNotificationFlags.AltReverse | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop);
    public const DropNotificationFlags kMask_Drag = (DropNotificationFlags.DragReverse | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop);
    public const DropNotificationFlags kMask_Mid = (DropNotificationFlags.MidReverse | DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop);
    public const DropNotificationFlags kMidReverseBit = DropNotificationFlags.MidReverse;
    public const DropNotificationFlags MidDrop = DropNotificationFlags.MidDrop;
    public const DropNotificationFlags MidDropThenLand = (DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop);
    public const DropNotificationFlags MidLand = DropNotificationFlags.MidLand;
    public const DropNotificationFlags MidLandThenDrop = (DropNotificationFlags.MidReverse | DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop);
    private static GameObject scanItem;
    private static bool stopDrag;

    internal static bool DropMessage(ref DropNotificationFlags flags, DragEventKind kind, GameObject Pressed, GameObject Released)
    {
        bool flag;
        bool flag2;
        string str;
        string str2;
        DropNotificationFlags dragDrop;
        DropNotificationFlags dragLand;
        DropNotificationFlags dragReverse;
        switch (kind)
        {
            case DragEventKind.Drag:
                flag = true;
                if (Released == null)
                {
                    flag2 = false;
                    dragDrop = -2147483648;
                    dragReverse = DropNotificationFlags.DragLandOutside;
                    dragLand = DropNotificationFlags.DragLandOutside;
                    str = "----";
                    str2 = "OnLandOutside";
                    break;
                }
                flag2 = true;
                dragDrop = DropNotificationFlags.DragDrop;
                dragLand = DropNotificationFlags.DragLand;
                dragReverse = DropNotificationFlags.DragReverse;
                str = "OnDrop";
                str2 = "OnLand";
                break;

            case DragEventKind.Alt:
                flag2 = true;
                flag = false;
                dragDrop = DropNotificationFlags.AltDrop;
                dragLand = DropNotificationFlags.AltLand;
                dragReverse = DropNotificationFlags.AltReverse;
                str = "OnAltDrop";
                str2 = "OnAltLand";
                break;

            case DragEventKind.Mid:
                flag2 = true;
                flag = false;
                dragDrop = DropNotificationFlags.MidDrop;
                dragLand = DropNotificationFlags.MidLand;
                dragReverse = DropNotificationFlags.MidReverse;
                str = "OnMidDrop";
                str2 = "OnMidLand";
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        if ((flags & dragReverse) == dragReverse)
        {
            if ((flags & dragLand) == dragLand)
            {
                if (flag2)
                {
                    Message(Pressed, Released, str2, Pressed, kind, ref flag);
                }
                else
                {
                    Message(Pressed, str2, Pressed, kind, ref flag);
                }
            }
            if ((flags & dragDrop) == dragDrop)
            {
                if (flag2)
                {
                    Message(Released, Pressed, str, Pressed, kind, ref flag);
                    return flag;
                }
                Message(Released, str, Pressed, kind, ref flag);
            }
            return flag;
        }
        if ((flags & dragDrop) == dragDrop)
        {
            if (flag2)
            {
                Message(Released, Pressed, str, Pressed, kind, ref flag);
            }
            else
            {
                Message(Released, str, Pressed, kind, ref flag);
            }
        }
        if ((flags & dragLand) == dragLand)
        {
            if (flag2)
            {
                Message(Pressed, Released, str2, Pressed, kind, ref flag);
                return flag;
            }
            Message(Pressed, str2, Pressed, kind, ref flag);
        }
        return flag;
    }

    private static bool Message(GameObject target, string messageName, GameObject scan, DragEventKind kind, ref bool drop)
    {
        if (target != null)
        {
            GameObject scanItem = DropNotification.scanItem;
            bool stopDrag = DropNotification.stopDrag;
            DragEventKind inDrag = DropNotification.inDrag;
            try
            {
                DropNotification.scanItem = scan;
                DropNotification.stopDrag = drop;
                DropNotification.inDrag = kind;
                target.NGUIMessage(messageName);
                drop = DropNotification.stopDrag;
                return true;
            }
            finally
            {
                DropNotification.scanItem = scanItem;
                DropNotification.stopDrag = stopDrag;
                DropNotification.inDrag = inDrag;
            }
        }
        return false;
    }

    private static bool Message(GameObject target, GameObject parameter, string messageName, GameObject scan, DragEventKind kind, ref bool drop)
    {
        if (target != null)
        {
            GameObject scanItem = DropNotification.scanItem;
            bool stopDrag = DropNotification.stopDrag;
            DragEventKind inDrag = DropNotification.inDrag;
            try
            {
                DropNotification.scanItem = scan;
                DropNotification.stopDrag = drop;
                DropNotification.inDrag = kind;
                target.NGUIMessage(messageName, parameter);
                drop = DropNotification.stopDrag;
                return true;
            }
            finally
            {
                DropNotification.scanItem = scanItem;
                DropNotification.stopDrag = stopDrag;
                DropNotification.inDrag = inDrag;
            }
        }
        return false;
    }

    public static void StopDragging(GameObject item)
    {
        if (inDrag == DragEventKind.None)
        {
            Debug.LogError("StopDragging can only be called from within Drop or Land messages");
        }
        else if (item != scanItem)
        {
            Debug.LogWarning("StopDragging was called with a invalid value, should have been the thing being dragged");
        }
        else
        {
            stopDrag = true;
        }
    }
}

