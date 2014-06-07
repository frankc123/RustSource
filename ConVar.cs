using System;
using System.Reflection;

public class ConVar
{
    public static bool GetBool(string strName, bool strDefault)
    {
        string str = GetString(strName, !strDefault ? bool.FalseString : bool.TrueString);
        try
        {
            return bool.Parse(str);
        }
        catch
        {
            return (GetInt(strName, !strDefault ? ((float) 0) : ((float) 1)) != 0);
        }
    }

    public static float GetFloat(string strName, float strDefault)
    {
        string s = GetString(strName, string.Empty);
        if (s.Length != 0)
        {
            float result = strDefault;
            if (float.TryParse(s, out result))
            {
                return result;
            }
        }
        return strDefault;
    }

    public static int GetInt(string strName, float strDefault)
    {
        return (int) GetFloat(strName, strDefault);
    }

    public static string GetString(string strName, string strDefault)
    {
        ConsoleSystem.Arg arg = new ConsoleSystem.Arg(strName);
        if (!arg.Invalid)
        {
            Type[] typeArray = ConsoleSystem.FindTypes(arg.Class);
            if (typeArray.Length == 0)
            {
                return strDefault;
            }
            foreach (Type type in typeArray)
            {
                FieldInfo field = type.GetField(arg.Function);
                if ((field != null) && field.IsStatic)
                {
                    return field.GetValue(null).ToString();
                }
                PropertyInfo property = type.GetProperty(arg.Function);
                if ((property != null) && property.GetGetMethod().IsStatic)
                {
                    return property.GetValue(null, null).ToString();
                }
            }
        }
        return strDefault;
    }
}

