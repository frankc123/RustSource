using System;
using System.Reflection;
using UnityEngine;

public class dfClipboardHelper
{
    private static PropertyInfo m_systemCopyBufferProperty;

    private static PropertyInfo GetSystemCopyBufferProperty()
    {
        if (m_systemCopyBufferProperty == null)
        {
            m_systemCopyBufferProperty = typeof(GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.NonPublic | BindingFlags.Static);
            if (m_systemCopyBufferProperty == null)
            {
                throw new Exception("Can'time access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
            }
        }
        return m_systemCopyBufferProperty;
    }

    public static string clipBoard
    {
        get
        {
            try
            {
                return (string) GetSystemCopyBufferProperty().GetValue(null, null);
            }
            catch
            {
                return string.Empty;
            }
        }
        set
        {
            try
            {
                GetSystemCopyBufferProperty().SetValue(null, value, null);
            }
            catch
            {
            }
        }
    }
}

