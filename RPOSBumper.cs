using System;
using System.Collections.Generic;
using UnityEngine;

public class RPOSBumper : MonoBehaviour
{
    public UISlicedSprite background;
    public UIButton buttonPrefab;
    private HashSet<Instance> instances;

    private void Clear()
    {
        if (this.instances != null)
        {
            foreach (Instance instance in this.instances)
            {
                if (instance.window != null)
                {
                    instance.window.RemoveBumper(instance);
                }
            }
            this.instances.Clear();
        }
    }

    private void OnDestroy()
    {
        this.Clear();
    }

    public void Populate()
    {
        this.Clear();
        List<RPOSWindow> list = new List<RPOSWindow>(RPOS.GetBumperWindowList());
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            if ((list[i] != null) && !string.IsNullOrEmpty(list[i].title))
            {
                list[i].EnsureAwake<RPOSWindow>();
            }
            else
            {
                list.RemoveAt(i--);
                count--;
            }
        }
        float num3 = 75f * this.buttonPrefab.gameObject.transform.localScale.x;
        float num4 = 5f;
        float num5 = (count * num3) * -0.5f;
        int num6 = 0;
        if (this.instances == null)
        {
            this.instances = new HashSet<Instance>();
        }
        foreach (RPOSWindow window in list)
        {
            Instance inst = new Instance {
                window = window
            };
            Vector3 vector = this.buttonPrefab.gameObject.transform.localScale;
            GameObject obj2 = NGUITools.AddChild(base.gameObject, this.buttonPrefab.gameObject);
            inst.label = obj2.gameObject.GetComponentInChildren<UILabel>();
            inst.label.name = window.title + "BumperButton";
            Vector3 localPosition = obj2.transform.localPosition;
            localPosition.x = num5 + ((num3 + num4) * num6);
            obj2.transform.localPosition = localPosition;
            obj2.transform.localScale = vector;
            inst.button = obj2.GetComponentInChildren<UIButton>();
            inst.bumper = this;
            window.AddBumper(inst);
            this.instances.Add(inst);
            num6++;
        }
        Vector3 localScale = this.background.transform.localScale;
        localScale.x = (count * num3) + (count - (1f * num4));
        this.background.gameObject.transform.localScale = localScale;
        this.background.gameObject.transform.localPosition = new Vector3(localScale.x * -0.5f, base.transform.localPosition.y, 0f);
    }

    public class Instance
    {
        private UIEventListener _listener;
        public RPOSBumper bumper;
        public UIButton button;
        public UILabel label;
        private bool onceGetListener;
        public RPOSWindow window;

        public UIEventListener listener
        {
            get
            {
                if (!this.onceGetListener)
                {
                    this.onceGetListener = true;
                    if (this.button != null)
                    {
                        this._listener = UIEventListener.Get(this.button.gameObject);
                    }
                }
                return this._listener;
            }
        }
    }
}

