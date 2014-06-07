using System;
using System.Collections.Generic;

public class SaveStringPool
{
    private static Dictionary<string, int> prefabDictionary;

    static SaveStringPool()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("StructureMasterPrefab", 1);
        prefabDictionary = dictionary;
    }

    public static string Convert(int iNum)
    {
        foreach (KeyValuePair<string, int> pair in prefabDictionary)
        {
            if (pair.Value == iNum)
            {
                return pair.Key;
            }
        }
        return string.Empty;
    }

    public static int GetInt(string strName)
    {
        return prefabDictionary[strName];
    }
}

