using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
    public string keyName;

    private void OnDisable()
    {
        this.Save(null);
    }

    private void OnEnable()
    {
        string str = PlayerPrefs.GetString(this.key);
        if (!string.IsNullOrEmpty(str))
        {
            UICheckbox component = base.GetComponent<UICheckbox>();
            if (component != null)
            {
                component.isChecked = str == "true";
            }
            else
            {
                UICheckbox[] componentsInChildren = base.GetComponentsInChildren<UICheckbox>();
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    UICheckbox checkbox2 = componentsInChildren[index];
                    UIEventListener listener1 = UIEventListener.Get(checkbox2.gameObject);
                    listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Remove(listener1.onClick, new UIEventListener.VoidDelegate(this.Save));
                    checkbox2.isChecked = checkbox2.name == str;
                    Debug.Log(str);
                    UIEventListener listener2 = UIEventListener.Get(checkbox2.gameObject);
                    listener2.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener2.onClick, new UIEventListener.VoidDelegate(this.Save));
                    index++;
                }
            }
        }
    }

    private void Save(GameObject go)
    {
        UICheckbox component = base.GetComponent<UICheckbox>();
        if (component != null)
        {
            PlayerPrefs.SetString(this.key, !component.isChecked ? "false" : "true");
        }
        else
        {
            UICheckbox[] componentsInChildren = base.GetComponentsInChildren<UICheckbox>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                UICheckbox checkbox2 = componentsInChildren[index];
                if (checkbox2.isChecked)
                {
                    PlayerPrefs.SetString(this.key, checkbox2.name);
                    break;
                }
                index++;
            }
        }
    }

    private string key
    {
        get
        {
            return (!string.IsNullOrEmpty(this.keyName) ? this.keyName : ("NGUI State: " + base.name));
        }
    }
}

