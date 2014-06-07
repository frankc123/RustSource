using System;
using UnityEngine;

public class WeightSelection
{
    public static object RandomPick(WeightedEntry[] array)
    {
        return RandomPickEntry(array).obj;
    }

    public static T RandomPick<T>(WeightedEntry<T>[] array)
    {
        return RandomPickEntry<T>(array).obj;
    }

    public static WeightedEntry RandomPickEntry(WeightedEntry[] array)
    {
        float max = 0f;
        foreach (WeightedEntry entry in array)
        {
            max += entry.weight;
        }
        if (max == 0f)
        {
            return null;
        }
        float num3 = Random.Range(0f, max);
        foreach (WeightedEntry entry2 in array)
        {
            num3 -= entry2.weight;
            if (num3 <= 0f)
            {
                return entry2;
            }
        }
        return array[array.Length - 1];
    }

    public static WeightedEntry<T> RandomPickEntry<T>(WeightedEntry<T>[] array)
    {
        float max = 0f;
        foreach (WeightedEntry<T> entry in array)
        {
            max += entry.weight;
        }
        if (max == 0f)
        {
            return null;
        }
        float num3 = Random.Range(0f, max);
        foreach (WeightedEntry<T> entry2 in array)
        {
            num3 -= entry2.weight;
            if (num3 <= 0f)
            {
                return entry2;
            }
        }
        return array[array.Length - 1];
    }

    [Serializable]
    public class WeightedEntry
    {
        public Object obj;
        public float weight;
    }

    [Serializable]
    public class WeightedEntry<T>
    {
        public T obj;
        public float weight;
    }
}

