using System;
using System.Reflection;
using UnityEngine;

public class TerrainMap : ScriptableObject
{
    [SerializeField]
    private string[] _guids;
    [SerializeField]
    private int _height;
    [SerializeField]
    private int _width;
    public float baseHeight;
    public Terrain copyFrom;
    public TerrainData root;
    public Vector3 scale;

    public void ResizeGUIDS(int width, int height)
    {
        int a = this._width;
        int num2 = this._height;
        if ((a != width) || (num2 != height))
        {
            string[] strArray = this._guids;
            this._guids = new string[width * height];
            this._width = width;
            this._height = height;
            int num3 = Mathf.Min(a, width);
            int num4 = Mathf.Min(num2, height);
            for (int i = 0; i < num4; i++)
            {
                for (int j = 0; j < num3; j++)
                {
                    this._guids[(i * this._width) + j] = strArray[(i * a) + j];
                }
            }
        }
    }

    public int count
    {
        get
        {
            return (this._width * this._height);
        }
    }

    public int height
    {
        get
        {
            return this._height;
        }
    }

    public string this[int i]
    {
        get
        {
            return this._guids[i];
        }
        set
        {
            this._guids[i] = value;
        }
    }

    public string this[int x, int y]
    {
        get
        {
            return this[(y * this._width) + x];
        }
        set
        {
            this[(y * this._width) + x] = value;
        }
    }

    public int width
    {
        get
        {
            return this._width;
        }
    }
}

