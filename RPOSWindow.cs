using System;
using System.Collections.Generic;
using UnityEngine;

public class RPOSWindow : MonoBehaviour
{
    private bool _awake;
    [SerializeField]
    private GameObject _background;
    [SerializeField]
    private UIButton _closeButton;
    private bool _closed;
    private bool _destroyAfterAwake;
    private bool _destroyed;
    [SerializeField]
    private GameObject _dragger;
    private bool _forceHide;
    private bool _inventoryHide;
    [SerializeField]
    private bool _isInventoryRelated;
    private bool _lock_awake;
    private bool _lock_close;
    private bool _lock_destroy;
    private bool _lock_open;
    private bool _lock_show;
    [SerializeField]
    private UIPanel _myPanel;
    private bool _opened;
    private bool _showing;
    private bool _showWithoutRPOS;
    private bool _showWithRPOS;
    [SerializeField]
    private UIWidget.Pivot _shrinkPivot = UIWidget.Pivot.Center;
    [SerializeField]
    private UILabel _titleObj;
    [SerializeField]
    private Vector4 _windowDimensions = new Vector4(0f, 0f, 128f, 32f);
    [SerializeField]
    protected bool autoShowWithoutRPOS;
    [SerializeField]
    protected bool autoShowWithRPOS;
    private List<RPOSBumper.Instance> bumpers;
    private bool bumpersDisabled;
    private readonly UIEventListener.VoidDelegate buttonCallback;
    [SerializeField]
    protected UIPanel[] childPanels;
    private UIEventListener closeButtonListener;
    [SerializeField]
    protected bool destroyWithClose;
    [SerializeField]
    protected bool hidesWithCloseButton;
    [SerializeField]
    protected bool hidesWithExternalClose;
    private RPOSWindowMessageCenter messageCenter;
    protected bool neverAutoShow;
    [SerializeField]
    private string TitleText;
    [Obsolete("RPOS ONLY")]
    internal int zzz__index = -1;

