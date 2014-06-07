using RustProto;
using System;
using UnityEngine;

public class AvatarSaveRestore : MonoBehaviour
{
    public static void CopyPersistantMessages(ref Avatar.Builder builder, ref Avatar avatar)
    {
        builder.ClearBlueprints();
        for (int i = 0; i < avatar.BlueprintsCount; i++)
        {
            builder.AddBlueprints(avatar.GetBlueprints(i));
        }
    }
}

