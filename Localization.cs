using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
    public TextAsset[] languages;
    private Dictionary<string, string> mDictionary = new Dictionary<string, string>();
    private static Localization mInst;
    private string mLanguage;
    public string startingLanguage;

    private void Awake()
    {
        if (mInst == null)
        {
            mInst = this;
            Object.DontDestroyOnLoad(base.gameObject);
        }
        else
        {
            Object.Destroy(base.gameObject);
        }
    }

    public string Get(string key)
    {
        string str;
        return (!this.mDictionary.TryGetValue(key, out str) ? key : str);
    }

    private void Load(TextAsset asset)
    {
        this.mLanguage = asset.name;
        PlayerPrefs.SetString("Language", this.mLanguage);
        this.mDictionary = new ByteReader(asset).ReadDictionary();
        UIRoot.Broadcast("OnLocalize", this);
    }

    private void OnDestroy()
    {
        if (mInst == this)
        {
            mInst = null;
        }
    }

    private void OnEnable()
    {
        if (mInst == null)
        {
            mInst = this;
        }
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(this.startingLanguage))
        {
            this.currentLanguage = this.startingLanguage;
        }
    }

    public string currentLanguage
    {
        get
        {
            if (string.IsNullOrEmpty(this.mLanguage))
            {
                this.currentLanguage = PlayerPrefs.GetString("Language");
                if (string.IsNullOrEmpty(this.mLanguage))
                {
                    this.currentLanguage = this.startingLanguage;
                    if ((string.IsNullOrEmpty(this.mLanguage) && (this.languages != null)) && (this.languages.Length > 0))
                    {
                        this.currentLanguage = this.languages[0].name;
                    }
                }
            }
            return this.mLanguage;
        }
        set
        {
            if (this.mLanguage != value)
            {
                this.startingLanguage = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (this.languages != null)
                    {
                        int index = 0;
                        int length = this.languages.Length;
                        while (index < length)
                        {
                            TextAsset asset = this.languages[index];
                            if ((asset != null) && (asset.name == value))
                            {
                                this.Load(asset);
                                return;
                            }
                            index++;
                        }
                    }
                    TextAsset asset2 = Resources.Load(value, typeof(TextAsset)) as TextAsset;
                    if (asset2 != null)
                    {
                        this.Load(asset2);
                        return;
                    }
                }
                this.mDictionary.Clear();
                PlayerPrefs.DeleteKey("Language");
            }
        }
    }

    public static Localization instance
    {
        get
        {
            if (mInst == null)
            {
                mInst = Object.FindObjectOfType(typeof(Localization)) as Localization;
                if (mInst == null)
                {
                    GameObject target = new GameObject("_Localization");
                    Object.DontDestroyOnLoad(target);
                    mInst = target.AddComponent<Localization>();
                }
            }
            return mInst;
        }
    }
}

