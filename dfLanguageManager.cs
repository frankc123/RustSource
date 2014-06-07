using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class dfLanguageManager : MonoBehaviour
{
    [SerializeField]
    private dfLanguageCode currentLanguage;
    [SerializeField]
    private TextAsset dataFile;
    private Dictionary<string, string> strings = new Dictionary<string, string>();

    public string GetValue(string key)
    {
        string str = string.Empty;
        if (this.strings.TryGetValue(key, out str))
        {
            return str;
        }
        return key;
    }

    public void LoadLanguage(dfLanguageCode language)
    {
        this.currentLanguage = language;
        this.strings.Clear();
        if (this.dataFile != null)
        {
            this.parseDataFile();
        }
        dfControl[] componentsInChildren = base.GetComponentsInChildren<dfControl>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].Localize();
        }
    }

    private void parseDataFile()
    {
        string data = this.dataFile.text.Replace("\r\n", "\n").Trim();
        List<string> values = new List<string>();
        int index = this.parseLine(data, values, 0);
        int num2 = values.IndexOf(this.currentLanguage.ToString());
        if (num2 >= 0)
        {
            List<string> list2 = new List<string>();
            while (index < data.Length)
            {
                index = this.parseLine(data, list2, index);
                if (list2.Count != 0)
                {
                    string str2 = list2[0];
                    string str3 = (num2 >= list2.Count) ? string.Empty : list2[num2];
                    this.strings[str2] = str3;
                }
            }
        }
    }

    private int parseLine(string data, List<string> values, int index)
    {
        values.Clear();
        bool flag = false;
        StringBuilder builder = new StringBuilder(0x100);
        while (index < data.Length)
        {
            char ch = data[index];
            if (ch == '"')
            {
                if (!flag)
                {
                    flag = true;
                }
                else if (((index + 1) < data.Length) && (data[index + 1] == ch))
                {
                    index++;
                    builder.Append(ch);
                }
                else
                {
                    flag = false;
                }
            }
            else if (ch == ',')
            {
                if (flag)
                {
                    builder.Append(ch);
                }
                else
                {
                    values.Add(builder.ToString());
                    builder.Length = 0;
                }
            }
            else
            {
                if (ch == '\n')
                {
                    if (flag)
                    {
                        builder.Append(ch);
                        goto Label_00D0;
                    }
                    index++;
                    break;
                }
                builder.Append(ch);
            }
        Label_00D0:
            index++;
        }
        if (builder.Length > 0)
        {
            values.Add(builder.ToString());
        }
        return index;
    }

    public void Start()
    {
        dfLanguageCode currentLanguage = this.currentLanguage;
        if (this.currentLanguage == dfLanguageCode.None)
        {
            currentLanguage = this.SystemLanguageToLanguageCode(Application.systemLanguage);
        }
        this.LoadLanguage(currentLanguage);
    }

    private dfLanguageCode SystemLanguageToLanguageCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Afrikaans:
                return dfLanguageCode.AF;

            case SystemLanguage.Arabic:
                return dfLanguageCode.AR;

            case SystemLanguage.Basque:
                return dfLanguageCode.EU;

            case SystemLanguage.Belarusian:
                return dfLanguageCode.BE;

            case SystemLanguage.Bulgarian:
                return dfLanguageCode.BG;

            case SystemLanguage.Catalan:
                return dfLanguageCode.CA;

            case SystemLanguage.Chinese:
                return dfLanguageCode.ZH;

            case SystemLanguage.Czech:
                return dfLanguageCode.CS;

            case SystemLanguage.Danish:
                return dfLanguageCode.DA;

            case SystemLanguage.Dutch:
                return dfLanguageCode.NL;

            case SystemLanguage.English:
                return dfLanguageCode.EN;

            case SystemLanguage.Estonian:
                return dfLanguageCode.ES;

            case SystemLanguage.Faroese:
                return dfLanguageCode.FO;

            case SystemLanguage.Finnish:
                return dfLanguageCode.FI;

            case SystemLanguage.French:
                return dfLanguageCode.FR;

            case SystemLanguage.German:
                return dfLanguageCode.DE;

            case SystemLanguage.Greek:
                return dfLanguageCode.EL;

            case SystemLanguage.Hebrew:
                return dfLanguageCode.HE;

            case SystemLanguage.Hugarian:
                return dfLanguageCode.HU;

            case SystemLanguage.Icelandic:
                return dfLanguageCode.IS;

            case SystemLanguage.Indonesian:
                return dfLanguageCode.ID;

            case SystemLanguage.Italian:
                return dfLanguageCode.IT;

            case SystemLanguage.Japanese:
                return dfLanguageCode.JA;

            case SystemLanguage.Korean:
                return dfLanguageCode.KO;

            case SystemLanguage.Latvian:
                return dfLanguageCode.LV;

            case SystemLanguage.Lithuanian:
                return dfLanguageCode.LT;

            case SystemLanguage.Norwegian:
                return dfLanguageCode.NO;

            case SystemLanguage.Polish:
                return dfLanguageCode.PL;

            case SystemLanguage.Portuguese:
                return dfLanguageCode.PT;

            case SystemLanguage.Romanian:
                return dfLanguageCode.RO;

            case SystemLanguage.Russian:
                return dfLanguageCode.RU;

            case SystemLanguage.SerboCroatian:
                return dfLanguageCode.SH;

            case SystemLanguage.Slovak:
                return dfLanguageCode.SK;

            case SystemLanguage.Slovenian:
                return dfLanguageCode.SL;

            case SystemLanguage.Spanish:
                return dfLanguageCode.ES;

            case SystemLanguage.Swedish:
                return dfLanguageCode.SV;

            case SystemLanguage.Thai:
                return dfLanguageCode.TH;

            case SystemLanguage.Turkish:
                return dfLanguageCode.TR;

            case SystemLanguage.Ukrainian:
                return dfLanguageCode.UK;

            case SystemLanguage.Vietnamese:
                return dfLanguageCode.VI;

            case SystemLanguage.Unknown:
                return dfLanguageCode.EN;
        }
        throw new ArgumentException("Unknown system language: " + language);
    }

    public dfLanguageCode CurrentLanguage
    {
        get
        {
            return this.currentLanguage;
        }
    }

    public TextAsset DataFile
    {
        get
        {
            return this.dataFile;
        }
        set
        {
            if (value != this.dataFile)
            {
                this.dataFile = value;
                this.LoadLanguage(this.currentLanguage);
            }
        }
    }
}

