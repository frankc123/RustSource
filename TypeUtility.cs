using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public static class TypeUtility
{
    private static bool ginit;
    private static readonly string[] hintsAQN = new string[] { ", Version=", ", Culture=", ", PublicKeyToken=" };

    private static bool ContainsAQN(string text)
    {
        int index = text.IndexOf(", ");
        if (index != -1)
        {
            for (int i = 0; i < hintsAQN.Length; i++)
            {
                if (text.IndexOf(hintsAQN[i], index) != -1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static Type Parse(string text)
    {
        Type type;
        if (object.ReferenceEquals(text, null))
        {
            throw new ArgumentNullException("text");
        }
        if (text.Length == 0)
        {
            throw new ArgumentException("text.Length==0", "text");
        }
        if (!Parse(text, out type))
        {
            throw new ArgumentException("could not get type", text);
        }
        return type;
    }

    public static Type Parse<TRequiredBaseClass>(string text) where TRequiredBaseClass: class
    {
        Type type;
        if (object.ReferenceEquals(text, null))
        {
            throw new ArgumentNullException("text");
        }
        if (text.Length == 0)
        {
            throw new ArgumentException("text.Length==0", "text");
        }
        if (!Parse(typeof(TRequiredBaseClass), text, out type))
        {
            throw new ArgumentException("could not get type that would match base class " + typeof(TRequiredBaseClass), text);
        }
        return type;
    }

    private static bool Parse(string text, out Type type)
    {
        if (Parse(text, false, out type))
        {
            return true;
        }
        if (ContainsAQN(text))
        {
            string str = g.StrippedName(text);
            return ((Parse(str, false, out type) || Parse(text, true, out type)) || Parse(str, true, out type));
        }
        return Parse(text, true, out type);
    }

    private static bool Parse(string text, bool ignoreCase, out Type type)
    {
        type = Type.GetType(text, false, ignoreCase);
        if (object.ReferenceEquals(type, null))
        {
            return false;
        }
        return true;
    }

    private static bool Parse(Type requiredType, string text, out Type type)
    {
        if (Parse(requiredType, text, false, out type))
        {
            return true;
        }
        if (ContainsAQN(text))
        {
            string str = g.StrippedName(text);
            return ((Parse(requiredType, str, false, out type) || Parse(requiredType, text, true, out type)) || Parse(requiredType, str, true, out type));
        }
        return Parse(requiredType, text, true, out type);
    }

    private static bool Parse(Type requiredBase, string text, bool ignoreCase, out Type type)
    {
        if (Parse(text, ignoreCase, out type))
        {
            if (requiredBase.IsAssignableFrom(type))
            {
                return true;
            }
            type = null;
        }
        return false;
    }

    public static bool TryParse(string text, out Type type)
    {
        if (string.IsNullOrEmpty(text))
        {
            type = null;
            return false;
        }
        return Parse(text, out type);
    }

    public static bool TryParse<TRequiredBaseClass>(string text, out Type type) where TRequiredBaseClass: class
    {
        if (string.IsNullOrEmpty(text))
        {
            type = null;
            return false;
        }
        return Parse(typeof(TRequiredBaseClass), text, out type);
    }

    public static string VersionlessName<T>()
    {
        return typeof(T).VersionlessName();
    }

    public static string VersionlessName(this Type type)
    {
        if (object.ReferenceEquals(type, null))
        {
            return null;
        }
        return g.StrippedName(type);
    }

    private static class g
    {
        private static readonly Dictionary<Type, string> strippedNames;

        static g()
        {
            TypeUtility.ginit = true;
            strippedNames = new Dictionary<Type, string>();
        }

        public static string StrippedName(string assemblyQualifiedName)
        {
            return expression.replace(assemblyQualifiedName);
        }

        public static string StrippedName(Type type)
        {
            string str;
            if (!strippedNames.TryGetValue(type, out str))
            {
                strippedNames[type] = str = expression.replace(type.AssemblyQualifiedName);
            }
            return str;
        }

        private static class expression
        {
            public static readonly Regex culture = new Regex(@", Culture=\w+", RegexOptions.Compiled);
            private const RegexOptions kRegexOptions = RegexOptions.Compiled;
            public static readonly Regex publicKeyToken = new Regex(@", PublicKeyToken=\w+", RegexOptions.Compiled);
            public static readonly Regex version = new Regex(@", Version=\d+.\d+.\d+.\d+", RegexOptions.Compiled);

            public static string replace(string assemblyQualifiedName)
            {
                return version.Replace(assemblyQualifiedName, string.Empty);
            }
        }
    }
}

