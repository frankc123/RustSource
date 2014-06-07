using Facepunch.Util;
using Facepunch.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class ConsoleSystem
{
    private static UnityEngine.Application.LogCallback LogCallback;
    private static bool LogCallbackWritesToConsole;
    private static bool RegisteredLogCallback;

    public static string CollectSavedFields(Type type)
    {
        string str = string.Empty;
        FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].IsStatic && Reflection.HasAttribute(fields[i], typeof(Saved)))
            {
                string str2 = type.Name + ".";
                if (str2 == "global.")
                {
                    str2 = string.Empty;
                }
                string str3 = str;
                string[] textArray1 = new string[] { str3, str2, fields[i].Name, " ", fields[i].GetValue(null).ToString(), "\n" };
                str = string.Concat(textArray1);
            }
        }
        return str;
    }

    public static string CollectSavedFunctions(Type type)
    {
        string str = string.Empty;
        MethodInfo[] methods = type.GetMethods();
        for (int i = 0; i < methods.Length; i++)
        {
            if ((methods[i].IsStatic && Reflection.HasAttribute(methods[i], typeof(Saved))) && (methods[i].ReturnType == typeof(string)))
            {
                str = str + methods[i].Invoke(null, null);
            }
        }
        return str;
    }

    public static string CollectSavedProperties(Type type)
    {
        string str = string.Empty;
        PropertyInfo[] properties = type.GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].GetGetMethod().IsStatic && Reflection.HasAttribute(properties[i], typeof(Saved)))
            {
                string str2 = type.Name + ".";
                if (str2 == "global.")
                {
                    str2 = string.Empty;
                }
                string str3 = str;
                string[] textArray1 = new string[] { str3, str2, properties[i].Name, " ", properties[i].GetValue(null, null).ToString(), "\n" };
                str = string.Concat(textArray1);
            }
        }
        return str;
    }

    public static Type[] FindTypes(string className)
    {
        List<Type> list = new List<Type>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type item = assemblies[i].GetType(className);
            if ((item != null) && item.IsSubclassOf(typeof(ConsoleSystem)))
            {
                list.Add(item);
            }
        }
        return list.ToArray();
    }

    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        Debug.Log(message, context);
    }

    public static void LogError(object message)
    {
        Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        Debug.LogError(message, context);
    }

    public static void LogException(Exception exception)
    {
        Debug.LogException(exception);
    }

    public static void LogException(Exception exception, Object context)
    {
        Debug.LogException(exception, context);
    }

    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(message, context);
    }

    public static void Print(object message, bool toLogFile = false)
    {
        PrintLogType(LogType.Log, message, toLogFile);
    }

    public static void PrintError(object message, bool toLogFile = false)
    {
        PrintLogType(LogType.Error, message, toLogFile);
    }

    private static void PrintLogType(LogType logType, object obj, bool log = false)
    {
        if (obj == null)
        {
        }
        PrintLogType(logType, "Null", log);
    }

    private static void PrintLogType(LogType logType, string message, bool log = false)
    {
        if (global.logprint)
        {
            switch (logType)
            {
                case LogType.Error:
                    LogError(message);
                    return;

                case LogType.Warning:
                    LogWarning(message);
                    return;

                case LogType.Log:
                    Log(message);
                    return;
            }
        }
        if (log && !LogCallbackWritesToConsole)
        {
            try
            {
                ((logType != LogType.Log) ? Console.Error : Console.Out).WriteLine("Print{0}:{1}", logType, message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("PrintLogType Log Exception\n:{0}", exception);
            }
        }
        if (RegisteredLogCallback)
        {
            try
            {
                LogCallback(message, string.Empty, logType);
            }
            catch (Exception exception2)
            {
                Console.Error.WriteLine("PrintLogType Exception\n:{0}", exception2);
            }
        }
    }

    public static void PrintWarning(object message, bool toLogFile = false)
    {
        PrintLogType(LogType.Warning, message, toLogFile);
    }

    public static void RegisterLogCallback(UnityEngine.Application.LogCallback Callback, bool CallbackWritesToConsole = false)
    {
        if (RegisteredLogCallback)
        {
            if (Callback != LogCallback)
            {
                if (object.ReferenceEquals(Callback, null))
                {
                    Application.RegisterLogCallback(null);
                    LogCallbackWritesToConsole = RegisteredLogCallback = false;
                    LogCallback = null;
                }
                else
                {
                    Application.RegisterLogCallback(Callback);
                    LogCallback = Callback;
                    LogCallbackWritesToConsole = CallbackWritesToConsole;
                }
            }
            else
            {
                LogCallbackWritesToConsole = CallbackWritesToConsole;
            }
        }
        else if (!object.ReferenceEquals(Callback, null))
        {
            Application.RegisterLogCallback(Callback);
            RegisteredLogCallback = true;
            LogCallbackWritesToConsole = CallbackWritesToConsole;
            LogCallback = Callback;
        }
    }

    public static bool Run(string strCommand, bool bWantsFeedback = false)
    {
        string strOutput = string.Empty;
        bool flag = RunCommand_Clientside(strCommand, out strOutput, bWantsFeedback);
        if (strOutput.Length > 0)
        {
            Debug.Log(strOutput);
        }
        return flag;
    }

    public static bool RunCommand(ref Arg arg, bool bWantReply = true)
    {
        Type[] typeArray = FindTypes(arg.Class);
        if (typeArray.Length == 0)
        {
            if (bWantReply)
            {
                arg.ReplyWith("Console class not found: " + arg.Class);
            }
            return false;
        }
        if (bWantReply)
        {
            arg.ReplyWith("command " + arg.Class + "." + arg.Function + " was executed");
        }
        foreach (Type type in typeArray)
        {
            MethodInfo method = type.GetMethod(arg.Function);
            if ((method != null) && method.IsStatic)
            {
                if (!arg.CheckPermissions(method.GetCustomAttributes(true)))
                {
                    if (bWantReply)
                    {
                        arg.ReplyWith("No permission: " + arg.Class + "." + arg.Function);
                    }
                    return false;
                }
                Arg[] argArray1 = new Arg[] { arg };
                object[] parameters = argArray1;
                try
                {
                    method.Invoke(null, parameters);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Error: " + arg.Class + "." + arg.Function + " - " + exception.Message);
                    arg.ReplyWith("Error: " + arg.Class + "." + arg.Function + " - " + exception.Message);
                    return false;
                }
                arg = parameters[0] as Arg;
                return true;
            }
            FieldInfo field = type.GetField(arg.Function);
            if ((field != null) && field.IsStatic)
            {
                if (!arg.CheckPermissions(field.GetCustomAttributes(true)))
                {
                    if (bWantReply)
                    {
                        arg.ReplyWith("No permission: " + arg.Class + "." + arg.Function);
                    }
                    return false;
                }
                Type fieldType = field.FieldType;
                if (arg.HasArgs(1))
                {
                    try
                    {
                        string str = field.GetValue(null).ToString();
                        if (fieldType == typeof(float))
                        {
                            field.SetValue(null, float.Parse(arg.Args[0]));
                        }
                        if (fieldType == typeof(int))
                        {
                            field.SetValue(null, int.Parse(arg.Args[0]));
                        }
                        if (fieldType == typeof(string))
                        {
                            field.SetValue(null, arg.Args[0]);
                        }
                        if (fieldType == typeof(bool))
                        {
                            field.SetValue(null, bool.Parse(arg.Args[0]));
                        }
                        if (bWantReply)
                        {
                            arg.ReplyWith(arg.Class + "." + arg.Function + ": changed " + String.QuoteSafe(str) + " to " + String.QuoteSafe(field.GetValue(null).ToString()) + " (" + fieldType.Name + ")");
                        }
                    }
                    catch (Exception)
                    {
                        if (bWantReply)
                        {
                            arg.ReplyWith("error setting value: " + arg.Class + "." + arg.Function);
                        }
                    }
                }
                else if (bWantReply)
                {
                    arg.ReplyWith(arg.Class + "." + arg.Function + ": " + String.QuoteSafe(field.GetValue(null).ToString()) + " (" + fieldType.Name + ")");
                }
                return true;
            }
            PropertyInfo property = type.GetProperty(arg.Function);
            if (((property != null) && property.GetGetMethod().IsStatic) && property.GetSetMethod().IsStatic)
            {
                if (!arg.CheckPermissions(property.GetCustomAttributes(true)))
                {
                    if (bWantReply)
                    {
                        arg.ReplyWith("No permission: " + arg.Class + "." + arg.Function);
                    }
                    return false;
                }
                Type propertyType = property.PropertyType;
                if (arg.HasArgs(1))
                {
                    try
                    {
                        string str2 = property.GetValue(null, null).ToString();
                        if (propertyType == typeof(float))
                        {
                            property.SetValue(null, float.Parse(arg.Args[0]), null);
                        }
                        if (propertyType == typeof(int))
                        {
                            property.SetValue(null, int.Parse(arg.Args[0]), null);
                        }
                        if (propertyType == typeof(string))
                        {
                            property.SetValue(null, arg.Args[0], null);
                        }
                        if (propertyType == typeof(bool))
                        {
                            property.SetValue(null, bool.Parse(arg.Args[0]), null);
                        }
                        if (bWantReply)
                        {
                            arg.ReplyWith(arg.Class + "." + arg.Function + ": changed " + String.QuoteSafe(str2) + " to " + String.QuoteSafe(property.GetValue(null, null).ToString()) + " (" + propertyType.Name + ")");
                        }
                    }
                    catch (Exception)
                    {
                        if (bWantReply)
                        {
                            arg.ReplyWith("error setting value: " + arg.Class + "." + arg.Function);
                        }
                    }
                }
                else if (bWantReply)
                {
                    arg.ReplyWith(arg.Class + "." + arg.Function + ": " + String.QuoteSafe(property.GetValue(null, null).ToString()) + " (" + propertyType.Name + ")");
                }
                return true;
            }
        }
        if (bWantReply)
        {
            arg.ReplyWith("Command not found: " + arg.Class + "." + arg.Function);
        }
        return false;
    }

    public static bool RunCommand_Clientside(string strCommand, out string StrOutput, bool bWantsFeedback = false)
    {
        StrOutput = string.Empty;
        Arg arg = new Arg(strCommand);
        if (arg.Invalid)
        {
            return false;
        }
        if (!RunCommand(ref arg, bWantsFeedback))
        {
            return false;
        }
        if ((arg.Reply != null) && (arg.Reply.Length > 0))
        {
            StrOutput = arg.Reply;
        }
        return true;
    }

    public static void RunFile(string strFile)
    {
        char[] separator = new char[] { '\n' };
        foreach (string str in strFile.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
            if (str[0] != '#')
            {
                Run(str, false);
            }
        }
    }

    public static string SaveToConfigString()
    {
        string str = string.Empty;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type[] types = assemblies[i].GetTypes();
            for (int j = 0; j < types.Length; j++)
            {
                if (types[j].IsSubclassOf(typeof(ConsoleSystem)))
                {
                    str = (str + CollectSavedFields(types[j])) + CollectSavedProperties(types[j]) + CollectSavedFunctions(types[j]);
                }
            }
        }
        return str;
    }

    public static bool UnregisterLogCallback(UnityEngine.Application.LogCallback Callback)
    {
        if (RegisteredLogCallback && (Callback == LogCallback))
        {
            RegisterLogCallback(null, false);
            return true;
        }
        return false;
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class Admin : Attribute
    {
    }

    public class Arg
    {
        public string[] Args;
        public string ArgsStr = string.Empty;
        public string Class = string.Empty;
        public string Function = string.Empty;
        public bool Invalid = true;
        public string Reply = string.Empty;

        public Arg(string rconCommand)
        {
            rconCommand = RemoveInvalidCharacters(rconCommand);
            if ((rconCommand.IndexOf('.') <= 0) || (rconCommand.IndexOf(' ', 0, rconCommand.IndexOf('.')) != -1))
            {
                rconCommand = "global." + rconCommand;
            }
            if (rconCommand.IndexOf('.') > 0)
            {
                this.Class = rconCommand.Substring(0, rconCommand.IndexOf('.'));
                if (this.Class.Length > 1)
                {
                    this.Class = this.Class.ToLower();
                    this.Function = rconCommand.Substring(this.Class.Length + 1);
                    if (this.Function.Length > 1)
                    {
                        this.Invalid = false;
                        if (this.Function.IndexOf(' ') > 0)
                        {
                            this.ArgsStr = this.Function.Substring(this.Function.IndexOf(' '));
                            this.ArgsStr = this.ArgsStr.Trim();
                            this.Args = String.SplitQuotesStrings(this.ArgsStr);
                            this.Function = this.Function.Substring(0, this.Function.IndexOf(' '));
                            this.Function.ToLower();
                        }
                    }
                }
            }
        }

        public bool CheckPermissions(object[] attributes)
        {
            foreach (object obj2 in attributes)
            {
                if (obj2 is ConsoleSystem.Client)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetBool(int iArg, bool def = false)
        {
            return ConsoleSystem.Parse.DefaultBool(this.GetString(iArg, null), def);
        }

        public Enum GetEnum(Type enumType, int iArg, Enum def)
        {
            return ConsoleSystem.Parse.DefaultEnum(enumType, this.GetString(iArg, null), def);
        }

        public float GetFloat(int iArg, float def = 0f)
        {
            return ConsoleSystem.Parse.DefaultFloat(this.GetString(iArg, null), def);
        }

        public int GetInt(int iArg, int def = 0)
        {
            return ConsoleSystem.Parse.DefaultInt(this.GetString(iArg, null), def);
        }

        public string GetString(int iArg, string def = "")
        {
            if (this.HasArgs(iArg + 1))
            {
                return ConsoleSystem.Parse.DefaultString(this.Args[iArg], def);
            }
            return def;
        }

        public ulong GetUInt64(int iArg, ulong def = 0)
        {
            return ConsoleSystem.Parse.DefaultUInt64(this.GetString(iArg, null), def);
        }

        public bool HasArgs(int iMinimum = 1)
        {
            if (this.Args == null)
            {
                return false;
            }
            return (this.Args.Length >= iMinimum);
        }

        private static string RemoveInvalidCharacters(string str)
        {
            if (str == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if ((char.IsLetterOrDigit(c) || char.IsPunctuation(c)) || (char.IsSeparator(c) || char.IsSymbol(c)))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public void ReplyWith(string strValue)
        {
            this.Reply = strValue;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class Client : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class Help : Attribute
    {
        public string argsDescription;
        public string helpDescription;

        public Help(string strHelp, string strArgs = "")
        {
            this.helpDescription = strHelp;
            this.argsDescription = strArgs;
        }
    }

    public static class Parse
    {
        private const bool kEnumCaseInsensitive = true;

        public static bool AttemptBool(string text, out bool value)
        {
            if (bool.TryParse(text, out value))
            {
                return true;
            }
            if (text.Length != 0)
            {
                if (char.IsLetter(text[0]))
                {
                    if (text.Length != 4)
                    {
                        if (text.Length != 5)
                        {
                            decimal num;
                            if (decimal.TryParse(text, out num))
                            {
                                value = num != 0M;
                                return true;
                            }
                        }
                        else if (string.Equals(text, "false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = false;
                            return true;
                        }
                    }
                    else if (string.Equals(text, "true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = true;
                        return true;
                    }
                }
                else
                {
                    decimal num2;
                    if (decimal.TryParse(text, out num2))
                    {
                        value = num2 != 0M;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AttemptEnum<TEnum>(string text, out TEnum value) where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            return VerifyEnum<TEnum>.TryParse(text, out value);
        }

        public static bool AttemptEnum(Type enumType, string text, out System.Enum value)
        {
            bool flag;
            try
            {
                value = (System.Enum) System.Enum.Parse(enumType, text, true);
                flag = true;
            }
            catch
            {
                try
                {
                    value = (System.Enum) System.Enum.ToObject(enumType, long.Parse(text));
                    return true;
                }
                catch
                {
                    value = 0;
                    return false;
                }
            }
            return flag;
        }

        public static bool AttemptFloat(string text, out float value)
        {
            return float.TryParse(text, out value);
        }

        public static bool AttemptInt(string text, out int value)
        {
            return int.TryParse(text, out value);
        }

        public static bool AttemptObject(Type type, string value, out object boxed)
        {
            try
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        if (typeof(bool) != type)
                        {
                            goto Label_014B;
                        }
                        boxed = Bool(value);
                        return true;

                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        if (!type.IsEnum)
                        {
                            goto Label_014B;
                        }
                        boxed = Enum(type, value);
                        return true;

                    case TypeCode.Int32:
                        if (type != typeof(int))
                        {
                            break;
                        }
                        boxed = Int(value);
                        goto Label_0094;

                    case TypeCode.Single:
                        if (typeof(float) != type)
                        {
                            goto Label_014B;
                        }
                        boxed = Float(value);
                        return true;

                    case TypeCode.String:
                        if (typeof(string) != type)
                        {
                            goto Label_014B;
                        }
                        boxed = String(value);
                        return true;

                    default:
                        goto Label_014B;
                }
                if (!type.IsEnum)
                {
                    goto Label_014B;
                }
                boxed = Enum(type, value);
            Label_0094:
                return true;
            }
            catch (Exception exception)
            {
                boxed = exception;
                return false;
            }
        Label_014B:
            boxed = null;
            return false;
        }

        public static bool AttemptString(string text, out string value)
        {
            if (string.IsNullOrEmpty(text))
            {
                value = string.Empty;
                return false;
            }
            value = text;
            return true;
        }

        public static bool Bool(string text)
        {
            bool flag;
            if (!AttemptBool(text, out flag))
            {
                throw new FormatException("not in the correct format.");
            }
            return flag;
        }

        public static bool DefaultBool(string text)
        {
            return DefaultBool(text, false);
        }

        public static bool DefaultBool(string text, bool @default)
        {
            bool flag;
            if (!object.ReferenceEquals(text, null) && AttemptBool(text, out flag))
            {
                return flag;
            }
            return @default;
        }

        public static TEnum DefaultEnum<TEnum>(string text) where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            return DefaultEnum<TEnum>(text, default(TEnum));
        }

        public static TEnum DefaultEnum<TEnum>(string text, TEnum @default) where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            TEnum local;
            if (!object.ReferenceEquals(text, null) && AttemptEnum<TEnum>(text, out local))
            {
                return local;
            }
            return @default;
        }

        public static System.Enum DefaultEnum(Type enumType, string text)
        {
            return DefaultEnum(enumType, text, 0);
        }

        public static System.Enum DefaultEnum(Type enumType, string text, System.Enum @default)
        {
            System.Enum enum2;
            if (!object.ReferenceEquals(text, null) && AttemptEnum(enumType, text, out enum2))
            {
                return enum2;
            }
            return @default;
        }

        public static float DefaultFloat(string text)
        {
            return DefaultFloat(text, 0f);
        }

        public static float DefaultFloat(string text, float @default)
        {
            float num;
            if (!object.ReferenceEquals(text, null) && AttemptFloat(text, out num))
            {
                return num;
            }
            return @default;
        }

        public static int DefaultInt(string text)
        {
            return DefaultInt(text, 0);
        }

        public static int DefaultInt(string text, int @default)
        {
            int num;
            if (!object.ReferenceEquals(text, null) && AttemptInt(text, out num))
            {
                return num;
            }
            return @default;
        }

        public static string DefaultString(string text)
        {
            return DefaultString(text, string.Empty);
        }

        public static string DefaultString(string text, string @default)
        {
            string str;
            if (!AttemptString(text, out str))
            {
                str = @default;
            }
            return str;
        }

        public static ulong DefaultUInt64(string text, ulong @default)
        {
            if (text == null)
            {
                return @default;
            }
            ulong result = @default;
            ulong.TryParse(text, out result);
            return result;
        }

        public static TEnum Enum<TEnum>(string text) where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            return VerifyEnum<TEnum>.Parse(text);
        }

        public static System.Enum Enum(Type enumType, string text)
        {
            System.Enum enum2;
            try
            {
                enum2 = (System.Enum) System.Enum.Parse(enumType, text, true);
            }
            catch (Exception exception)
            {
                try
                {
                    return (System.Enum) System.Enum.ToObject(enumType, long.Parse(text));
                }
                catch
                {
                    throw exception;
                }
            }
            return enum2;
        }

        public static float Float(string text)
        {
            return float.Parse(text);
        }

        public static int Int(string text)
        {
            return int.Parse(text);
        }

        public static bool IsSupported<T>()
        {
            return PrecachedSupport<T>.IsSupported;
        }

        public static bool IsSupported(Type type)
        {
            if (!object.ReferenceEquals(type, null))
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return (typeof(bool) == type);

                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return type.IsEnum;

                    case TypeCode.Int32:
                        return ((typeof(int) == type) || type.IsEnum);

                    case TypeCode.Single:
                        return (typeof(float) == type);

                    case TypeCode.String:
                        return (typeof(string) == type);
                }
            }
            return false;
        }

        public static string String(string text)
        {
            if (object.ReferenceEquals(text, null))
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 1)
            {
                throw new FormatException("Cannot use empty strings.");
            }
            return text;
        }

        private static class PrecachedSupport<T>
        {
            public static readonly bool IsSupported;

            static PrecachedSupport()
            {
                ConsoleSystem.Parse.PrecachedSupport<T>.IsSupported = ConsoleSystem.Parse.IsSupported(typeof(T));
            }
        }

        private static class VerifyEnum<TEnum> where TEnum: struct, IComparable, IFormattable, IConvertible
        {
            static VerifyEnum()
            {
                if (!typeof(TEnum).IsEnum)
                {
                    throw new ArgumentException("TEnum", "Is not a enum type");
                }
            }

            public static TEnum Parse(string text)
            {
                TEnum local;
                try
                {
                    local = (TEnum) Enum.Parse(typeof(TEnum), text, true);
                }
                catch (Exception exception)
                {
                    try
                    {
                        return (TEnum) Enum.ToObject(typeof(TEnum), long.Parse(text));
                    }
                    catch
                    {
                        throw exception;
                    }
                }
                return local;
            }

            public static bool TryParse(string text, out TEnum value)
            {
                bool flag;
                try
                {
                    value = (TEnum) Enum.Parse(typeof(TEnum), text, true);
                    flag = true;
                }
                catch
                {
                    try
                    {
                        value = (TEnum) Enum.ToObject(typeof(TEnum), long.Parse(text));
                        return true;
                    }
                    catch
                    {
                        value = default(TEnum);
                        return false;
                    }
                }
                return flag;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class Saved : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class User : Attribute
    {
    }
}

