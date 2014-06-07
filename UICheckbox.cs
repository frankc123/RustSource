using AnimationOrTween;
using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox")]
public class UICheckbox : MonoBehaviour
{
    public Animation checkAnimation;
    public UISprite checkSprite;
    public static UICheckbox current;
    public GameObject eventReceiver;
    public string functionName = "OnActivate";
    private bool mChecked = true;
    private bool mStarted;
    private Transform mTrans;
    [HideInInspector, SerializeField]
    private bool option;
    public bool optionCanBeNone;
    public Transform radioButtonRoot;
    public bool startsChecked = true;

    private void Awake()
    {
        this.mTrans = base.transform;
        if (this.checkSprite != null)
        {
            this.checkSprite.alpha = !this.startsChecked ? 0f : 1f;
        }
        if (this.option)
        {
            this.option = false;
            if (this.radioButtonRoot == null)
            {
                this.radioButtonRoot = this.mTrans.parent;
            }
        }
    }

    private void OnClick()
    {
        if (base.enabled)
        {
            this.isChecked = !this.isChecked;
        }
    }

    private void Set(bool state)
    {
        if (!this.mStarted)
        {
            this.mChecked = state;
            this.startsChecked = state;
            if (this.checkSprite != null)
            {
                this.checkSprite.alpha = !state ? 0f : 1f;
            }
        }
        else if (this.mChecked != state)
        {
            if ((this.radioButtonRoot != null) && state)
            {
                UICheckbox[] componentsInChildren = this.radioButtonRoot.GetComponentsInChildren<UICheckbox>(true);
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    UICheckbox checkbox = componentsInChildren[index];
                    if ((checkbox != this) && (checkbox.radioButtonRoot == this.radioButtonRoot))
                    {
                        checkbox.Set(false);
                    }
                    index++;
                }
            }
            this.mChecked = state;
            if (this.checkSprite != null)
            {
                Color color = this.checkSprite.color;
                color.a = !this.mChecked ? 0f : 1f;
                TweenColor.Begin(this.checkSprite.gameObject, 0.2f, color);
            }
            if ((this.eventReceiver != null) && !string.IsNullOrEmpty(this.functionName))
            {
                current = this;
                this.eventReceiver.SendMessage(this.functionName, this.mChecked, SendMessageOptions.DontRequireReceiver);
            }
            if (this.checkAnimation != null)
            {
                ActiveAnimation.Play(this.checkAnimation, !state ? Direction.Reverse : Direction.Forward);
            }
        }
    }

    private void Start()
    {
        if (this.eventReceiver == null)
        {
            this.eventReceiver = base.gameObject;
        }
        this.mChecked = !this.startsChecked;
        this.mStarted = true;
        this.Set(this.startsChecked);
    }

    public bool isChecked
    {
        get
        {
            return this.mChecked;
        }
        set
        {
            if (((this.radioButtonRoot == null) || value) || (this.optionCanBeNone || !this.mStarted))
            {
                this.Set(value);
            }
        }
    }
}