    public event RPOSWindowMessageHandler DidClose
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.DidClose, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.DidClose, value);
        }
    }

    public event RPOSWindowMessageHandler DidHide
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.DidHide, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.DidHide, value);
        }
    }

    public event RPOSWindowMessageHandler DidOpen
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.DidOpen, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.DidOpen, value);
        }
    }

    public event RPOSWindowMessageHandler DidShow
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.DidShow, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.DidShow, value);
        }
    }

    public event RPOSWindowMessageHandler WillClose
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.WillClose, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.WillClose, value);
        }
    }

    public event RPOSWindowMessageHandler WillHide
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.WillHide, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.WillHide, value);
        }
    }

    public event RPOSWindowMessageHandler WillOpen
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.WillOpen, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.WillOpen, value);
        }
    }

    public event RPOSWindowMessageHandler WillShow
    {
        add
        {
            this.AddMessageHandler(RPOSWindowMessage.WillShow, value);
        }
        remove
        {
            this.RemoveMessageHandler(RPOSWindowMessage.WillShow, value);
        }
    }

    public RPOSWindow()
    {
        this.buttonCallback = new UIEventListener.VoidDelegate(this.ButtonClickCallback);
    }

    private void _EnsureAwake()
    {
        if (!this._awake)
        {
            if (this._lock_awake)
            {
                Debug.LogWarning("Something tried to ensure this while it was being awoken in ensure awake", this);
            }
            else if (this._destroyed)
            {
                Debug.LogWarning("This window was destroyed before it could be awoke", this);
            }
            else if (this._lock_destroy)
            {
                Debug.LogWarning("This window is in the process of being destroyed, please look at the call stack and avoid this", this);
            }
            else
            {
                try
                {
                    this._lock_awake = true;
                    this._myPanel = base.GetComponent<UIPanel>();
                    this.panelsEnabled = false;
                    if (this._closeButton != null)
                    {
                        this.closeButtonListener = UIEventListener.Get(this._closeButton.gameObject);
                        if (this.closeButtonListener != null)
                        {
                            this.closeButtonListener.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(this.closeButtonListener.onClick, this.buttonCallback);
                        }
                    }
                    this.WindowAwake();
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format("A exception was thrown during window awake ({0}, title={1}) and has probably broken something, exception is below\r\n{2}", this, this.TitleText, exception), this);
                }
                finally
                {
                    this._awake = true;
                    this._lock_awake = false;
                    if (this._destroyAfterAwake)
                    {
                        Debug.LogWarning("Because of something trying to destroy this while we were awaking, destroy will occur now", this);
                        try
                        {
                            this._lock_destroy = true;
                            this.WindowDestroy();
                        }
                        catch (Exception exception2)
                        {
                            Debug.LogError(string.Format("A exception was thrown during window destroy following awake. ({0}, title={1}) and potentially screwed up stuff, exception is below\r\n{2}", this, this.TitleText, exception2), this);
                        }
                        finally
                        {
                            this._destroyed = true;
                            this._lock_destroy = false;
                        }
                    }
                    else
                    {
                        RPOS.RegisterWindow(this);
                    }
                }
            }
        }
    }

    private void _EnsureDestroy()
    {
        if (this._awake)
        {
            if (this._lock_destroy)
            {
                Debug.LogWarning("Something tried to destroy while this window was destroying", this);
            }
            else
            {
                try
                {
                    this._lock_destroy = true;
                    if (this.closeButtonListener != null)
                    {
                        this.closeButtonListener.onClick = (UIEventListener.VoidDelegate) Delegate.Remove(this.closeButtonListener.onClick, this.buttonCallback);
                    }
                    if (!this._closed)
                    {
                        this._showWithRPOS = false;
                        this._showWithoutRPOS = false;
                        this.CheckDisplay();
                        if (this._opened && !this._closed)
                        {
                            this.WindowClose();
                        }
                    }
                    this.WindowDestroy();
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format("A exception was thrown during window destroy ({0}, title={1}) and potentially screwed up stuff, exception is below\r\n{2}", this, this.TitleText, exception), this);
                }
                finally
                {
                    this._destroyed = true;
                    this._lock_destroy = false;
                    RPOS.UnregisterWindow(this);
                }
            }
        }
        else if (this._lock_awake)
        {
            Debug.LogWarning("This window was awakening.. the call to destroy will happen when its done. Look at call stack. Avoid this.", this);
            this._destroyAfterAwake = true;
        }
        else if (!this._lock_destroy)
        {
            this._lock_destroy = true;
            Debug.LogWarning("This window is being destroyed, and has never got it's Awake.", this);
        }
    }

    internal void AddBumper(RPOSBumper.Instance inst)
    {
        inst.label.text = this.title;
        UIEventListener listener = inst.listener;
        if (listener != null)
        {
            listener.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener.onClick, this.buttonCallback);
            if (this.bumpers == null)
            {
                this.bumpers = new List<RPOSBumper.Instance>();
            }
            this.bumpers.Add(inst);
        }
    }

    public bool AddMessageHandler(RPOSWindowMessage message, RPOSWindowMessageHandler handler)
    {
        return (((!this._destroyed && !this._lock_destroy) && (!this._destroyAfterAwake && (this._awake || this._lock_awake))) && this.messageCenter.Add(message, handler));
    }

    [Obsolete("Use WindowAwake", true)]
    protected void Awake()
    {
        this._EnsureAwake();
    }

    public bool BringToFront()
    {
        return RPOS.BringToFront(this);
    }

    private void ButtonClickCallback(GameObject button)
    {
        if (GameObjectEqual(button, this._closeButton))
        {
            this.CloseButtonClicked();
        }
        else if (this.bumpers != null)
        {
            int count = this.bumpers.Count;
            if ((count > 0) && (button != null))
            {
                UIButton component = button.GetComponent<UIButton>();
                if (component != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (component == this.bumpers[i].button)
                        {
                            this.OnBumperClick(this.bumpers[i]);
                            return;
                        }
                    }
                }
            }
        }
    }

    [Obsolete("For use by RPOS only!")]
    internal bool CheckDisplay()
    {
        if (this._lock_show)
        {
            return false;
        }
        if (!this._showing)
        {
            if ((!this._showWithoutRPOS && (!this._showWithRPOS || !RPOS.IsOpen)) || (this._inventoryHide && this._isInventoryRelated))
            {
                return false;
            }
            this._showing = true;
            this.WindowShow();
        }
        else
        {
            if ((!this._forceHide && (this._showWithoutRPOS || (this._showWithRPOS && RPOS.IsOpen))) && (!this._inventoryHide || !this._isInventoryRelated))
            {
                return false;
            }
            this._showing = false;
            this.WindowHide();
        }
        return true;
    }

    protected virtual void CloseButtonClicked()
    {
        this.HideOrClose(this.hidesWithCloseButton);
    }

    public static void EnsureAwake(RPOSWindow window)
    {
        window._EnsureAwake();
    }

    public void ExternalClose()
    {
        this.OnExternalClose();
    }

    private void FireEvent(RPOSWindowMessage message)
    {
        this.messageCenter.Fire(this, message);
    }

    private static bool GameObjectEqual(Object A, Object B)
    {
        if (A != null)
        {
            if (B != null)
            {
                if (A is GameObject)
                {
                    if (B is GameObject)
                    {
                        return (A == B);
                    }
                    return ((B is Component) && (((GameObject) A) == ((Component) B).gameObject));
                }
                if (A is Component)
                {
                    if (B is GameObject)
                    {
                        return (((Component) A).gameObject == ((GameObject) B));
                    }
                    if (B is Component)
                    {
                        return (((Component) A).gameObject == ((Component) B).gameObject);
                    }
                }
            }
            return false;
        }
        if (B != null)
        {
            return false;
        }
        return true;
    }

    protected void Hide()
    {
        this.showWithRPOS = false;
        this.showWithoutRPOS = false;
    }

    private void HideOrClose(bool hideIsTrue)
    {
        if (hideIsTrue)
        {
            this.Hide();
        }
        else
        {
            this.WindowClose();
        }
    }

    public bool IsAbove(RPOSWindow window)
    {
        return (window.order < this.order);
    }

    public bool IsBelow(RPOSWindow window)
    {
        return (window.order > this.order);
    }

    public bool MoveDown()
    {
        EnsureAwake(this);
        return RPOS.MoveDown(this);
    }

    public void MovePixelX(int x)
    {
        this.MovePixelXY(x, 0);
    }

    public void MovePixelXY(int x, int y)
    {
        base.transform.position = base.transform.TransformPoint((float) x, (float) y, 0f);
    }

    public void MovePixelY(int y)
    {
        this.MovePixelXY(0, y);
    }

    public bool MoveUp()
    {
        EnsureAwake(this);
        return RPOS.MoveUp(this);
    }

    protected virtual void OnBumperClick(RPOSBumper.Instance bumper)
    {
        if (!this.bumpersDisabled)
        {
            this.showWithRPOS = !this.showWithRPOS;
            if (this._showWithRPOS)
            {
                this.BringToFront();
            }
        }
    }

    [Obsolete("Forwarder to SubTouch with SubTouchKind.Click", true)]
    protected void OnChildClick(GameObject go)
    {
        this.SubTouch(go, SubTouchKind.Click);
    }

    [Obsolete("Forwarder to SubTouch with SubTouchKind.ClickCancel", true)]
    protected void OnChildClickMissed(GameObject go)
    {
        this.SubTouch(go, SubTouchKind.ClickCancel);
    }

    [Obsolete("Forwarder to SubTouch with SubTouchKind.Press if true else SubTouchKind.Release", true)]
    protected void OnChildPress(bool press)
    {
        this.SubTouch(UICamera.Cursor.Buttons.LeftValue.Pressed, !press ? SubTouchKind.Release : SubTouchKind.Press);
    }

    [Obsolete("Use WindowDestroy", true)]
    protected void OnDestroy()
    {
        if (this._awake)
        {
            this._EnsureDestroy();
        }
    }

    protected void OnDrawGizmosSelected()
    {
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix = base.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(this._windowDimensions.x + (this._windowDimensions.z / 2f), this._windowDimensions.y + (this._windowDimensions.w / 2f)), new Vector3(this._windowDimensions.z, this._windowDimensions.w));
        Gizmos.matrix = matrix;
    }

    protected virtual void OnExternalClose()
    {
        this.HideOrClose(this.hidesWithExternalClose);
    }

    protected virtual void OnRPOSClosed()
    {
    }

    protected virtual void OnRPOSOpened()
    {
    }

    public void OnScroll(float delta)
    {
        Debug.Log("fuck you" + delta);
    }

    protected virtual void OnWindowClosed()
    {
    }

    protected virtual void OnWindowHide()
    {
        this.panelsEnabled = false;
    }

    protected virtual void OnWindowOpened()
    {
        this.BringToFront();
    }

    protected virtual void OnWindowReOpen()
    {
        this.OnWindowOpened();
    }

    protected virtual void OnWindowShow()
    {
        this.panelsEnabled = true;
    }

    internal void RemoveBumper(RPOSBumper.Instance inst)
    {
        if ((this.bumpers != null) && this.bumpers.Remove(inst))
        {
            UIEventListener listener = inst.listener;
            if (listener != null)
            {
                listener.onClick = (UIEventListener.VoidDelegate) Delegate.Remove(listener.onClick, this.buttonCallback);
            }
        }
    }

    public bool RemoveMessageHandler(RPOSWindowMessage message, RPOSWindowMessageHandler handler)
    {
        if (this._awake && !this._destroyed)
        {
            return false;
        }
        return this.messageCenter.Remove(message, handler);
    }

    [Obsolete("For use by RPOS only!")]
    internal void RPOSOff()
    {
        this.OnRPOSClosed();
    }

    [Obsolete("For use by RPOS only!")]
    internal void RPOSOn()
    {
        this.OnRPOSOpened();
    }

    [Obsolete("For use by RPOS only!")]
    internal void RPOSReady()
    {
        if (!this.neverAutoShow)
        {
            if (this.autoShowWithRPOS)
            {
                this._showWithRPOS = true;
            }
            if (this.autoShowWithoutRPOS)
            {
                this._showWithoutRPOS = true;
            }
        }
        if (RPOS.IsOpen)
        {
            Debug.Log("Was ready");
            this.RPOSOn();
        }
        this.CheckDisplay();
    }

    public bool SendToBack()
    {
        return RPOS.SendToBack(this);
    }

    protected void SetWindowTitle(string title)
    {
        this.TitleText = title;
        this._titleObj.text = title.ToUpper();
        if (this.bumpers != null)
        {
            foreach (RPOSBumper.Instance instance in this.bumpers)
            {
                if (instance.label != null)
                {
                    instance.label.text = title.ToUpper();
                }
            }
        }
    }

    protected virtual void SubTouch(GameObject go, SubTouchKind kind)
    {
        switch (kind)
        {
            case SubTouchKind.Press:
                if ((go == this._dragger) || (go == this._background))
                {
                    this.BringToFront();
                }
                break;

            case SubTouchKind.Click:
            case SubTouchKind.ClickCancel:
                this.BringToFront();
                break;
        }
    }

    protected virtual void WindowAwake()
    {
        this.SetWindowTitle(this.TitleText);
    }

    private void WindowClose()
    {
        if (!this._closed && !this._lock_close)
        {
            if (this._lock_open)
            {
                throw new InvalidOperationException("cannot close while opening -- check call stack.");
            }
            try
            {
                this._lock_close = true;
                this._forceHide = true;
                if (this._showing)
                {
                    this.CheckDisplay();
                }
                if (this._opened)
                {
                    this.FireEvent(RPOSWindowMessage.WillClose);
                    this._closed = true;
                    this.OnWindowClosed();
                    this._opened = false;
                    this.FireEvent(RPOSWindowMessage.DidClose);
                }
            }
            finally
            {
                this._lock_close = false;
            }
            if ((this.destroyWithClose && !this._lock_destroy) && (!this._destroyed && !this._destroyAfterAwake))
            {
                Object.Destroy(base.gameObject);
            }
        }
    }

    protected virtual void WindowDestroy()
    {
    }

    private void WindowHide()
    {
        if (this._lock_show)
        {
            throw new InvalidOperationException("The window was already in the process of showing or hiding");
        }
        try
        {
            this._lock_show = true;
            this.FireEvent(RPOSWindowMessage.WillHide);
            this.OnWindowHide();
            this.FireEvent(RPOSWindowMessage.DidHide);
        }
        finally
        {
            this._lock_show = false;
        }
    }

    private void WindowOpen()
    {
        if (!this._opened && !this._lock_open)
        {
            if (this._lock_close)
            {
                throw new InvalidOperationException("cannot open while closing -- check call stack.");
            }
            try
            {
                this._lock_open = true;
                bool flag = this._closed;
                this.FireEvent(RPOSWindowMessage.WillOpen);
                this._opened = true;
                this._closed = false;
                if (flag)
                {
                    this.OnWindowReOpen();
                }
                else
                {
                    this.OnWindowOpened();
                }
                this.FireEvent(RPOSWindowMessage.DidOpen);
            }
            finally
            {
                this._lock_open = false;
            }
            if (!this._lock_show)
            {
                this.CheckDisplay();
            }
        }
    }

    private void WindowShow()
    {
        if (this._lock_show)
        {
            throw new InvalidOperationException("The window was already in the process of showing or hiding");
        }
        if (!this._opened)
        {
            this.WindowOpen();
        }
        try
        {
            this._lock_show = true;
            this.FireEvent(RPOSWindowMessage.WillShow);
            this.OnWindowShow();
            this.FireEvent(RPOSWindowMessage.DidShow);
        }
        finally
        {
            this._lock_show = false;
        }
    }

    internal void zzz___INTERNAL_FOCUS()
    {
        if (!this.showWithRPOS)
        {
            this.showWithRPOS = true;
        }
        this.BringToFront();
    }

    public GameObject background
    {
        get
        {
            return this._background;
        }
        private set
        {
            this._background = value;
        }
    }

    public bool bumpersEnabled
    {
        get
        {
            return !this.bumpersDisabled;
        }
        set
        {
            this.bumpersDisabled = !value;
        }
    }

    public UIButton closeButton
    {
        get
        {
            return this._closeButton;
        }
        private set
        {
            this._closeButton = value;
        }
    }

    public bool closed
    {
        get
        {
            return (this._closed && !this._opened);
        }
    }

    public GameObject dragger
    {
        get
        {
            return this._dragger;
        }
        private set
        {
            this._dragger = value;
        }
    }

    internal bool inventoryHide
    {
        get
        {
            return this._inventoryHide;
        }
        set
        {
            if (this._inventoryHide != value)
            {
                this._inventoryHide = value;
                if (this._isInventoryRelated && this.ready)
                {
                    this.CheckDisplay();
                }
            }
        }
    }

    public bool isInventoryRelated
    {
        get
        {
            return this._isInventoryRelated;
        }
    }

    public UIPanel mainPanel
    {
        get
        {
            return this._myPanel;
        }
        private set
        {
            this._myPanel = value;
        }
    }

    public RPOSWindow nextWindow
    {
        get
        {
            return RPOS.GetWindowAbove(this);
        }
    }

    public int numAbove
    {
        get
        {
            return (RPOS.WindowCount - (this.order + 1));
        }
    }

    public int numBelow
    {
        get
        {
            return this.order;
        }
    }

    public bool open
    {
        get
        {
            return (this._opened && !this._closed);
        }
    }

    public int order
    {
        get
        {
            if (this.zzz__index == -1)
            {
                throw new InvalidOperationException("this window is not yet ready. you should check .ready");
            }
            return this.zzz__index;
        }
    }

    private bool panelsEnabled
    {
        set
        {
            if (this._myPanel != null)
            {
                this._myPanel.enabled = value;
            }
            if (this.childPanels != null)
            {
                foreach (UIPanel panel in this.childPanels)
                {
                    if (panel != null)
                    {
                        panel.enabled = value;
                    }
                }
            }
        }
    }

    public RPOSWindow prevWindow
    {
        get
        {
            return RPOS.GetWindowBelow(this);
        }
    }

    public bool ready
    {
        get
        {
            return (this.zzz__index != -1);
        }
    }

    public bool showing
    {
        get
        {
            return this._showing;
        }
    }

    public bool showingWithoutRPOS
    {
        get
        {
            return (this._showing && !RPOS.IsOpen);
        }
    }

    public bool showingWithRPOS
    {
        get
        {
            return (this._showing && RPOS.IsOpen);
        }
    }

    public bool showWithoutRPOS
    {
        get
        {
            return (!this._forceHide && this._showWithoutRPOS);
        }
        protected set
        {
            if (value != this._showWithoutRPOS)
            {
                this._showWithoutRPOS = value;
                this.CheckDisplay();
            }
        }
    }

    public bool showWithRPOS
    {
        get
        {
            return (!this._forceHide && this._showWithRPOS);
        }
        protected set
        {
            if (value != this._showWithRPOS)
            {
                this._showWithRPOS = value;
                this.CheckDisplay();
            }
        }
    }

    public UIWidget.Pivot shrinkPivot
    {
        get
        {
            return this._shrinkPivot;
        }
    }

    public string title
    {
        get
        {
            return this.TitleText;
        }
        set
        {
            if ((value != null) && !string.Equals(this.TitleText, value))
            {
                this.SetWindowTitle(value);
            }
        }
    }

    public UILabel titleObj
    {
        get
        {
            return this._titleObj;
        }
        private set
        {
            this._titleObj = value;
        }
    }

    public Vector4 windowDimensions
    {
        get
        {
            return this._windowDimensions;
        }
    }

    protected enum SubTouchKind
    {
        Press,
        Click,
        ClickCancel,
        Release
    }
}

