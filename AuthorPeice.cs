using System;
using UnityEngine;

public class AuthorPeice : AuthorShared
{
    [SerializeField]
    private AuthorCreation _creation;
    [SerializeField]
    private string _peiceID;
    private bool destroyed;

    protected static bool ActionButton(AuthorShared.Content content, ref AuthorShared.PeiceAction act, bool isSelected, AuthorShared.PeiceAction action, GUIStyle style, params GUILayoutOption[] options)
    {
        if (AuthorShared.Toggle(content, isSelected, style, options) != isSelected)
        {
            act = action;
            return true;
        }
        return false;
    }

    protected static bool ActionButton(AuthorShared.Content content, ref AuthorShared.PeiceAction act, bool isSelected, AuthorShared.PeiceAction onAction, AuthorShared.PeiceAction offAction, GUIStyle style, params GUILayoutOption[] options)
    {
        if (AuthorShared.Toggle(content, isSelected, style, options) != isSelected)
        {
            act = !isSelected ? onAction : offAction;
            return true;
        }
        return false;
    }

    public void Delete()
    {
        if (!this.destroyed)
        {
            try
            {
                this.OnPeiceDestroy();
            }
            finally
            {
                this.destroyed = true;
                Object.DestroyImmediate(this);
            }
        }
    }

    protected string FromRootBonePath(Transform transform)
    {
        if (this.creation != null)
        {
            return this.creation.RootBonePath(this, transform);
        }
        return string.Empty;
    }

    protected virtual void OnDidUnRegister()
    {
    }

    public virtual void OnListClicked()
    {
        if (!AuthorShared.SelectionContains(this.selectReference) && AuthorShared.SelectionContains(this))
        {
        }
    }

    protected virtual void OnPeiceDestroy()
    {
        if (this._creation != null)
        {
            this.OnWillUnRegister();
            this._creation.UnregisterPeice(this);
            this.OnDidUnRegister();
        }
    }

    protected virtual void OnRegistered()
    {
    }

    public virtual bool OnSceneView()
    {
        return false;
    }

    protected virtual void OnWillUnRegister()
    {
    }

    public virtual bool PeiceInspectorGUI()
    {
        AuthorShared.BeginHorizontal(AuthorShared.Styles.gradientInlineFill, new GUILayoutOption[0]);
        GUILayout.Space(48f);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) };
        if (GUILayout.Button(AuthorShared.ObjectContent<Transform>(base.transform, typeof(Transform)).image, options))
        {
            AuthorShared.PingObject(this);
        }
        GUILayout.Space(10f);
        GUILayout.Label(this.peiceID, AuthorShared.Styles.boldLabel, new GUILayoutOption[0]);
        GUILayout.FlexibleSpace();
        AuthorShared.EndHorizontal();
        return false;
    }

    public virtual AuthorShared.PeiceAction PeiceListGUI()
    {
        bool isSelected = AuthorShared.SelectionContains(this.selectReference) || AuthorShared.SelectionContains(this);
        AuthorShared.PeiceAction none = AuthorShared.PeiceAction.None;
        AuthorShared.BeginHorizontal(new GUILayoutOption[0]);
        ActionButton(this.peiceID, ref none, isSelected, AuthorShared.PeiceAction.AddToSelection, AuthorShared.PeiceAction.RemoveFromSelection, AuthorShared.Styles.peiceButtonLeft, new GUILayoutOption[0]);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
        ActionButton(AuthorShared.Icon.solo, ref none, isSelected, AuthorShared.PeiceAction.SelectSolo, AuthorShared.Styles.peiceButtonMid, options);
        Color contentColor = GUI.contentColor;
        GUI.contentColor = Color.red;
        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
        ActionButton(AuthorShared.Icon.delete, ref none, isSelected, AuthorShared.PeiceAction.Delete, AuthorShared.Styles.peiceButtonRight, optionArray2);
        GUI.contentColor = contentColor;
        AuthorShared.EndHorizontal();
        return none;
    }

    public void Registered(AuthorCreation creation)
    {
        this._creation = creation;
        if (this._peiceID == null)
        {
        }
        this._peiceID = string.Empty;
        this.OnRegistered();
    }

    public virtual void SaveJsonProperties(JSONStream stream)
    {
    }

    public AuthorCreation creation
    {
        get
        {
            return this._creation;
        }
    }

    public string peiceID
    {
        get
        {
            return this._peiceID;
        }
        set
        {
            if (value == null)
            {
            }
            this._peiceID = string.Empty;
        }
    }

    public Object selectReference
    {
        get
        {
            return base.gameObject;
        }
    }
}

