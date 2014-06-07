using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Texture Atlas"), ExecuteInEditMode]
public class dfAtlas : MonoBehaviour
{
    [SerializeField]
    protected List<ItemInfo> items = new List<ItemInfo>();
    private Dictionary<string, ItemInfo> map = new Dictionary<string, ItemInfo>();
    [SerializeField]
    protected UnityEngine.Material material;
    private dfAtlas replacementAtlas;

    public void AddItem(ItemInfo item)
    {
        this.items.Add(item);
        this.RebuildIndexes();
    }

    public void AddItems(IEnumerable<ItemInfo> items)
    {
        this.items.AddRange(items);
        this.RebuildIndexes();
    }

    internal static bool Equals(dfAtlas lhs, dfAtlas rhs)
    {
        return (object.ReferenceEquals(lhs, rhs) || (((lhs != null) && (rhs != null)) && (lhs.material == rhs.material)));
    }

    public void RebuildIndexes()
    {
        if (this.map == null)
        {
            this.map = new Dictionary<string, ItemInfo>();
        }
        else
        {
            this.map.Clear();
        }
        for (int i = 0; i < this.items.Count; i++)
        {
            ItemInfo info = this.items[i];
            this.map[info.name] = info;
        }
    }

    public void Remove(string name)
    {
        for (int i = this.items.Count - 1; i >= 0; i--)
        {
            if (this.items[i].name == name)
            {
                this.items.RemoveAt(i);
            }
        }
        this.RebuildIndexes();
    }

    public int Count
    {
        get
        {
            return ((this.replacementAtlas == null) ? this.items.Count : this.replacementAtlas.Count);
        }
    }

    public ItemInfo this[string key]
    {
        get
        {
            if (this.replacementAtlas != null)
            {
                return this.replacementAtlas[key];
            }
            if (!string.IsNullOrEmpty(key))
            {
                if (this.map.Count == 0)
                {
                    this.RebuildIndexes();
                }
                ItemInfo info = null;
                if (this.map.TryGetValue(key, out info))
                {
                    return info;
                }
            }
            return null;
        }
    }

    public List<ItemInfo> Items
    {
        get
        {
            return ((this.replacementAtlas == null) ? this.items : this.replacementAtlas.Items);
        }
    }

    public UnityEngine.Material Material
    {
        get
        {
            return ((this.replacementAtlas == null) ? this.material : this.replacementAtlas.Material);
        }
        set
        {
            if (this.replacementAtlas != null)
            {
                this.replacementAtlas.Material = value;
            }
            else
            {
                this.material = value;
            }
        }
    }

    public dfAtlas Replacement
    {
        get
        {
            return this.replacementAtlas;
        }
        set
        {
            this.replacementAtlas = value;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return ((this.replacementAtlas == null) ? (this.material.mainTexture as Texture2D) : this.replacementAtlas.Texture);
        }
    }

    [Serializable]
    public class ItemInfo : IComparable<dfAtlas.ItemInfo>, IEquatable<dfAtlas.ItemInfo>
    {
        public RectOffset border = new RectOffset();
        public bool deleted;
        public string name;
        public Rect region;
        public bool rotated;
        public Vector2 sizeInPixels = Vector2.zero;
        [SerializeField]
        public Texture2D texture;
        [SerializeField]
        public string textureGUID = string.Empty;

        public int CompareTo(dfAtlas.ItemInfo other)
        {
            return this.name.CompareTo(other.name);
        }

        public bool Equals(dfAtlas.ItemInfo other)
        {
            return this.name.Equals(other.name);
        }

        public override bool Equals(object obj)
        {
            return ((obj is dfAtlas.ItemInfo) && this.name.Equals(((dfAtlas.ItemInfo) obj).name));
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public static bool operator ==(dfAtlas.ItemInfo lhs, dfAtlas.ItemInfo rhs)
        {
            return (object.ReferenceEquals(lhs, rhs) || (((lhs != null) && (rhs != null)) && lhs.name.Equals(rhs.name)));
        }

        public static bool operator !=(dfAtlas.ItemInfo lhs, dfAtlas.ItemInfo rhs)
        {
            return !(lhs == rhs);
        }
    }
}

