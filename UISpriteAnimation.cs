using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(UISprite)), AddComponentMenu("NGUI/UI/Sprite Animation")]
public class UISpriteAnimation : MonoBehaviour
{
    private float mDelta;
    [HideInInspector, SerializeField]
    private int mFPS = 30;
    private int mIndex;
    [SerializeField, HideInInspector]
    private string mPrefix = string.Empty;
    private UISprite mSprite;
    private List<string> mSpriteNames = new List<string>();

    private void RebuildSpriteList()
    {
        if (this.mSprite == null)
        {
            this.mSprite = base.GetComponent<UISprite>();
        }
        this.mSpriteNames.Clear();
        if ((this.mSprite != null) && (this.mSprite.atlas != null))
        {
            List<UIAtlas.Sprite> spriteList = this.mSprite.atlas.spriteList;
            int num = 0;
            int count = spriteList.Count;
            while (num < count)
            {
                UIAtlas.Sprite sprite = spriteList[num];
                if (string.IsNullOrEmpty(this.mPrefix) || sprite.name.StartsWith(this.mPrefix))
                {
                    this.mSpriteNames.Add(sprite.name);
                }
                num++;
            }
            this.mSpriteNames.Sort();
        }
    }

    private void Start()
    {
        this.RebuildSpriteList();
    }

    private void Update()
    {
        if ((this.mSpriteNames.Count > 1) && Application.isPlaying)
        {
            this.mDelta += Time.deltaTime;
            float num = (this.mFPS <= 0f) ? 0f : (1f / ((float) this.mFPS));
            if (num < this.mDelta)
            {
                this.mDelta = (num <= 0f) ? 0f : (this.mDelta - num);
                if (++this.mIndex >= this.mSpriteNames.Count)
                {
                    this.mIndex = 0;
                }
                this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
                this.mSprite.MakePixelPerfect();
            }
        }
    }

    public int framesPerSecond
    {
        get
        {
            return this.mFPS;
        }
        set
        {
            this.mFPS = value;
        }
    }

    public string namePrefix
    {
        get
        {
            return this.mPrefix;
        }
        set
        {
            if (this.mPrefix != value)
            {
                this.mPrefix = value;
                this.RebuildSpriteList();
            }
        }
    }
}

