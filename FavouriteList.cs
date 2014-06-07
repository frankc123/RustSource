using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FavouriteList
{
    private static List<string> faveList = new List<string>();

    public static void Add(string strName)
    {
        if (!Contains(strName) && (strName.Length >= 8))
        {
            faveList.Add(strName);
        }
    }

    public static void Clear()
    {
        faveList.Clear();
    }

    public static bool Contains(string strName)
    {
        return faveList.Contains(strName);
    }

    public static void Load()
    {
        Clear();
        if (File.Exists("cfg/favourites.cfg"))
        {
            string str = File.ReadAllText("cfg/favourites.cfg");
            if (!string.IsNullOrEmpty(str))
            {
                Debug.Log("Running cfg/favourites.cfg");
                ConsoleSystem.RunFile(str);
            }
        }
    }

    public static bool Remove(string strName)
    {
        if (!Contains(strName))
        {
            return false;
        }
        return faveList.Remove(strName);
    }

    public static void Save()
    {
        string contents = string.Empty;
        if (!Directory.Exists("cfg"))
        {
            Directory.CreateDirectory("cfg");
        }
        foreach (string str2 in faveList)
        {
            contents = contents + "serverfavourite.add \"" + str2.ToString() + "\"\r\n";
            Debug.Log("serverfavourite.add \"" + str2.ToString() + "\"\r\n");
        }
        File.WriteAllText("cfg/favourites.cfg", contents);
        Debug.Log(contents);
    }
}

