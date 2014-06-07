using System;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IContextRequestablePointText : IContextRequestable, IContextRequestableText, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    bool ContextTextPoint(out Vector3 worldPoint);
}

