using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize"), RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
    public string key;
    private string mLanguage;
    private bool mStarted;

    private void OnEnable()
    {
        if (this.mStarted && (Localization.instance != null))
        {
            this.OnLocalize(Localization.instance);
        }
    }

    private void OnLocalize(Localization loc)
    {
        if (this.mLanguage != loc.currentLanguage)
        {
            UIWidget component = base.GetComponent<UIWidget>();
            UILabel label = component as UILabel;
            UISprite sprite = component as UISprite;
            if ((string.IsNullOrEmpty(this.mLanguage) && string.IsNullOrEmpty(this.key)) && (label != null))
            {
                this.key = label.text;
            }
            string str = !string.IsNullOrEmpty(this.key) ? loc.Get(this.key) : loc.Get(component.name);
            if (label != null)
            {
                label.text = str;
            }
            else if (sprite != null)
            {
                sprite.spriteName = str;
                sprite.MakePixelPerfect();
            }
            this.mLanguage = loc.currentLanguage;
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (Localization.instance != null)
        {
            this.OnLocalize(Localization.instance);
        }
    }
}

