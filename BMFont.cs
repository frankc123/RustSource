using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class BMFont
{
    [SerializeField, HideInInspector]
    private int mBase;
    [NonSerialized]
    private Dictionary<int, BMGlyph> mDict;
    [NonSerialized]
    private bool mDictAny;
    [NonSerialized]
    private bool mDictMade;
    [SerializeField, HideInInspector]
    private BMGlyph[] mGlyphs;
    [SerializeField, HideInInspector]
    private int mHeight;
    [HideInInspector, SerializeField]
    private List<BMGlyph> mSaved = new List<BMGlyph>();
    [HideInInspector, SerializeField]
    private int mSize;
    [SerializeField, HideInInspector]
    private string mSpriteName;
    [HideInInspector, SerializeField]
    private List<BMSymbol> mSymbols = new List<BMSymbol>();
    [HideInInspector, SerializeField]
    private int mWidth;

    public void Clear()
    {
        this.mGlyphs = null;
        this.mDict = null;
        this.mDictAny = this.mDictMade = false;
        this.mSaved.Clear();
    }

    public bool ContainsGlyph(int index)
    {
        if (!this.mDictMade)
        {
            this.mDictMade = true;
            int count = this.mSaved.Count;
            if ((count == 0) && this.LegacyCheck())
            {
                count = this.mSaved.Count;
            }
            this.mDictAny = count > 0;
            if (this.mDictAny)
            {
                this.mDict = CreateGlyphDictionary(count);
                for (int i = count - 1; i >= 0; i--)
                {
                    BMGlyph glyph = this.mSaved[i];
                    this.mDict.Add(glyph.index, glyph);
                    if (glyph.index == index)
                    {
                        while (--i >= 0)
                        {
                            glyph = this.mSaved[i];
                            this.mDict.Add(glyph.index, glyph);
                        }
                        return true;
                    }
                }
            }
        }
        else if (this.mDictAny && this.mDict.ContainsKey(index))
        {
            return true;
        }
        return false;
    }

    private static Dictionary<int, BMGlyph> CreateGlyphDictionary()
    {
        return new Dictionary<int, BMGlyph>();
    }

    private static Dictionary<int, BMGlyph> CreateGlyphDictionary(int cap)
    {
        return new Dictionary<int, BMGlyph>(cap);
    }

    private int GetArraySize(int index)
    {
        if (index < 0x100)
        {
            return 0x100;
        }
        if (index < 0x10000)
        {
            return 0x10000;
        }
        if (index < 0x40000)
        {
            return 0x40000;
        }
        return 0;
    }

    public bool GetGlyph(int index, out BMGlyph glyph)
    {
        if (!this.mDictMade)
        {
            this.mDictMade = true;
            int count = this.mSaved.Count;
            if ((count == 0) && this.LegacyCheck())
            {
                count = this.mSaved.Count;
            }
            this.mDictAny = count > 0;
            if (this.mDictAny)
            {
                this.mDict = CreateGlyphDictionary(count);
                for (int i = count - 1; i >= 0; i--)
                {
                    BMGlyph glyph2 = this.mSaved[i];
                    this.mDict.Add(glyph2.index, glyph2);
                    if (glyph2.index == index)
                    {
                        glyph = glyph2;
                        while (--i >= 0)
                        {
                            glyph2 = this.mSaved[i];
                            this.mDict.Add(glyph2.index, glyph2);
                        }
                        return true;
                    }
                }
            }
        }
        else if (this.mDictAny)
        {
            return this.mDict.TryGetValue(index, out glyph);
        }
        glyph = null;
        return false;
    }

    public GetOrCreateGlyphResult GetOrCreateGlyph(int index, out BMGlyph glyph)
    {
        if (!this.mDictMade)
        {
            this.mDictMade = true;
            this.mDictAny = true;
            int count = this.mSaved.Count;
            if ((count == 0) && this.LegacyCheck())
            {
                count = this.mSaved.Count;
            }
            if (count > 0)
            {
                this.mDict = CreateGlyphDictionary(count + 1);
                for (int i = count - 1; i >= 0; i--)
                {
                    BMGlyph glyph2 = this.mSaved[i];
                    this.mDict.Add(glyph2.index, glyph2);
                    if (glyph2.index == index)
                    {
                        glyph = glyph2;
                        while (--i >= 0)
                        {
                            glyph2 = this.mSaved[i];
                            this.mDict.Add(glyph2.index, glyph2);
                        }
                        return GetOrCreateGlyphResult.Found;
                    }
                }
            }
            else
            {
                this.mDict = CreateGlyphDictionary();
            }
        }
        else if (this.mDictAny)
        {
            if (this.mDict.TryGetValue(index, out glyph))
            {
                return GetOrCreateGlyphResult.Found;
            }
        }
        else
        {
            this.mDict = CreateGlyphDictionary();
            this.mDictAny = true;
        }
        BMGlyph glyph3 = new BMGlyph {
            index = index
        };
        glyph = glyph3;
        this.mDict.Add(index, glyph);
        return GetOrCreateGlyphResult.Created;
    }

    public BMSymbol GetSymbol(string sequence, bool createIfMissing)
    {
        int num = 0;
        int count = this.mSymbols.Count;
        while (num < count)
        {
            BMSymbol symbol = this.mSymbols[num];
            if (symbol.sequence == sequence)
            {
                return symbol;
            }
            num++;
        }
        if (createIfMissing)
        {
            BMSymbol item = new BMSymbol {
                sequence = sequence
            };
            this.mSymbols.Add(item);
            return item;
        }
        return null;
    }

    public bool LegacyCheck()
    {
        if ((this.mGlyphs != null) && (this.mGlyphs.Length > 0))
        {
            int index = 0;
            int length = this.mGlyphs.Length;
            while (index < length)
            {
                BMGlyph item = this.mGlyphs[index];
                if (item != null)
                {
                    item.index = index;
                    this.mSaved.Add(item);
                    while (++index < length)
                    {
                        if (item != null)
                        {
                            item.index = index;
                            this.mSaved.Add(item);
                        }
                    }
                    this.mGlyphs = null;
                    return true;
                }
                index++;
            }
            this.mGlyphs = null;
        }
        return false;
    }

    public bool MatchSymbol(string text, int offset, int textLength, out BMSymbol symbol)
    {
        int count = this.mSymbols.Count;
        if (count > 0)
        {
            textLength -= offset;
            if (textLength > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    BMSymbol symbol2 = this.mSymbols[i];
                    int length = symbol2.sequence.Length;
                    if (((length != 0) && (textLength >= length)) && (string.Compare(symbol2.sequence, 0, text, offset, length) == 0))
                    {
                        symbol = symbol2;
                        if ((length < textLength) && (++i < count))
                        {
                            int num4 = length;
                            do
                            {
                                symbol2 = this.mSymbols[i];
                                length = symbol2.sequence.Length;
                                if (((textLength >= length) && (length > num4)) && (string.Compare(symbol2.sequence, 0, text, offset, length) == 0))
                                {
                                    num4 = length;
                                    symbol = symbol2;
                                }
                            }
                            while (++i < count);
                        }
                        return true;
                    }
                }
            }
        }
        symbol = null;
        return false;
    }

    public void Trim(int xMin, int yMin, int xMax, int yMax)
    {
        if (this.isValid)
        {
            int num = 0;
            int count = this.mSaved.Count;
            while (num < count)
            {
                BMGlyph glyph = this.mSaved[num];
                if (glyph != null)
                {
                    glyph.Trim(xMin, yMin, xMax, yMax);
                }
                num++;
            }
        }
    }

    public int baseOffset
    {
        get
        {
            return this.mBase;
        }
        set
        {
            this.mBase = value;
        }
    }

    public int charSize
    {
        get
        {
            return this.mSize;
        }
        set
        {
            this.mSize = value;
        }
    }

    public int glyphCount
    {
        get
        {
            return (!this.isValid ? 0 : this.mSaved.Count);
        }
    }

    public bool isValid
    {
        get
        {
            return ((this.mSaved.Count > 0) || this.LegacyCheck());
        }
    }

    public string spriteName
    {
        get
        {
            return this.mSpriteName;
        }
        set
        {
            this.mSpriteName = value;
        }
    }

    public List<BMSymbol> symbols
    {
        get
        {
            return this.mSymbols;
        }
    }

    public int texHeight
    {
        get
        {
            return this.mHeight;
        }
        set
        {
            this.mHeight = value;
        }
    }

    public int texWidth
    {
        get
        {
            return this.mWidth;
        }
        set
        {
            this.mWidth = value;
        }
    }

    public enum GetOrCreateGlyphResult : sbyte
    {
        Created = 1,
        Failed = 0,
        Found = -1
    }
}

