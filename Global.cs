using System;
using System.Reflection;
using UnityEngine;

public class global : ConsoleSystem
{
    [Help("Prints fps at said interval", "interval (seconds)"), Client, Admin, User]
    public static float fpslog = -1f;
    [User, Client, Help("When set to True, all console printing will go through Debug.Log", "")]
    public static bool logprint;

    public static string BuildFieldsString(ref FieldInfo field)
    {
        string helpDescription = "no help";
        foreach (object obj2 in field.GetCustomAttributes(true))
        {
            if (obj2 is ConsoleSystem.Help)
            {
                helpDescription = (obj2 as ConsoleSystem.Help).helpDescription;
            }
        }
        return (field.Name + " : " + helpDescription);
    }

    public static string BuildMethodString(ref MethodInfo method)
    {
        string argsDescription = string.Empty;
        string helpDescription = "no help";
        foreach (object obj2 in method.GetCustomAttributes(true))
        {
            if (obj2 is ConsoleSystem.Help)
            {
                argsDescription = (obj2 as ConsoleSystem.Help).argsDescription;
                helpDescription = (obj2 as ConsoleSystem.Help).helpDescription;
                argsDescription = " " + argsDescription.Trim() + " ";
            }
        }
        string[] textArray1 = new string[] { method.Name, "(", argsDescription, ") : ", helpDescription };
        return string.Concat(textArray1);
    }

    public static string BuildPropertyString(ref PropertyInfo field)
    {
        string helpDescription = "no help";
        foreach (object obj2 in field.GetCustomAttributes(true))
        {
            if (obj2 is ConsoleSystem.Help)
            {
                helpDescription = (obj2 as ConsoleSystem.Help).helpDescription;
            }
        }
        return (field.Name + " : " + helpDescription);
    }

    [Client, Help("Creates an error", "")]
    public static void create_error(ref ConsoleSystem.Arg arg)
    {
        Debug.LogError("this is an error");
    }

    [Admin, User, Client, Help("Prints something to the debug output", "string output")]
    public static void echo(ref ConsoleSystem.Arg arg)
    {
        arg.ReplyWith(arg.ArgsStr);
    }

    [User, Client, Admin, Help("Search for a command", "string Name")]
    public static void find(ref ConsoleSystem.Arg arg)
    {
        if (arg.HasArgs(1))
        {
            string str = arg.Args[0];
            string str2 = string.Empty;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j].IsSubclassOf(typeof(ConsoleSystem)))
                    {
                        string str3;
                        MethodInfo[] methods = types[j].GetMethods();
                        for (int k = 0; k < methods.Length; k++)
                        {
                            if ((methods[k].IsStatic && (((str == "*") || types[j].Name.Contains(str)) || methods[k].Name.Contains(str))) && arg.CheckPermissions(methods[k].GetCustomAttributes(true)))
                            {
                                str3 = str2;
                                string[] textArray1 = new string[] { str3, types[j].Name, ".", BuildMethodString(ref methods[k]), "\n" };
                                str2 = string.Concat(textArray1);
                            }
                        }
                        FieldInfo[] fields = types[j].GetFields();
                        for (int m = 0; m < fields.Length; m++)
                        {
                            if ((fields[m].IsStatic && (((str == "*") || types[j].Name.Contains(str)) || fields[m].Name.Contains(str))) && arg.CheckPermissions(fields[m].GetCustomAttributes(true)))
                            {
                                str3 = str2;
                                string[] textArray2 = new string[] { str3, types[j].Name, ".", BuildFieldsString(ref fields[m]), "\n" };
                                str2 = string.Concat(textArray2);
                            }
                        }
                        PropertyInfo[] properties = types[j].GetProperties();
                        for (int n = 0; n < properties.Length; n++)
                        {
                            if ((((str == "*") || types[j].Name.Contains(str)) || properties[n].Name.Contains(str)) && arg.CheckPermissions(properties[n].GetCustomAttributes(true)))
                            {
                                str3 = str2;
                                string[] textArray3 = new string[] { str3, types[j].Name, ".", BuildPropertyString(ref properties[n]), "\n" };
                                str2 = string.Concat(textArray3);
                            }
                        }
                    }
                }
            }
            arg.ReplyWith("Finding " + str + ":\n" + str2);
        }
    }

    [Client, Help("Quits the game", ""), Admin]
    public static void quit(ref ConsoleSystem.Arg arg)
    {
        Application.Quit();
    }
}

