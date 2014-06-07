using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Sprite/Web")]
public class dfWebSprite : dfTextureSprite
{
    [SerializeField]
    protected Texture2D errorImage;
    [SerializeField]
    protected Texture2D loadingImage;
    [SerializeField]
    protected string url = string.Empty;

    [DebuggerHidden]
    private IEnumerator downloadTexture()
    {
        return new <downloadTexture>c__Iterator45 { <>f__this = this };
    }

    public override void Start()
    {
        base.Start();
        if (base.Texture == null)
        {
            base.Texture = this.LoadingImage;
        }
        if (Application.isPlaying)
        {
            base.StartCoroutine(this.downloadTexture());
        }
    }

    public Texture2D ErrorImage
    {
        get
        {
            return this.errorImage;
        }
        set
        {
            this.errorImage = value;
        }
    }

    public Texture2D LoadingImage
    {
        get
        {
            return this.loadingImage;
        }
        set
        {
            this.loadingImage = value;
        }
    }

    public string URL
    {
        get
        {
            return this.url;
        }
        set
        {
            if (value != this.url)
            {
                this.url = value;
                if (Application.isPlaying)
                {
                    base.StopAllCoroutines();
                    base.StartCoroutine(this.downloadTexture());
                }
            }
        }
    }

    [CompilerGenerated]
    private sealed class <downloadTexture>c__Iterator45 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal dfWebSprite <>f__this;
        internal WWW <request>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        if (this.<request>__0 != null)
                        {
                            this.<request>__0.Dispose();
                        }
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<>f__this.Texture = this.<>f__this.loadingImage;
                    this.<request>__0 = new WWW(this.<>f__this.url);
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_0111;
            }
            try
            {
                switch (num)
                {
                    case 1:
                        if (string.IsNullOrEmpty(this.<request>__0.error))
                        {
                            break;
                        }
                        Debug.Log("Error downloading image: " + this.<request>__0.error);
                        if (this.<>f__this.errorImage == null)
                        {
                        }
                        this.<>f__this.Texture = this.<>f__this.loadingImage;
                        goto Label_010A;

                    default:
                        this.$current = this.<request>__0;
                        this.$PC = 1;
                        flag = true;
                        return true;
                }
                this.<>f__this.Texture = this.<request>__0.texture;
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<request>__0 != null)
                {
                    this.<request>__0.Dispose();
                }
            }
        Label_010A:
            this.$PC = -1;
        Label_0111:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

