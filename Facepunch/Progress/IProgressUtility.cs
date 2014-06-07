namespace Facepunch.Progress
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class IProgressUtility
    {
        public static bool Poll(this IProgress IProgress, out float progress)
        {
            bool flag;
            if (IProgress is Object)
            {
                flag = ((Object) IProgress) == 0;
            }
            else
            {
                flag = object.ReferenceEquals(IProgress, null);
            }
            if (flag)
            {
                progress = 0f;
                return false;
            }
            float num = IProgress.progress;
            if (num >= 1f)
            {
                progress = 1f;
            }
            else if (num <= 0f)
            {
                progress = 0f;
            }
            else
            {
                progress = num;
            }
            return true;
        }
    }
}

