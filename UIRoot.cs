using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Root")]
public class UIRoot : MonoBehaviour
{
    public bool automatic = true;
    public int manualHeight = 800;
    private static List<UIRoot> mRoots = new List<UIRoot>();
    private Transform mTrans;

    private void Awake()
    {
        mRoots.Add(this);
    }

    public static void Broadcast(string funcName)
    {
        int num = 0;
        int count = mRoots.Count;
        while (num < count)
        {
            UIRoot root = mRoots[num];
            if (root != null)
            {
                root.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
            }
            num++;
        }
    }

    public static void Broadcast(string funcName, object param)
    {
        if (param == null)
        {
            Debug.LogError("SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
        }
        else
        {
            int num = 0;
            int count = mRoots.Count;
            while (num < count)
            {
                UIRoot root = mRoots[num];
                if (root != null)
                {
                    root.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
                }
                num++;
            }
        }
    }

    private void OnDestroy()
    {
        mRoots.Remove(this);
    }

    private void Start()
    {
        this.mTrans = base.transform;
        UIOrthoCamera componentInChildren = base.GetComponentInChildren<UIOrthoCamera>();
        if (componentInChildren != null)
        {
            Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", componentInChildren);
            Camera component = componentInChildren.gameObject.GetComponent<Camera>();
            componentInChildren.enabled = false;
            if (component != null)
            {
                component.orthographicSize = 1f;
            }
        }
    }

    private void Update()
    {
        this.manualHeight = Mathf.Max(2, !this.automatic ? this.manualHeight : Screen.height);
        float x = 2f / ((float) this.manualHeight);
        Vector3 localScale = this.mTrans.localScale;
        if (((Mathf.Abs((float) (localScale.x - x)) > float.Epsilon) || (Mathf.Abs((float) (localScale.y - x)) > float.Epsilon)) || (Mathf.Abs((float) (localScale.z - x)) > float.Epsilon))
        {
            this.mTrans.localScale = new Vector3(x, x, x);
        }
    }
}

