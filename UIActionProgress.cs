using System;
using UnityEngine;

public class UIActionProgress : MonoBehaviour
{
    [SerializeField]
    private UILabel _label;
    [SerializeField]
    private UISlider _slider;
    private UISprite[] sliderSprites;

    private void Awake()
    {
        this.sliderSprites = this._slider.GetComponentsInChildren<UISprite>();
    }

    private void OnDisable()
    {
        this.SetEnabled(false);
    }

    private void OnEnable()
    {
        this.SetEnabled(true);
    }

    private void SetEnabled(bool yes)
    {
        if (this._slider != null)
        {
            this._slider.enabled = yes;
        }
        if (this._label != null)
        {
            this._label.enabled = yes;
        }
        if (this.sliderSprites != null)
        {
            foreach (UISprite sprite in this.sliderSprites)
            {
                if (sprite != null)
                {
                    sprite.enabled = yes;
                }
            }
        }
    }

    public UILabel label
    {
        get
        {
            return this._label;
        }
    }

    public float progress
    {
        get
        {
            return this.slider.sliderValue;
        }
        set
        {
            this.slider.sliderValue = value;
        }
    }

    public UISlider slider
    {
        get
        {
            return this._slider;
        }
    }

    public string text
    {
        get
        {
            return this.label.text;
        }
        set
        {
            this.label.text = value;
        }
    }
}

