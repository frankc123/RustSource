using Facepunch.Actor;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class RagdollHelper : MonoBehaviour
{
    private static void _RecursiveLinkTransformsByName(Transform ragdoll, Transform body)
    {
        for (int i = 0; i < ragdoll.childCount; i++)
        {
            Transform childAtIndex = FindChildHelper.GetChildAtIndex(ragdoll, i);
            Transform transform2 = FindChildHelper.FindChildByName(childAtIndex.name, body);
            if (transform2 != null)
            {
                childAtIndex.position = transform2.position;
                childAtIndex.rotation = transform2.rotation;
            }
            _RecursiveLinkTransformsByName(childAtIndex, body);
        }
    }

    private static void _RecursiveLinkTransformsByName(Transform ragdoll, Transform body, Transform bodyMatchTransform, ref Transform ragdollMatchTransform, ref bool foundMatch)
    {
        ragdollMatchTransform = null;
        for (int i = 0; i < ragdoll.childCount; i++)
        {
            Transform childAtIndex = FindChildHelper.GetChildAtIndex(ragdoll, i);
            Transform transform2 = FindChildHelper.FindChildByName(childAtIndex.name, body);
            if (transform2 != null)
            {
                childAtIndex.position = transform2.position;
                childAtIndex.rotation = transform2.rotation;
                if (!foundMatch && (transform2 == bodyMatchTransform))
                {
                    foundMatch = true;
                    ragdollMatchTransform = childAtIndex;
                }
                if (foundMatch)
                {
                    _RecursiveLinkTransformsByName(childAtIndex, transform2);
                }
                else
                {
                    _RecursiveLinkTransformsByName(childAtIndex, transform2, bodyMatchTransform, ref ragdollMatchTransform, ref foundMatch);
                }
            }
        }
    }

    public static void RecursiveLinkTransformsByName(Transform ragdoll, Transform body)
    {
        BoneStructure component = body.GetComponent<BoneStructure>();
        if (component != null)
        {
            BoneStructure structure2 = ragdoll.GetComponent<BoneStructure>();
            if (structure2 != null)
            {
                using (BoneStructure.ParentDownOrdered.Enumerator enumerator = component.parentDown.GetEnumerator())
                {
                    using (BoneStructure.ParentDownOrdered.Enumerator enumerator2 = structure2.parentDown.GetEnumerator())
                    {
                        while (enumerator.MoveNext() && enumerator2.MoveNext())
                        {
                            Transform current = enumerator.Current;
                            Transform transform2 = enumerator2.Current;
                            transform2.position = current.position;
                            transform2.rotation = current.rotation;
                        }
                    }
                }
                return;
            }
        }
        _RecursiveLinkTransformsByName(ragdoll, body);
    }

    public static bool RecursiveLinkTransformsByName(Transform ragdoll, Transform body, Transform bodyMatchTransform, out Transform ragdollMatchTransform)
    {
        if (bodyMatchTransform == null)
        {
            ragdollMatchTransform = null;
            RecursiveLinkTransformsByName(ragdoll, body);
            return false;
        }
        if (body == bodyMatchTransform)
        {
            ragdollMatchTransform = ragdoll;
            RecursiveLinkTransformsByName(ragdoll, body);
            return true;
        }
        BoneStructure component = body.GetComponent<BoneStructure>();
        if (component != null)
        {
            BoneStructure structure2 = ragdoll.GetComponent<BoneStructure>();
            if (structure2 != null)
            {
                using (BoneStructure.ParentDownOrdered.Enumerator enumerator = component.parentDown.GetEnumerator())
                {
                    using (BoneStructure.ParentDownOrdered.Enumerator enumerator2 = structure2.parentDown.GetEnumerator())
                    {
                        while (enumerator.MoveNext() && enumerator2.MoveNext())
                        {
                            Transform current = enumerator.Current;
                            Transform transform2 = enumerator2.Current;
                            transform2.position = current.position;
                            transform2.rotation = current.rotation;
                            if (current == bodyMatchTransform)
                            {
                                ragdollMatchTransform = transform2;
                                while (enumerator.MoveNext() && enumerator2.MoveNext())
                                {
                                    current = enumerator.Current;
                                    transform2 = enumerator2.Current;
                                    transform2.position = current.position;
                                    transform2.rotation = current.rotation;
                                }
                                return true;
                            }
                        }
                    }
                }
                ragdollMatchTransform = null;
                return false;
            }
        }
        bool foundMatch = false;
        ragdollMatchTransform = null;
        _RecursiveLinkTransformsByName(ragdoll, body, bodyMatchTransform, ref ragdollMatchTransform, ref foundMatch);
        return foundMatch;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}

