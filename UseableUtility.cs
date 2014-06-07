using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class UseableUtility
{
    public const UseResponse kMaxException = UseResponse.Fail_Vacancy;
    public const UseResponse kMaxFailedChecked = ((UseResponse) (-17));
    public const UseResponse kMinException = UseResponse.Fail_CheckException;
    public const UseResponse kMinSuccess = UseResponse.Pass_Unchecked;
    public const UseResponse kMinSucessChecked = UseResponse.Pass_Checked;
    private static bool log_enabled;

    public static bool Checked(this UseResponse response)
    {
        return ((((int) response) < -16) || (((int) response) > 0));
    }

    public static void Log<T>(T a)
    {
        if (log_enabled)
        {
            Debug.Log(a);
        }
    }

    public static void Log<T>(T a, Object b)
    {
        if (log_enabled)
        {
            Debug.Log(a, b);
        }
    }

    public static void LogError<T>(T a)
    {
        if (log_enabled)
        {
            Debug.LogError(a);
        }
    }

    public static void LogError<T>(T a, Object b)
    {
        if (log_enabled)
        {
            Debug.LogError(a, b);
        }
    }

    public static void LogWarning<T>(T a)
    {
        if (log_enabled)
        {
            Debug.LogWarning(a);
        }
    }

    public static void LogWarning<T>(T a, Object b)
    {
        if (log_enabled)
        {
            Debug.LogWarning(a, b);
        }
    }

    public static void OnDestroy(IUseable self)
    {
        MonoBehaviour behaviour = self as MonoBehaviour;
        if (behaviour != null)
        {
            OnDestroy(self, behaviour.GetComponent<Useable>());
        }
    }

    public static void OnDestroy(IUseable self, Useable useable)
    {
        if ((useable != null) && useable.occupied)
        {
            useable.Eject();
        }
    }

    public static bool Succeeded(this UseResponse response)
    {
        bool flag = ((int) response) >= 0;
        if (!flag)
        {
            LogWarning<string>("Did not succeed " + response);
        }
        return flag;
    }

    public static bool ThrewException(this UseResponse response, out Exception e)
    {
        return response.ThrewException(out e, false);
    }

    public static bool ThrewException(this UseResponse response, out Exception e, bool doNotClear)
    {
        if ((((int) response) >= -16) && (((int) response) <= -10))
        {
            return Useable.GetLastException(out e, doNotClear);
        }
        e = null;
        return false;
    }

    public static bool ThrewException<E>(this UseResponse response, out E e, bool doNotClear) where E: Exception
    {
        if ((((int) response) >= -16) && (((int) response) <= -10))
        {
            return Useable.GetLastException<E>(out e, doNotClear);
        }
        e = null;
        return false;
    }
}

