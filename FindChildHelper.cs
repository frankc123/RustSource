using System;
using UnityEngine;

public static class FindChildHelper
{
    private static Transform found;

    private static bool __FindChildByNameRecurse(string name, Transform parent)
    {
        if (parent.childCount != 0)
        {
            found = parent.Find(name);
            if (found != null)
            {
                return true;
            }
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if ((child.childCount > 0) && __FindChildByNameRecurse(name, child))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool _FindChildByNameRecurse(string name, Transform parent)
    {
        return __FindChildByNameRecurse(name, parent);
    }

    private static Transform _GetFound()
    {
        Transform found = FindChildHelper.found;
        FindChildHelper.found = null;
        return found;
    }

    [Obsolete("If this is being called in Start, Awake, or OnEnabled consider using the @PrefetchChildComponent on the variable.", false)]
    public static Transform FindChildByName(string name, Component parent)
    {
        if (parent.name == name)
        {
            return parent.transform;
        }
        if (_FindChildByNameRecurse(name, parent.transform))
        {
            return _GetFound();
        }
        return NoChildNamed(name, parent);
    }

    [Obsolete("If this is being called in Start, Awake, or OnEnabled consider using the @PrefetchChildComponent on the variable.", false)]
    public static Transform FindChildByName(string name, GameObject parent)
    {
        if (parent.name == name)
        {
            return parent.transform;
        }
        if (_FindChildByNameRecurse(name, parent.transform))
        {
            return _GetFound();
        }
        return NoChildNamed(name, parent);
    }

    [Obsolete("If this is being called in Start, Awake, or OnEnabled consider using the @PrefetchChildComponent on the variable.", false)]
    public static Transform FindChildByName(string name, Transform parent)
    {
        if (parent.name == name)
        {
            return parent;
        }
        if (_FindChildByNameRecurse(name, parent))
        {
            return _GetFound();
        }
        return NoChildNamed(name, parent);
    }

    public static Transform GetChildAtIndex(Transform transform, int i)
    {
        if ((0 <= i) && (transform.childCount > i))
        {
            return transform.GetChild(i);
        }
        return null;
    }

    private static Transform NoChildNamed(string name, Object parent)
    {
        return null;
    }

    public static Transform RandomChild(Transform transform)
    {
        int childCount = transform.childCount;
        switch (childCount)
        {
            case 0:
                return null;

            case 1:
                return transform.GetChild(0);
        }
        return GetChildAtIndex(transform, Random.Range(0, childCount));
    }
}

