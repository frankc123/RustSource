using NGUI.MessageUtil;
using NGUIHack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Camera)), AddComponentMenu("NGUI/UI/Camera"), ExecuteInEditMode]
public class UICamera : MonoBehaviour
{
    public bool allowMultiTouch = true;
    public KeyCode cancelKey0 = KeyCode.Escape;
    public KeyCode cancelKey1 = KeyCode.JoystickButton1;
    public static Camera currentCamera = null;
    public static BackwardsCompatabilitySupport currentTouch;
    public static int currentTouchID = -1;
    public LayerMask eventReceiverMask = -1;
    public static GameObject fallThrough;
    public string horizontalAxisName = "Horizontal";
    public static bool inputHasFocus = false;
    private static bool inSelectionCallback;
    public UIInputMode keyboardMode = UIInputMode.UseInputAndEvents;
    private const int kMouseButton0Flag = 1;
    private const int kMouseButton1Flag = 2;
    private const int kMouseButton2Flag = 4;
    private const int kMouseButton3Flag = 8;
    private const int kMouseButton4Flag = 0x10;
    private const int kMouseButtonCount = 3;
    private int lastBoundLayerIndex = -1;
    public static UIHotSpot.Hit lastHit;
    public static Vector2 lastMousePosition = Vector2.zero;
    public static Vector2 lastTouchPosition = Vector2.zero;
    private Camera mCam;
    private static List<Highlighted> mHighlighted = new List<Highlighted>();
    private static GameObject mHover;
    private bool mIsEditor;
    private static Dictionary<KeyCode, UICamera> mKeyCamera = new Dictionary<KeyCode, UICamera>();
    private LayerMask mLayerMask;
    private static UICamera[] mList = new UICamera[0x20];
    private static int mListCount = 0;
    private static byte[] mListSort = new byte[0x20];
    private static Dictionary<int, UICamera> mMouseCamera = new Dictionary<int, UICamera>();
    private static float mNextEvent = 0f;
    public float mouseClickThreshold = 10f;
    public UIInputMode mouseMode = UIInputMode.UseEvents;
    private static UIInput mPressInput = null;
    private static GameObject mSel = null;
    private static UIInput mSelInput = null;
    private GameObject mTooltip;
    private float mTooltipTime;
    public bool onlyHotSpots;
    private static UIPanel popupPanel;
    public float rangeDistance = -1f;
    public string scrollAxisName = "Mouse ScrollWheel";
    public UIInputMode scrollWheelMode = UIInputMode.UseEvents;
    private static readonly CamSorter sorter = new CamSorter();
    public bool stickyTooltip = true;
    public KeyCode submitKey0 = KeyCode.Return;
    public KeyCode submitKey1 = KeyCode.JoystickButton0;
    public static bool SwallowScroll;
    public float tooltipDelay = 1f;
    public float touchClickThreshold = 40f;
    public bool useController = true;
    public bool useKeyboard = true;
    public bool useMouse = true;
    public bool useTouch = true;
    public string verticalAxisName = "Vertical";

    private void AddToList()
    {
        int layer = base.gameObject.layer;
        if (layer != this.lastBoundLayerIndex)
        {
            bool flag;
            if ((this.lastBoundLayerIndex != -1) && (mList[this.lastBoundLayerIndex] == this))
            {
                mList[this.lastBoundLayerIndex] = null;
                for (int i = 0; i < mListCount; i++)
                {
                    if (mListSort[i] == this.lastBoundLayerIndex)
                    {
                        mListSort[i] = (byte) layer;
                    }
                }
                flag = false;
            }
            else
            {
                mListSort[mListCount++] = (byte) layer;
                flag = true;
            }
            mList[layer] = this;
            this.lastBoundLayerIndex = layer;
            if (flag)
            {
                Array.Sort<byte>(mListSort, 0, mListCount, sorter);
            }
        }
    }

    private void Awake()
    {
        if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            this.useMouse = false;
            this.useTouch = true;
            this.useKeyboard = false;
            this.useController = false;
        }
        else if ((Application.platform == RuntimePlatform.PS3) || (Application.platform == RuntimePlatform.XBOX360))
        {
            this.useMouse = false;
            this.useTouch = false;
            this.useKeyboard = false;
            this.useController = true;
        }
        else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.OSXEditor))
        {
            this.mIsEditor = true;
        }
        this.AddToList();
        if (this.eventReceiverMask == -1)
        {
            this.eventReceiverMask = base.camera.cullingMask;
        }
        if (this.usesAnyEvents && Application.isPlaying)
        {
            UIUnityEvents.CameraCreated(this);
        }
    }

    private static bool CheckRayEnterClippingRect(Ray ray, Transform transform, Vector4 clipRange)
    {
        float num;
        Plane plane = new Plane(transform.forward, transform.position);
        if (plane.Raycast(ray, out num))
        {
            Vector3 point = transform.InverseTransformPoint(ray.GetPoint(num));
            clipRange.z = Mathf.Abs(clipRange.z);
            clipRange.w = Mathf.Abs(clipRange.w);
            Rect rect = new Rect(clipRange.x - (clipRange.z / 2f), clipRange.y - (clipRange.w / 2f), clipRange.z, clipRange.w);
            return rect.Contains(point);
        }
        return false;
    }

    private static int CompareFunc(UICamera a, UICamera b)
    {
        return b.cachedCamera.depth.CompareTo(a.cachedCamera.depth);
    }

    public static UICamera FindCameraForLayer(int layer)
    {
        return mList[layer];
    }

    private static int GetDirection(string axis)
    {
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        if (mNextEvent < realtimeSinceStartup)
        {
            float num2 = Input.GetAxis(axis);
            if (num2 > 0.75f)
            {
                mNextEvent = realtimeSinceStartup + 0.25f;
                return 1;
            }
            if (num2 < -0.75f)
            {
                mNextEvent = realtimeSinceStartup + 0.25f;
                return -1;
            }
        }
        return 0;
    }

    private static int GetDirection(KeyCode up, KeyCode down)
    {
        bool keyDown = Input.GetKeyDown(up);
        bool flag2 = Input.GetKeyDown(down);
        if (keyDown == flag2)
        {
            return 0;
        }
        if (keyDown)
        {
            return (!Input.GetKey(down) ? 1 : 0);
        }
        return (!Input.GetKey(up) ? -1 : 0);
    }

    private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
    {
        bool flag = Input.GetKeyDown(up0) | Input.GetKeyDown(up1);
        bool flag2 = Input.GetKeyDown(down0) | Input.GetKeyDown(down1);
        if (flag == flag2)
        {
            return 0;
        }
        if (flag)
        {
            return ((!Input.GetKey(down0) && !Input.GetKey(down1)) ? 1 : 0);
        }
        return ((!Input.GetKey(up0) && !Input.GetKey(up1)) ? -1 : 0);
    }

    public static void HandleEvent(Event @event, EventType type)
    {
        switch (type)
        {
            case EventType.MouseDown:
                using (new Mouse.Button.ButtonPressEventHandler(@event))
                {
                    IssueEvent(@event, EventType.MouseDown);
                }
                return;

            case EventType.MouseUp:
                using (new Mouse.Button.ButtonReleaseEventHandler(@event))
                {
                    IssueEvent(@event, EventType.MouseUp);
                }
                return;

            case EventType.MouseMove:
                if (!Mouse.Button.AllowMove)
                {
                    return;
                }
                break;

            case EventType.MouseDrag:
                if (!Mouse.Button.AllowDrag)
                {
                    return;
                }
                break;

            case EventType.Repaint:
            case EventType.Layout:
            case EventType.Ignore:
            case EventType.Used:
                return;
        }
        IssueEvent(@event, type);
        if ((type == EventType.MouseMove) && (@event.type == EventType.Used))
        {
            Debug.LogWarning("Mouse move was used.");
        }
    }

    private static void Highlight(GameObject go, bool highlighted)
    {
        if (go != null)
        {
            int count = mHighlighted.Count;
            while (count > 0)
            {
                Highlighted item = mHighlighted[--count];
                if ((item == null) || (item.go == null))
                {
                    mHighlighted.RemoveAt(count);
                }
                else if (item.go == go)
                {
                    if (highlighted)
                    {
                        item.counter++;
                    }
                    else if (--item.counter < 1)
                    {
                        mHighlighted.Remove(item);
                        go.Hover(false);
                    }
                    return;
                }
            }
            if (highlighted)
            {
                Highlighted highlighted3 = new Highlighted {
                    go = go,
                    counter = 1
                };
                mHighlighted.Add(highlighted3);
                go.Hover(true);
            }
        }
    }

    public static bool IsHighlighted(GameObject go)
    {
        int count = mHighlighted.Count;
        while (count > 0)
        {
            Highlighted highlighted = mHighlighted[--count];
            if (highlighted.go == go)
            {
                return true;
            }
        }
        return false;
    }

    private static void IssueEvent(Event @event, EventType type)
    {
        int num2;
        int button = @event.button;
        KeyCode keyCode = @event.keyCode;
        UICamera camera = null;
        switch (type)
        {
            case EventType.MouseDown:
                if (((button == 0) || !mMouseCamera.TryGetValue(0, out camera)) || (camera == null))
                {
                    goto Label_0158;
                }
                camera.OnEvent(@event, type);
                if (@event.type == EventType.MouseDown)
                {
                    goto Label_0158;
                }
                mMouseCamera[button] = camera;
                return;

            case EventType.MouseUp:
                if (mMouseCamera.TryGetValue(button, out camera))
                {
                    if (camera == null)
                    {
                        @event.Use();
                    }
                    else
                    {
                        camera.OnEvent(@event, type);
                        if (@event.type == EventType.MouseUp)
                        {
                            @event.Use();
                        }
                    }
                    mMouseCamera.Remove(button);
                }
                return;

            case EventType.MouseDrag:
                if (mMouseCamera.TryGetValue(0, out camera))
                {
                    if (camera == null)
                    {
                        @event.Use();
                        break;
                    }
                    camera.OnEvent(@event, type);
                }
                break;

            case EventType.KeyUp:
                if (mKeyCamera.TryGetValue(keyCode, out camera))
                {
                    if (camera == null)
                    {
                        @event.Use();
                    }
                    else
                    {
                        camera.OnEvent(@event, type);
                        if (@event.type == EventType.KeyUp)
                        {
                            @event.Use();
                        }
                    }
                    mKeyCamera.Remove(keyCode);
                }
                return;

            default:
                goto Label_0158;
        }
        return;
    Label_0158:
        num2 = 0;
        while (num2 < mListCount)
        {
            UICamera camera2 = mList[mListSort[num2]];
            if ((camera2 != camera) && camera2.usesAnyEvents)
            {
                camera2.OnEvent(@event, type);
                if (@event.type != type)
                {
                    switch (type)
                    {
                        case EventType.MouseDown:
                            mMouseCamera[button] = camera2;
                            break;

                        case EventType.KeyDown:
                            mKeyCamera[keyCode] = camera2;
                            break;
                    }
                    return;
                }
            }
            num2++;
        }
    }

    private void OnApplicationQuit()
    {
        mHighlighted.Clear();
    }

    private void OnCancelEvent(Event @event, EventType type)
    {
        if (type == EventType.KeyDown)
        {
            mSel.SendMessage("OnKey", KeyCode.Escape, SendMessageOptions.DontRequireReceiver);
            @event.Use();
        }
    }

    private void OnDestroy()
    {
        this.RemoveFromList();
    }

    private void OnDirectionEvent(Event @event, int x, int y, EventType type)
    {
        bool flag = false;
        if (type == EventType.KeyDown)
        {
            if (x != 0)
            {
                mSel.SendMessage("OnKey", (x >= 0) ? KeyCode.RightArrow : KeyCode.LeftArrow, SendMessageOptions.DontRequireReceiver);
                flag = true;
            }
            if (y != 0)
            {
                mSel.SendMessage("OnKey", (y >= 0) ? KeyCode.UpArrow : KeyCode.DownArrow, SendMessageOptions.DontRequireReceiver);
                flag = true;
            }
        }
        if (flag)
        {
            @event.Use();
        }
    }

    private void OnEvent(Event @event, EventType type)
    {
        Camera currentCamera = UICamera.currentCamera;
        try
        {
            UICamera.currentCamera = this.cachedCamera;
            switch (type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseMove:
                case EventType.MouseDrag:
                    if ((this.mouseMode & UIInputMode.UseEvents) == UIInputMode.UseEvents)
                    {
                        this.OnMouseEvent(@event, type);
                    }
                    return;

                case EventType.KeyDown:
                case EventType.KeyUp:
                    if ((this.keyboardMode & UIInputMode.UseEvents) == UIInputMode.UseEvents)
                    {
                        this.OnKeyboardEvent(@event, type);
                    }
                    return;

                case EventType.ScrollWheel:
                    if ((this.scrollWheelMode & UIInputMode.UseEvents) == UIInputMode.UseEvents)
                    {
                        this.OnScrollWheelEvent(@event, type);
                    }
                    return;
            }
        }
        finally
        {
            UICamera.currentCamera = currentCamera;
        }
    }

    private bool OnEventShared(Event @event, EventType type)
    {
        return false;
    }

    private void OnKeyboardEvent(Event @event, EventType type)
    {
        if (!this.OnEventShared(@event, type))
        {
            char character = @event.character;
            KeyCode keyCode = @event.keyCode;
            bool mSelInput = (bool) UICamera.mSelInput;
            if (mSelInput)
            {
                UICamera.mSelInput.OnEvent(this, @event, type);
            }
            if (mSel != null)
            {
                KeyCode code2 = keyCode;
                if (code2 != KeyCode.Tab)
                {
                    if (code2 != KeyCode.Delete)
                    {
                        if ((type == EventType.KeyDown) && (character != '\0'))
                        {
                            mSel.Input(character.ToString());
                        }
                        if ((keyCode == this.submitKey0) || (keyCode == this.submitKey1))
                        {
                            if (!mSelInput || (@event.type == type))
                            {
                                this.OnSubmitEvent(@event, type);
                            }
                        }
                        else if ((keyCode == this.cancelKey0) || (keyCode == this.cancelKey1))
                        {
                            if (!mSelInput || (@event.type == type))
                            {
                                this.OnCancelEvent(@event, type);
                            }
                        }
                        else if (inputHasFocus)
                        {
                            if (!mSelInput || (@event.type == type))
                            {
                                switch (keyCode)
                                {
                                    case KeyCode.UpArrow:
                                        this.OnDirectionEvent(@event, 0, 1, type);
                                        return;

                                    case KeyCode.DownArrow:
                                        this.OnDirectionEvent(@event, 0, -1, type);
                                        return;

                                    case KeyCode.LeftArrow:
                                        this.OnDirectionEvent(@event, -1, 0, type);
                                        return;
                                }
                                if (keyCode == KeyCode.RightArrow)
                                {
                                    this.OnDirectionEvent(@event, 1, 0, type);
                                }
                            }
                        }
                        else if (!mSelInput || (@event.type == type))
                        {
                            switch (keyCode)
                            {
                                case KeyCode.UpArrow:
                                case KeyCode.W:
                                    this.OnDirectionEvent(@event, 0, 1, type);
                                    break;

                                case KeyCode.DownArrow:
                                case KeyCode.S:
                                    this.OnDirectionEvent(@event, 0, -1, type);
                                    break;

                                case KeyCode.LeftArrow:
                                case KeyCode.A:
                                    this.OnDirectionEvent(@event, -1, 0, type);
                                    break;

                                case KeyCode.RightArrow:
                                case KeyCode.D:
                                    this.OnDirectionEvent(@event, 1, 0, type);
                                    break;
                            }
                        }
                    }
                    else if (type == EventType.KeyDown)
                    {
                        mSel.Input("\b");
                    }
                }
                else if (type == EventType.KeyDown)
                {
                    mSel.Key(KeyCode.Tab);
                }
            }
        }
    }

    private void OnMouseEvent(Event @event, EventType type)
    {
        if (!this.OnEventShared(@event, type))
        {
            Cursor.MouseEvent(@event, type);
        }
    }

    private void OnScrollWheelEvent(Event @event, EventType type)
    {
        if (mHover != null)
        {
            Vector2 delta = @event.delta;
            bool flag = false;
            bool flag2 = false;
            if (delta.y != 0f)
            {
                SwallowScroll = false;
                mHover.Scroll(delta.y);
                flag2 = !SwallowScroll;
            }
            if (delta.x != 0f)
            {
                SwallowScroll = false;
                mHover.ScrollX(delta.x);
                flag = !SwallowScroll;
            }
            if (flag2 || flag)
            {
                UIPanel panel = UIPanel.Find(mHover.transform);
                if (panel != null)
                {
                    if (flag2)
                    {
                        panel.gameObject.NGUIMessage<float>("OnHoverScroll", delta.y);
                    }
                    if (flag)
                    {
                        panel.gameObject.NGUIMessage<float>("OnHoverScrollX", delta.x);
                    }
                }
            }
            @event.Use();
        }
    }

    private void OnSubmitEvent(Event @event, EventType type)
    {
    }

    public static bool PopupPanel(UIPanel panel)
    {
        if (popupPanel == panel)
        {
            return false;
        }
        if (popupPanel != null)
        {
            popupPanel.gameObject.NGUIMessage("PopupEnd");
            popupPanel = null;
        }
        if (panel != null)
        {
            popupPanel = panel;
            popupPanel.gameObject.NGUIMessage("PopupStart");
        }
        return true;
    }

    private void ProcessOthers()
    {
        int num = 0;
        int num2 = 0;
        if (this.useController)
        {
            if (!string.IsNullOrEmpty(this.verticalAxisName))
            {
                num += GetDirection(this.verticalAxisName);
            }
            if (!string.IsNullOrEmpty(this.horizontalAxisName))
            {
                num2 += GetDirection(this.horizontalAxisName);
            }
        }
        if (num != 0)
        {
            mSel.SendMessage("OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow, SendMessageOptions.DontRequireReceiver);
        }
        if (num2 != 0)
        {
            mSel.SendMessage("OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow, SendMessageOptions.DontRequireReceiver);
        }
    }

    private static bool Raycast(UICamera cam, Vector3 inPos, ref RaycastHit hit)
    {
        RaycastCheckWork work;
        if (Screen.lockCursor)
        {
            return false;
        }
        if (!cam.enabled || !cam.gameObject.activeInHierarchy)
        {
            return false;
        }
        if (!cam.cachedCamera.pixelRect.Contains(inPos))
        {
            return false;
        }
        work.ray = cam.cachedCamera.ScreenPointToRay(inPos);
        work.mask = currentCamera.cullingMask & cam.eventReceiverMask;
        work.dist = (cam.rangeDistance <= 0f) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : cam.rangeDistance;
        bool flag = Physics.Raycast(work.ray, out work.hit, work.dist, work.mask) && work.Check();
        hit = work.hit;
        return flag;
    }

    private static bool Raycast(Vector3 inPos, ref UIHotSpot.Hit hit, out UICamera cam)
    {
        if (!Screen.lockCursor)
        {
            for (int i = 0; i < mListCount; i++)
            {
                cam = mList[mListSort[i]];
                if (cam.enabled && cam.gameObject.activeInHierarchy)
                {
                    currentCamera = cam.cachedCamera;
                    Vector3 vector = currentCamera.ScreenToViewportPoint(inPos);
                    if ((((vector.x >= -1f) && (vector.x <= 1f)) && (vector.y >= -1f)) && (vector.y <= 1f))
                    {
                        RaycastCheckWork work;
                        work.ray = currentCamera.ScreenPointToRay(inPos);
                        work.mask = currentCamera.cullingMask & cam.eventReceiverMask;
                        work.dist = (cam.rangeDistance <= 0f) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : cam.rangeDistance;
                        if (!cam.onlyHotSpots && (Physics.Raycast(work.ray, out work.hit, work.dist, work.mask) && work.Check()))
                        {
                            UIHotSpot.Hit hit2;
                            if (UIHotSpot.Raycast(work.ray, out hit2, work.dist) && (hit2.distance <= work.hit.distance))
                            {
                                hit = hit2;
                            }
                            else
                            {
                                UIHotSpot.ConvertRaycastHit(ref work.ray, ref work.hit, out hit);
                            }
                            return true;
                        }
                        if (UIHotSpot.Raycast(work.ray, out hit, work.dist))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        cam = null;
        return false;
    }

    public UITextPosition RaycastText(Vector3 inPos, UILabel label)
    {
        float num;
        if ((!base.enabled || !base.camera.enabled) || (!base.camera.pixelRect.Contains(inPos) || (label == null)))
        {
            Debug.Log("No Sir");
            return new UITextPosition();
        }
        Ray ray = base.camera.ScreenPointToRay(inPos);
        Vector3 forward = label.transform.forward;
        if (Vector3.Dot(ray.direction, forward) <= 0f)
        {
            Debug.Log("Bad Dir");
            return new UITextPosition();
        }
        Plane plane = new Plane(forward, label.transform.position);
        if (!plane.Raycast(ray, out num))
        {
            Debug.Log("Paralell");
            return new UITextPosition();
        }
        Vector3 point = ray.GetPoint(num);
        Vector3[] points = new Vector3[] { label.transform.InverseTransformPoint(point) };
        UITextPosition[] positions = new UITextPosition[] { new UITextPosition() };
        if (label.CalculateTextPosition(Space.Self, points, positions) == 0)
        {
            Debug.Log("Zero");
        }
        return positions[0];
    }

    private void RemoveFromList()
    {
        if (this.lastBoundLayerIndex != -1)
        {
            mList[this.lastBoundLayerIndex] = null;
            int num = 0;
            for (int i = 0; i < mListCount; i++)
            {
                if (mListSort[i] != this.lastBoundLayerIndex)
                {
                    mListSort[num++] = mListSort[i];
                }
            }
            mListCount = num;
            this.lastBoundLayerIndex = -1;
        }
    }

    public static void Render()
    {
        for (int i = 0; i < mListCount; i++)
        {
            if (((mList[i] != null) && mList[i].enabled) && ((mList[i].camera != null) && !mList[i].camera.enabled))
            {
                mList[i].camera.Render();
            }
        }
    }

    internal bool SetKeyboardFocus(UIInput input)
    {
        if (mSelInput == input)
        {
            return true;
        }
        if (mSelInput != null)
        {
            return false;
        }
        return ((input != null) && SetSelectedObject(input.gameObject));
    }

    public static bool SetSelectedObject(GameObject value)
    {
        if (mSel != value)
        {
            if (inSelectionCallback)
            {
                return false;
            }
            UIInput input = (value == null) ? null : value.GetComponent<UIInput>();
            if (mSelInput != input)
            {
                if (mSelInput != null)
                {
                    mSelInput.LoseFocus();
                }
                mSelInput = input;
                if ((input != null) && (mPressInput != input))
                {
                    input.GainFocus();
                }
            }
            if (mSel != null)
            {
                UICamera camera = FindCameraForLayer(mSel.layer);
                if (camera != null)
                {
                    Camera currentCamera = UICamera.currentCamera;
                    try
                    {
                        UICamera.currentCamera = camera.mCam;
                        inSelectionCallback = true;
                        mSel.Select(false);
                        if (camera.useController || camera.useKeyboard)
                        {
                            Highlight(mSel, false);
                        }
                    }
                    finally
                    {
                        UICamera.currentCamera = currentCamera;
                        inSelectionCallback = false;
                    }
                }
            }
            mSel = value;
            if (mSel != null)
            {
                UICamera camera3 = FindCameraForLayer(mSel.layer);
                if (camera3 != null)
                {
                    UICamera.currentCamera = camera3.mCam;
                    if (camera3.useController || camera3.useKeyboard)
                    {
                        Highlight(mSel, true);
                    }
                    mSel.Select(true);
                }
            }
        }
        return true;
    }

    public void ShowTooltip(bool val)
    {
        this.mTooltipTime = 0f;
        if (this.mTooltip != null)
        {
            this.mTooltip.Tooltip(val);
        }
        if (!val)
        {
            this.mTooltip = null;
        }
    }

    public static bool UnPopupPanel(UIPanel panel)
    {
        if ((popupPanel == panel) && (panel != null))
        {
            popupPanel.gameObject.NGUIMessage("PopupEnd");
            popupPanel = null;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (Application.isPlaying && this.handlesEvents)
        {
            if (mSel != null)
            {
                this.ProcessOthers();
            }
            else
            {
                inputHasFocus = false;
            }
            if (this.useMouse && (mHover != null))
            {
                if ((this.mouseMode & UIInputMode.UseInput) == UIInputMode.UseInput)
                {
                    float axis = Input.GetAxis(this.scrollAxisName);
                    if (axis != 0f)
                    {
                        mHover.Scroll(axis);
                    }
                }
                if ((this.mTooltipTime != 0f) && (this.mTooltipTime < Time.realtimeSinceStartup))
                {
                    this.mTooltip = mHover;
                    this.ShowTooltip(true);
                }
            }
        }
    }

    public Camera cachedCamera
    {
        get
        {
            if (this.mCam == null)
            {
                this.mCam = base.camera;
            }
            return this.mCam;
        }
    }

    public static CursorSampler Cursor
    {
        get
        {
            return LateLoadCursor.Sampler;
        }
    }

    public static UICamera eventHandler
    {
        get
        {
            return mList[mListSort[0]];
        }
    }

    private bool handlesEvents
    {
        get
        {
            return (eventHandler == this);
        }
    }

    public static GameObject hoveredObject
    {
        get
        {
            return mHover;
        }
    }

    public static bool IsPressing
    {
        get
        {
            return (Cursor.Buttons.LeftValue.Held && ((bool) Cursor.Buttons.LeftValue.Pressed));
        }
    }

    [Obsolete("Use UICamera.currentCamera instead")]
    public static Camera lastCamera
    {
        get
        {
            return currentCamera;
        }
    }

    [Obsolete("Use UICamera.currentTouchID instead")]
    public static int lastTouchID
    {
        get
        {
            return currentTouchID;
        }
    }

    public static Camera mainCamera
    {
        get
        {
            UICamera eventHandler = UICamera.eventHandler;
            return ((eventHandler == null) ? null : eventHandler.cachedCamera);
        }
    }

    public static GameObject selectedObject
    {
        get
        {
            return mSel;
        }
        set
        {
            if (!SetSelectedObject(value))
            {
                throw new InvalidOperationException("Do not set selectedObject within a OnSelect message.");
            }
        }
    }

    public bool usesAnyEvents
    {
        get
        {
            return ((((this.mouseMode | this.keyboardMode) | this.scrollWheelMode) & UIInputMode.UseEvents) == UIInputMode.UseEvents);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size=1)]
    public struct BackwardsCompatabilitySupport
    {
        public UICamera.ClickNotification clickNotification
        {
            get
            {
                return UICamera.Cursor.Buttons.LeftValue.ClickNotification;
            }
            set
            {
                UICamera.Cursor.Buttons.LeftValue.ClickNotification = value;
            }
        }
        public Vector2 pos
        {
            get
            {
                return ((UICamera.Cursor.CurrentButton != null) ? (UICamera.Cursor.CurrentButton.Point + UICamera.Cursor.CurrentButton.TotalDelta) : UICamera.Cursor.Current.Mouse.Point);
            }
        }
        public Vector2 delta
        {
            get
            {
                return UICamera.Cursor.FrameDelta;
            }
        }
        public Vector2 totalDelta
        {
            get
            {
                return UICamera.Cursor.Buttons.LeftValue.TotalDelta;
            }
        }
        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return -1;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.clickNotification, this.pos, this.delta, this.totalDelta };
            return string.Format("[BackwardsCompatabilitySupport: clickNotification={0}, pos={1}, delta={2}, totalDelta={3}]", args);
        }

        public static bool operator ==(UICamera.BackwardsCompatabilitySupport b, bool? s)
        {
            return (UICamera.Cursor.Current.Valid == s.HasValue);
        }

        public static bool operator !=(UICamera.BackwardsCompatabilitySupport b, bool? s)
        {
            return (UICamera.Cursor.Current.Valid != s.HasValue);
        }
    }

    private class CamSorter : Comparer<byte>
    {
        public override int Compare(byte a, byte b)
        {
            return UICamera.mList[b].cachedCamera.depth.CompareTo(UICamera.mList[a].cachedCamera.depth);
        }
    }

    public enum ClickNotification
    {
        None,
        Always,
        BasedOnDelta
    }

    public sealed class CursorSampler
    {
        public UICamera.Mouse.Button.ValCollection<UICamera.Mouse.Button.Sampler> Buttons;
        public Sample Current;
        public UICamera.Mouse.Button.Sampler CurrentButton;
        public bool Dragging;
        private GameObject DragHover;
        public DropNotificationFlags DropNotification;
        public bool IsCurrent;
        public bool IsFirst;
        public bool IsLast;
        private const float kDoubleClickLimit = 0.25f;
        public Sample Last;
        public float LastClickTime = float.MaxValue;
        private DropNotificationFlags LastHoverDropNotification;
        private UIPanel Panel;
        private DropNotificationFlags PressDropNotification;

        public CursorSampler()
        {
            this.Buttons.LeftValue = new UICamera.Mouse.Button.Sampler(UICamera.Mouse.Button.Flags.Left, this);
            this.Buttons.RightValue = new UICamera.Mouse.Button.Sampler(UICamera.Mouse.Button.Flags.Right, this);
            this.Buttons.MiddleValue = new UICamera.Mouse.Button.Sampler(UICamera.Mouse.Button.Flags.Middle, this);
        }

        private void CheckDragHover(bool HasCurrent, GameObject Current, GameObject Pressed)
        {
            if (HasCurrent)
            {
                if (this.DragHover != Current)
                {
                    if ((this.DragHover != null) && (this.DragHover != Pressed))
                    {
                        ExitDragHover(Pressed, this.DragHover, this.LastHoverDropNotification);
                    }
                    this.DragHover = Current;
                    if (Current != Pressed)
                    {
                        this.LastHoverDropNotification = this.DropNotification;
                        EnterDragHover(Pressed, this.DragHover, this.LastHoverDropNotification);
                    }
                }
            }
            else
            {
                this.ClearDragHover(Pressed);
            }
        }

        private void ClearDragHover(GameObject Pressed)
        {
            if (this.DragHover != null)
            {
                if (this.DragHover != Pressed)
                {
                    ExitDragHover(Pressed, this.DragHover, this.LastHoverDropNotification);
                }
                this.DragHover = null;
            }
        }

        private static void EnterDragHover(GameObject lander, GameObject drop, DropNotificationFlags flags)
        {
            if ((flags & DropNotificationFlags.ReverseHover) == DropNotificationFlags.ReverseHover)
            {
                if ((flags & DropNotificationFlags.LandHover) == DropNotificationFlags.LandHover)
                {
                    lander.NGUIMessage("OnLandHoverEnter", drop);
                }
                if ((flags & DropNotificationFlags.DragHover) == DropNotificationFlags.DragHover)
                {
                    drop.NGUIMessage("OnDragHoverEnter", lander);
                }
            }
            else
            {
                if ((flags & DropNotificationFlags.DragHover) == DropNotificationFlags.DragHover)
                {
                    drop.NGUIMessage("OnDragHoverEnter", lander);
                }
                if ((flags & DropNotificationFlags.LandHover) == DropNotificationFlags.LandHover)
                {
                    lander.NGUIMessage("OnLandHoverEnter", drop);
                }
            }
        }

        private static void ExitDragHover(GameObject lander, GameObject drop, DropNotificationFlags flags)
        {
            if ((flags & DropNotificationFlags.ReverseHover) == DropNotificationFlags.ReverseHover)
            {
                if ((flags & DropNotificationFlags.DragHover) == DropNotificationFlags.DragHover)
                {
                    drop.NGUIMessage("OnDragHoverExit", lander);
                }
                if ((flags & DropNotificationFlags.LandHover) == DropNotificationFlags.LandHover)
                {
                    lander.NGUIMessage("OnLandHoverExit", drop);
                }
            }
            else
            {
                if ((flags & DropNotificationFlags.LandHover) == DropNotificationFlags.LandHover)
                {
                    lander.NGUIMessage("OnLandHoverExit", drop);
                }
                if ((flags & DropNotificationFlags.DragHover) == DropNotificationFlags.DragHover)
                {
                    drop.NGUIMessage("OnDragHoverExit", lander);
                }
            }
        }

        internal void MouseEvent(Event @event, EventType type)
        {
            Sample sample;
            sample.Mouse.Scroll = new Vector2();
            sample.Mouse.Buttons.Pressed = UICamera.Mouse.Button.Held | UICamera.Mouse.Button.NewlyPressed;
            sample.Mouse.Point = @event.mousePosition;
            if (this.Current.Valid)
            {
                sample.IsFirst = false;
                if (this.Current.Mouse.Point.x != sample.Mouse.Point.x)
                {
                    sample.Mouse.Delta.x = sample.Mouse.Point.x - this.Current.Mouse.Point.x;
                    if (this.Current.Mouse.Point.y != sample.Mouse.Point.y)
                    {
                        sample.Mouse.Delta.y = sample.Mouse.Point.y - this.Current.Mouse.Point.y;
                    }
                    else
                    {
                        sample.Mouse.Delta.y = 0f;
                    }
                    sample.DidMove = true;
                }
                else if (this.Current.Mouse.Point.y != sample.Mouse.Point.y)
                {
                    sample.Mouse.Delta.x = 0f;
                    sample.Mouse.Delta.y = sample.Mouse.Point.y - this.Current.Mouse.Point.y;
                    sample.DidMove = true;
                }
                else
                {
                    sample.DidMove = false;
                    sample.Mouse.Delta.x = sample.Mouse.Delta.y = 0f;
                }
            }
            else
            {
                sample.IsFirst = true;
                sample.DidMove = false;
                sample.Mouse.Delta.x = sample.Mouse.Delta.y = 0f;
            }
            sample.Hit = UIHotSpot.Hit.invalid;
            if (sample.DidHit = UICamera.Raycast((Vector3) sample.Mouse.Point, ref sample.Hit, out sample.UICamera))
            {
                UICamera.lastHit = sample.Hit;
                sample.Under = sample.Hit.gameObject;
                sample.HasUnder = true;
            }
            else if (UICamera.fallThrough != null)
            {
                sample.Under = UICamera.fallThrough;
                sample.HasUnder = true;
                sample.UICamera = UICamera.FindCameraForLayer(UICamera.fallThrough.layer);
                if (sample.UICamera == null)
                {
                    sample.UICamera = (sample.IsFirst || (this.Current.UICamera == null)) ? UICamera.mList[UICamera.mListSort[0]] : this.Current.UICamera;
                }
            }
            else
            {
                sample.Under = null;
                sample.HasUnder = false;
                sample.UICamera = (sample.IsFirst || (this.Current.UICamera == null)) ? UICamera.mList[UICamera.mListSort[0]] : this.Current.UICamera;
            }
            sample.UnderChange = sample.IsFirst || (!sample.HasUnder ? this.Current.HasUnder : (!this.Current.HasUnder || (this.Current.Under != sample.Under)));
            sample.HoverChange = sample.UnderChange && (sample.Under != UICamera.mHover);
            sample.ButtonChange = UICamera.Mouse.Button.AnyNewlyPressedOrReleased;
            bool flag = false;
            if (sample.ButtonChange && UICamera.Mouse.Button.AnyNewlyPressedThatCancelTooltips)
            {
                sample.UICamera.mTooltipTime = 0f;
            }
            else
            {
                if (sample.DidMove && (sample.HoverChange || !sample.UICamera.stickyTooltip))
                {
                    if (sample.UICamera.mTooltipTime != 0f)
                    {
                        sample.UICamera.mTooltipTime = Time.realtimeSinceStartup + sample.UICamera.tooltipDelay;
                    }
                    else if (sample.UICamera.mTooltip != null)
                    {
                        flag = true;
                        sample.UICamera.ShowTooltip(false);
                    }
                }
                if (sample.HoverChange && (UICamera.mHover != null))
                {
                    if (sample.UICamera.mTooltip != null)
                    {
                        sample.UICamera.ShowTooltip(false);
                    }
                    UICamera.Highlight(UICamera.mHover, false);
                    UICamera.mHover = null;
                }
            }
            sample.Time = Time.realtimeSinceStartup;
            sample.ButtonsPressed = UICamera.Mouse.Button.NewlyPressed;
            sample.ButtonsReleased = UICamera.Mouse.Button.NewlyReleased;
            if ((!flag && (sample.ButtonsPressed != 0)) && (sample.UICamera.mTooltip != null))
            {
                sample.UICamera.ShowTooltip(false);
                flag = true;
            }
            for (UICamera.Mouse.Button.Flags flags = UICamera.Mouse.Button.Flags.Left; flags < (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left); flags = (UICamera.Mouse.Button.Flags) (((int) flags) << 1))
            {
                UICamera.Mouse.Button.Sampler sampler = this.Buttons[flags];
                try
                {
                    this.CurrentButton = sampler;
                    sampler.PressedNow = sampler.ReleasedNow = false;
                    if ((sample.ButtonsPressed & flags) == flags)
                    {
                        float releaseTime;
                        if (sampler.Once)
                        {
                            releaseTime = sampler.ReleaseTime;
                        }
                        else
                        {
                            releaseTime = sampler.ReleaseTime = sample.Time - 120f;
                            sampler.Once = true;
                        }
                        sampler.PressTime = sample.Time;
                        sampler.Pressed = sample.Under;
                        sampler.DidHit = sample.DidHit;
                        sampler.PressedNow = true;
                        sampler.Hit = sample.Hit;
                        sampler.ReleasedNow = false;
                        sampler.Held = true;
                        sampler.Point = sample.Mouse.Point;
                        sampler.TotalDelta.x = sampler.TotalDelta.y = 0f;
                        sampler.ClickNotification = UICamera.ClickNotification.Always;
                        if (flags == UICamera.Mouse.Button.Flags.Left)
                        {
                            this.Dragging = false;
                            this.DropNotification = DropNotificationFlags.DragDrop;
                            sampler.DragClick = false;
                            sampler.DragClickNumber = 0L;
                        }
                        else if (this.Dragging)
                        {
                            sampler.DragClick = true;
                            sampler.DragClickNumber = this.Buttons.LeftValue.ClickCount;
                        }
                        else
                        {
                            sampler.DragClick = false;
                            sampler.DragClickNumber = 0L;
                        }
                        if (sample.DidHit)
                        {
                            if (flags == UICamera.Mouse.Button.Flags.Left)
                            {
                                UICamera.mPressInput = sample.Under.GetComponent<UIInput>();
                                if (UICamera.mSelInput != null)
                                {
                                    if (UICamera.mPressInput != null)
                                    {
                                        if (UICamera.mSelInput == UICamera.mPressInput)
                                        {
                                            UICamera.mSelInput.OnEvent(sample.UICamera, @event, type);
                                        }
                                        else
                                        {
                                            UICamera.mSelInput.LoseFocus();
                                            UICamera.mSelInput = null;
                                            UICamera.mPressInput.GainFocus();
                                            UICamera.mPressInput.OnEvent(sample.UICamera, @event, type);
                                        }
                                    }
                                    else
                                    {
                                        UICamera.mSelInput.LoseFocus();
                                        UICamera.mSelInput = null;
                                    }
                                }
                                else if (UICamera.mPressInput != null)
                                {
                                    UICamera.mPressInput.GainFocus();
                                    UICamera.mPressInput.OnEvent(sample.UICamera, @event, type);
                                }
                                if ((UICamera.mSel != null) && (UICamera.mSel != sample.Under))
                                {
                                    if (!flag && (sample.UICamera.mTooltip != null))
                                    {
                                        sample.UICamera.ShowTooltip(false);
                                    }
                                    UICamera.SetSelectedObject(null);
                                }
                                this.Panel = UIPanel.FindRoot(sample.Under.transform);
                                if (this.Panel != null)
                                {
                                    if ((this.Panel != UICamera.popupPanel) && (UICamera.popupPanel != null))
                                    {
                                        UICamera.PopupPanel(null);
                                    }
                                    sample.Under.Press(true);
                                    this.Panel.gameObject.NGUIMessage("OnChildPress", true);
                                }
                                else
                                {
                                    if (UICamera.popupPanel != null)
                                    {
                                        UICamera.PopupPanel(null);
                                    }
                                    sample.Under.Press(true);
                                }
                                this.PressDropNotification = this.DropNotification;
                            }
                            else
                            {
                                if (UICamera.mSelInput != null)
                                {
                                    UICamera.mSelInput.OnEvent(sample.UICamera, @event, type);
                                }
                                if (!sampler.DragClick)
                                {
                                    switch (flags)
                                    {
                                        case UICamera.Mouse.Button.Flags.Right:
                                        {
                                            UIPanel panel = UIPanel.FindRoot(sample.Under.transform);
                                            if ((UICamera.popupPanel != null) && (UICamera.popupPanel != panel))
                                            {
                                                UICamera.PopupPanel(null);
                                            }
                                            sample.Under.AltPress(true);
                                            break;
                                        }
                                        case UICamera.Mouse.Button.Flags.Middle:
                                            sample.Under.MidPress(true);
                                            break;
                                    }
                                }
                            }
                            @event.Use();
                            continue;
                        }
                        if (flags == UICamera.Mouse.Button.Flags.Left)
                        {
                            if (UICamera.popupPanel != null)
                            {
                                UICamera.PopupPanel(null);
                            }
                            UICamera.mPressInput = null;
                            if (UICamera.mSelInput != null)
                            {
                                UICamera.mSelInput.LoseFocus();
                                UICamera.mSelInput = null;
                            }
                            if (UICamera.mSel != null)
                            {
                                if (!flag && (sample.UICamera.mTooltip != null))
                                {
                                    sample.UICamera.ShowTooltip(false);
                                }
                                UICamera.SetSelectedObject(null);
                            }
                        }
                        continue;
                    }
                    if (sampler.Held && sampler.DidHit)
                    {
                        if ((type == EventType.MouseDrag) && (flags == UICamera.Mouse.Button.Flags.Left))
                        {
                            if (UICamera.mPressInput != null)
                            {
                                UICamera.mPressInput.OnEvent(sample.UICamera, @event, type);
                            }
                            @event.Use();
                        }
                        if (sample.DidMove)
                        {
                            if (!flag && (sample.UICamera.mTooltip != null))
                            {
                                sample.UICamera.ShowTooltip(false);
                            }
                            sampler.TotalDelta.x += sample.Mouse.Delta.x;
                            sampler.TotalDelta.y += sample.Mouse.Delta.y;
                            bool flag2 = sampler.ClickNotification == UICamera.ClickNotification.None;
                            if (((flags == UICamera.Mouse.Button.Flags.Left) && !sampler.DragClick) && ((this.PressDropNotification & (DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop)) != 0))
                            {
                                if (!this.Dragging)
                                {
                                    sampler.Pressed.DragState(true);
                                    this.Dragging = true;
                                }
                                sampler.Pressed.Drag(sample.Mouse.Delta);
                                this.CheckDragHover(sample.DidHit, sample.Under, sampler.Pressed);
                            }
                            if (flag2)
                            {
                                sampler.ClickNotification = UICamera.ClickNotification.None;
                            }
                            else if (sampler.ClickNotification == UICamera.ClickNotification.BasedOnDelta)
                            {
                                float mouseClickThreshold;
                                if (flags == UICamera.Mouse.Button.Flags.Left)
                                {
                                    mouseClickThreshold = sample.UICamera.mouseClickThreshold;
                                }
                                else
                                {
                                    mouseClickThreshold = Screen.height * 0.1f;
                                    if (mouseClickThreshold < sample.UICamera.touchClickThreshold)
                                    {
                                        mouseClickThreshold = sample.UICamera.touchClickThreshold;
                                    }
                                }
                                if (((sampler.TotalDelta.x * sampler.TotalDelta.x) + (sampler.TotalDelta.y * sampler.TotalDelta.y)) > (mouseClickThreshold * mouseClickThreshold))
                                {
                                    sampler.ClickNotification = UICamera.ClickNotification.None;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this.CurrentButton = null;
                }
            }
            for (UICamera.Mouse.Button.Flags flags2 = UICamera.Mouse.Button.Flags.Middle; flags2 != 0; flags2 = (UICamera.Mouse.Button.Flags) (((int) flags2) >> 1))
            {
                UICamera.Mouse.Button.Sampler sampler2 = this.Buttons[flags2];
                try
                {
                    this.CurrentButton = sampler2;
                    if ((sample.ButtonsReleased & flags2) != flags2)
                    {
                        continue;
                    }
                    sampler2.ReleasedNow = true;
                    if (sampler2.DidHit)
                    {
                        if (flags2 == UICamera.Mouse.Button.Flags.Left)
                        {
                            if (((type == EventType.MouseUp) || (type == EventType.KeyUp)) && ((UICamera.mPressInput != null) && (sampler2.Pressed == UICamera.mPressInput.gameObject)))
                            {
                                UICamera.mPressInput.OnEvent(sample.UICamera, @event, type);
                                UICamera.mSelInput = UICamera.mPressInput;
                            }
                            UICamera.mPressInput = null;
                            if (sample.HasUnder)
                            {
                                if (sampler2.Pressed == sample.Under)
                                {
                                    if (this.Dragging && ((this.PressDropNotification & (DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop)) != 0))
                                    {
                                        this.ClearDragHover(sampler2.Pressed);
                                        if (!sampler2.DragClick)
                                        {
                                            sampler2.Pressed.DragState(false);
                                        }
                                    }
                                    if (this.Panel != null)
                                    {
                                        this.Panel.gameObject.NGUIMessage("OnChildPress", false);
                                    }
                                    sampler2.Pressed.Press(false);
                                    if (sampler2.Pressed == UICamera.mHover)
                                    {
                                        sampler2.Pressed.Hover(true);
                                    }
                                    if (sampler2.Pressed != UICamera.mSel)
                                    {
                                        UICamera.mSel = sampler2.Pressed;
                                        sampler2.Pressed.Select(true);
                                    }
                                    else
                                    {
                                        UICamera.mSel = sampler2.Pressed;
                                    }
                                    if (!sampler2.DragClick && (sampler2.ClickNotification != UICamera.ClickNotification.None))
                                    {
                                        if (this.Panel != null)
                                        {
                                            this.Panel.gameObject.NGUIMessage("OnChildClick", sampler2.Pressed);
                                        }
                                        if (sampler2.ClickNotification != UICamera.ClickNotification.None)
                                        {
                                            sampler2.Pressed.Click();
                                            if ((sampler2.ClickNotification != UICamera.ClickNotification.None) && ((sampler2.ReleaseTime + 0.25f) > sample.Time))
                                            {
                                                sampler2.Pressed.DoubleClick();
                                            }
                                        }
                                    }
                                    else if (this.Panel != null)
                                    {
                                        this.Panel.gameObject.NGUIMessage("OnChildClickCanceled", sampler2.Pressed);
                                    }
                                }
                                else
                                {
                                    if ((this.Dragging && !sampler2.DragClick) && ((this.PressDropNotification & (DropNotificationFlags.MidLand | DropNotificationFlags.MidDrop | DropNotificationFlags.AltLand | DropNotificationFlags.AltDrop | DropNotificationFlags.DragLand | DropNotificationFlags.DragDrop)) != 0))
                                    {
                                        DropNotification.DropMessage(ref this.DropNotification, DragEventKind.Drag, sampler2.Pressed, sample.Under);
                                        this.ClearDragHover(sampler2.Pressed);
                                        sampler2.Pressed.DragState(false);
                                    }
                                    if (this.Panel != null)
                                    {
                                        this.Panel.gameObject.NGUIMessage("OnChildPress", false);
                                    }
                                    sampler2.Pressed.Press(false);
                                    if (sampler2.Pressed == UICamera.mHover)
                                    {
                                        sampler2.Pressed.Hover(true);
                                    }
                                }
                            }
                            else if (this.Dragging)
                            {
                                this.ClearDragHover(sampler2.Pressed);
                                if (!sampler2.DragClick)
                                {
                                    DropNotification.DropMessage(ref this.DropNotification, DragEventKind.Drag, sampler2.Pressed, sample.Under);
                                    sampler2.Pressed.DragState(false);
                                }
                                if (this.Panel != null)
                                {
                                    this.Panel.gameObject.NGUIMessage("OnChildPress", false);
                                }
                                sampler2.Pressed.Press(false);
                                if (sampler2.Pressed == UICamera.mHover)
                                {
                                    sampler2.Pressed.Hover(true);
                                }
                                this.Dragging = false;
                            }
                        }
                        else if (sampler2.DragClick)
                        {
                            if (!this.Buttons.LeftValue.DragClick && (this.Buttons.LeftValue.ClickCount == sampler2.DragClickNumber))
                            {
                                bool flag3;
                                switch (flags2)
                                {
                                    case UICamera.Mouse.Button.Flags.Right:
                                        flag3 = DropNotification.DropMessage(ref this.DropNotification, DragEventKind.Alt, this.Buttons.LeftValue.Pressed, sampler2.Pressed);
                                        break;

                                    case UICamera.Mouse.Button.Flags.Middle:
                                        flag3 = DropNotification.DropMessage(ref this.DropNotification, DragEventKind.Mid, this.Buttons.LeftValue.Pressed, sampler2.Pressed);
                                        break;

                                    default:
                                        flag3 = false;
                                        break;
                                }
                                if (flag3)
                                {
                                    this.Buttons.LeftValue.DragClick = true;
                                    this.ClearDragHover(this.Buttons.LeftValue.Pressed);
                                    sampler2.Pressed.DragState(false);
                                }
                            }
                        }
                        else
                        {
                            switch (flags2)
                            {
                                case UICamera.Mouse.Button.Flags.Right:
                                    sampler2.Pressed.AltPress(false);
                                    if ((sample.HasUnder && (sampler2.Pressed == sample.Under)) && (sampler2.ClickNotification != UICamera.ClickNotification.None))
                                    {
                                        sampler2.Pressed.AltClick();
                                        if ((sampler2.ClickNotification != UICamera.ClickNotification.None) && ((sampler2.ReleaseTime + 0.25f) > sample.Time))
                                        {
                                            sampler2.Pressed.AltDoubleClick();
                                        }
                                    }
                                    break;

                                case UICamera.Mouse.Button.Flags.Middle:
                                    sampler2.Pressed.MidPress(false);
                                    if ((sample.HasUnder && (sampler2.Pressed == sample.Under)) && (sampler2.ClickNotification != UICamera.ClickNotification.None))
                                    {
                                        sampler2.Pressed.MidClick();
                                        if ((sampler2.ClickNotification != UICamera.ClickNotification.None) && ((sampler2.ReleaseTime + 0.25f) > sample.Time))
                                        {
                                            sampler2.Pressed.MidDoubleClick();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    sampler2.ReleasedNow = true;
                    sampler2.ClickNotification = UICamera.ClickNotification.None;
                    sampler2.ReleaseTime = sample.Time;
                    sampler2.Held = false;
                    sampler2.ClickCount += (ulong) 1L;
                    sampler2.DragClick = false;
                    sampler2.DragClickNumber = 0L;
                    if (flags2 == UICamera.Mouse.Button.Flags.Left)
                    {
                        this.Dragging = false;
                        this.Panel = null;
                    }
                    if ((@event.type == EventType.MouseUp) || (@event.type == EventType.KeyUp))
                    {
                        @event.Use();
                    }
                }
                finally
                {
                    this.CurrentButton = null;
                }
            }
            UICamera.lastMousePosition = !sample.IsFirst ? this.Current.Mouse.Point : sample.Mouse.Point;
            if ((sample.HasUnder && (sample.Mouse.Buttons.NonePressed || (this.Dragging && ((this.DropNotification & DropNotificationFlags.RegularHover) == DropNotificationFlags.RegularHover)))) && (UICamera.mHover != sample.Under))
            {
                sample.UICamera.mTooltipTime = sample.Time + sample.UICamera.tooltipDelay;
                UICamera.mHover = sample.Under;
                UICamera.Highlight(UICamera.mHover, true);
            }
            sample.Valid = true;
            this.Last = this.Current;
            this.Current = sample;
        }

        public Vector2 FrameDelta
        {
            get
            {
                return this.Current.Mouse.Delta;
            }
        }

        public Vector2 Point
        {
            get
            {
                return this.Current.Mouse.Point;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Sample
        {
            public GameObject Under;
            public UICamera UICamera;
            public UICamera.Mouse.State Mouse;
            public UIHotSpot.Hit Hit;
            public float Time;
            public bool DidHit;
            public bool HasUnder;
            public bool Valid;
            public bool DidMove;
            public bool IsFirst;
            public bool ButtonChange;
            public bool UnderChange;
            public bool HoverChange;
            public UICamera.Mouse.Button.Flags ButtonsPressed;
            public UICamera.Mouse.Button.Flags ButtonsReleased;
            public UnityEngine.Camera Camera
            {
                get
                {
                    return ((this.UICamera == null) ? null : this.UICamera.cachedCamera);
                }
            }
            public static bool operator true(UICamera.CursorSampler.Sample sample)
            {
                return sample.Valid;
            }

            public static bool operator false(UICamera.CursorSampler.Sample sample)
            {
                return !sample.Valid;
            }
        }
    }

    private class Highlighted
    {
        public int counter;
        public GameObject go;
    }

    private static class LateLoadCursor
    {
        public static readonly UICamera.CursorSampler Sampler = new UICamera.CursorSampler();
    }

    public static class Mouse
    {
        public static class Button
        {
            public const Flags All = (Flags.Middle | Flags.Right | Flags.Left);
            public const int Count = 3;
            private static Flags held;
            private static int indexPressed = -1;
            private static int indexReleased = -1;
            private const Flags kCancelsTooltips = (Flags.Middle | Flags.Right | Flags.Left);
            public const Flags Left = Flags.Left;
            public const Flags Middle = Flags.Middle;
            public const Flags Mouse0 = Flags.Left;
            public const Flags Mouse1 = Flags.Right;
            public const Flags Mouse2 = Flags.Middle;
            public const Flags None = 0;
            private static Flags pressed;
            private static Flags released;
            public const Flags Right = Flags.Right;

            public static Flags Index(int index)
            {
                if ((index < 0) || (index >= 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return (Flags) (((int) 1) << index);
            }

            internal static bool IsNewlyPressed(Flags flag)
            {
                return ((pressed & flag) == flag);
            }

            internal static bool IsNewlyReleased(Flags flag)
            {
                return ((released & flag) == flag);
            }

            public static bool AllowDrag
            {
                get
                {
                    return (held != 0);
                }
            }

            public static bool AllowMove
            {
                get
                {
                    return (((held | released) | pressed) == 0);
                }
            }

            internal static bool AnyNewlyPressed
            {
                get
                {
                    return (pressed != 0);
                }
            }

            internal static bool AnyNewlyPressedOrReleased
            {
                get
                {
                    return ((pressed | released) != 0);
                }
            }

            internal static bool AnyNewlyPressedThatCancelTooltips
            {
                get
                {
                    return ((pressed & (Flags.Middle | Flags.Right | Flags.Left)) != 0);
                }
            }

            internal static bool AnyNewlyReleased
            {
                get
                {
                    return (released != 0);
                }
            }

            internal static Flags Held
            {
                get
                {
                    return held;
                }
            }

            internal static Flags NewlyPressed
            {
                get
                {
                    return pressed;
                }
            }

            internal static Flags NewlyReleased
            {
                get
                {
                    return released;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ButtonPressEventHandler : IDisposable
            {
                private Event @event;
                public ButtonPressEventHandler(Event @event)
                {
                    this.@event = @event;
                    UICamera.Mouse.Button.pressed = UICamera.Mouse.Button.Index(@event.button);
                    UICamera.Mouse.Button.indexPressed = @event.button;
                }

                public void Dispose()
                {
                    if (UICamera.Mouse.Button.indexPressed != -1)
                    {
                        if (this.@event.type == EventType.Used)
                        {
                            UICamera.Mouse.Button.held |= UICamera.Mouse.Button.pressed;
                        }
                        UICamera.Mouse.Button.indexPressed = -1;
                        UICamera.Mouse.Button.pressed = 0;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ButtonReleaseEventHandler : IDisposable
            {
                private Event @event;
                public ButtonReleaseEventHandler(Event @event)
                {
                    this.@event = @event;
                    UICamera.Mouse.Button.released = UICamera.Mouse.Button.Index(@event.button);
                    UICamera.Mouse.Button.indexReleased = @event.button;
                }

                public void Dispose()
                {
                    if (UICamera.Mouse.Button.indexReleased != -1)
                    {
                        if (this.@event.type == EventType.Used)
                        {
                            UICamera.Mouse.Button.held &= ~UICamera.Mouse.Button.released;
                        }
                        UICamera.Mouse.Button.indexReleased = -1;
                        UICamera.Mouse.Button.released = 0;
                    }
                }
            }

            [Flags]
            public enum Flags
            {
                Left = 1,
                Middle = 4,
                Right = 2
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Pair<T>
            {
                public readonly UICamera.Mouse.Button.Flags Button;
                public readonly T Value;
                public Pair(UICamera.Mouse.Button.Flags Button, T Value)
                {
                    this.Button = Button;
                    this.Value = Value;
                }

                public Pair(UICamera.Mouse.Button.Flags Button, ref T Value)
                {
                    this.Button = Button;
                    this.Value = Value;
                }

                public Pair(UICamera.Mouse.Button.Flags Button) : this(Button, default(T))
                {
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PressState : IEnumerable, IEnumerable<UICamera.Mouse.Button.Flags>
            {
                public UICamera.Mouse.Button.Flags Pressed;
                IEnumerator<UICamera.Mouse.Button.Flags> IEnumerable<UICamera.Mouse.Button.Flags>.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public bool LeftPressed
                {
                    get
                    {
                        return ((this.Pressed & UICamera.Mouse.Button.Flags.Left) == UICamera.Mouse.Button.Flags.Left);
                    }
                    set
                    {
                        if (value)
                        {
                            this.Pressed |= UICamera.Mouse.Button.Flags.Left;
                        }
                        else
                        {
                            this.Pressed &= ~UICamera.Mouse.Button.Flags.Left;
                        }
                    }
                }
                public bool RightPressed
                {
                    get
                    {
                        return ((this.Pressed & UICamera.Mouse.Button.Flags.Right) == UICamera.Mouse.Button.Flags.Right);
                    }
                    set
                    {
                        if (value)
                        {
                            this.Pressed |= UICamera.Mouse.Button.Flags.Right;
                        }
                        else
                        {
                            this.Pressed &= ~UICamera.Mouse.Button.Flags.Right;
                        }
                    }
                }
                public bool MiddlePressed
                {
                    get
                    {
                        return ((this.Pressed & UICamera.Mouse.Button.Flags.Middle) == UICamera.Mouse.Button.Flags.Middle);
                    }
                    set
                    {
                        if (value)
                        {
                            this.Pressed |= UICamera.Mouse.Button.Flags.Middle;
                        }
                        else
                        {
                            this.Pressed &= ~UICamera.Mouse.Button.Flags.Middle;
                        }
                    }
                }
                public UICamera.Mouse.Button.Flags Released
                {
                    get
                    {
                        return (~this.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left));
                    }
                    set
                    {
                        this.Pressed = ~value & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left);
                    }
                }
                public bool AnyPressed
                {
                    get
                    {
                        return ((this.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left)) != 0);
                    }
                }
                public bool AllPressed
                {
                    get
                    {
                        return ((this.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left)) == (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left));
                    }
                }
                public bool NonePressed
                {
                    get
                    {
                        return ((this.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left)) == 0);
                    }
                }
                public int PressedCount
                {
                    get
                    {
                        int num = 0;
                        uint num2 = (uint) (this.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left));
                        while (num2 != 0)
                        {
                            num2 &= num2 - 1;
                            num++;
                        }
                        return num;
                    }
                }
                public bool LeftReleased
                {
                    get
                    {
                        return !this.LeftPressed;
                    }
                    set
                    {
                        this.LeftPressed = !value;
                    }
                }
                public bool RightReleased
                {
                    get
                    {
                        return !this.RightPressed;
                    }
                    set
                    {
                        this.RightPressed = !value;
                    }
                }
                public bool MiddleReleased
                {
                    get
                    {
                        return !this.MiddlePressed;
                    }
                    set
                    {
                        this.MiddlePressed = !value;
                    }
                }
                public bool AnyReleased
                {
                    get
                    {
                        return !this.AllPressed;
                    }
                }
                public bool AllReleased
                {
                    get
                    {
                        return !this.AnyPressed;
                    }
                }
                public bool NoneReleased
                {
                    get
                    {
                        return !this.AllPressed;
                    }
                }
                public void Clear()
                {
                    this.Pressed &= ~(UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left);
                }

                public bool this[int index]
                {
                    get
                    {
                        UICamera.Mouse.Button.Flags flags = UICamera.Mouse.Button.Index(index);
                        return ((this.Pressed & flags) == flags);
                    }
                    set
                    {
                        UICamera.Mouse.Button.Flags flags = UICamera.Mouse.Button.Index(index);
                        if (value)
                        {
                            this.Pressed |= flags;
                        }
                        else
                        {
                            this.Pressed &= ~flags;
                        }
                    }
                }
                public Enumerator GetEnumerator()
                {
                    return Enumerator.Enumerate(this.Pressed);
                }

                public static implicit operator UICamera.Mouse.Button.Flags(UICamera.Mouse.Button.PressState state)
                {
                    return (state.Pressed & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left));
                }

                public static implicit operator UICamera.Mouse.Button.PressState(UICamera.Mouse.Button.Flags buttons)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = buttons & (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left);
                    return state;
                }

                public static bool operator true(UICamera.Mouse.Button.PressState state)
                {
                    return state.AnyPressed;
                }

                public static bool operator false(UICamera.Mouse.Button.PressState state)
                {
                    return state.NonePressed;
                }

                public static UICamera.Mouse.Button.PressState operator -(UICamera.Mouse.Button.PressState s)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = s.Released;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator +(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed | r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator +(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.Flags r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed | r;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator +(UICamera.Mouse.Button.Flags l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l | r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator -(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed & ~r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator -(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.Flags r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed & ~r;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator -(UICamera.Mouse.Button.Flags l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l & ~r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator *(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed & r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator *(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.Flags r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed & r;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator *(UICamera.Mouse.Button.Flags l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l & r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator /(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed ^ r.Pressed;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator /(UICamera.Mouse.Button.PressState l, UICamera.Mouse.Button.Flags r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l.Pressed ^ r;
                    return state;
                }

                public static UICamera.Mouse.Button.PressState operator /(UICamera.Mouse.Button.Flags l, UICamera.Mouse.Button.PressState r)
                {
                    UICamera.Mouse.Button.PressState state;
                    state.Pressed = l ^ r.Pressed;
                    return state;
                }
                public class Enumerator : IDisposable, IEnumerator, IEnumerator<UICamera.Mouse.Button.Flags>
                {
                    private static readonly UICamera.Mouse.Button.Flags[][] combos = new UICamera.Mouse.Button.Flags[8][];
                    private static UICamera.Mouse.Button.PressState.Enumerator dump;
                    private static uint dumpCount;
                    private UICamera.Mouse.Button.Flags[] flags;
                    private bool inDump;
                    private UICamera.Mouse.Button.PressState.Enumerator nextDump;
                    private int pos;
                    private UICamera.Mouse.Button.Flags value;

                    static Enumerator()
                    {
                        for (UICamera.Mouse.Button.Flags flags = 0; flags <= (UICamera.Mouse.Button.Flags.Middle | UICamera.Mouse.Button.Flags.Right | UICamera.Mouse.Button.Flags.Left); flags += 1)
                        {
                            int num = 0;
                            uint num2 = (uint) flags;
                            while (num2 != 0)
                            {
                                num2 &= num2 - 1;
                                num++;
                            }
                            UICamera.Mouse.Button.Flags[] flagsArray = new UICamera.Mouse.Button.Flags[num];
                            int num3 = 0;
                            for (int i = 0; (i < 3) && (num3 < num); i++)
                            {
                                if ((flags & (((int) 1) << i)) == (((int) 1) << i))
                                {
                                    flagsArray[num3++] = (UICamera.Mouse.Button.Flags) (((int) 1) << i);
                                }
                            }
                            combos[(int) flags] = flagsArray;
                        }
                    }

                    private Enumerator()
                    {
                    }

                    public void Dispose()
                    {
                        if (!this.inDump)
                        {
                            this.nextDump = dump;
                            this.inDump = true;
                            dump = this;
                            dumpCount++;
                        }
                    }

                    public static UICamera.Mouse.Button.PressState.Enumerator Enumerate(UICamera.Mouse.Button.Flags flags)
                    {
                        UICamera.Mouse.Button.PressState.Enumerator dump;
                        if (dumpCount == 0)
                        {
                            dump = new UICamera.Mouse.Button.PressState.Enumerator();
                        }
                        else
                        {
                            dump = UICamera.Mouse.Button.PressState.Enumerator.dump;
                            UICamera.Mouse.Button.PressState.Enumerator.dump = dump.nextDump;
                            dumpCount--;
                            dump.nextDump = null;
                        }
                        dump.pos = -1;
                        dump.value = flags;
                        dump.inDump = false;
                        dump.flags = combos[(int) flags];
                        return dump;
                    }

                    public bool MoveNext()
                    {
                        return (++this.pos < this.flags.Length);
                    }

                    public void Reset()
                    {
                        this.pos = -1;
                    }

                    public UICamera.Mouse.Button.Flags Current
                    {
                        get
                        {
                            return this.flags[this.pos];
                        }
                    }

                    object IEnumerator.Current
                    {
                        get
                        {
                            return this.flags[this.pos];
                        }
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RefCollection<T> : IEnumerable, IEnumerable<UICamera.Mouse.Button.Pair<T>>
            {
                public T LeftValue;
                public T RightValue;
                public T MiddleValue;
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public T this[UICamera.Mouse.Button.Flags button]
                {
                    get
                    {
                        switch (button)
                        {
                            case UICamera.Mouse.Button.Flags.Left:
                                return this.LeftValue;

                            case UICamera.Mouse.Button.Flags.Right:
                                return this.RightValue;

                            case UICamera.Mouse.Button.Flags.Middle:
                                return this.MiddleValue;
                        }
                        throw new ArgumentOutOfRangeException("button", "button should not be None or a Combination of multiple buttons");
                    }
                    set
                    {
                        switch (button)
                        {
                            case UICamera.Mouse.Button.Flags.Left:
                                this.LeftValue = value;
                                return;

                            case UICamera.Mouse.Button.Flags.Right:
                                this.RightValue = value;
                                return;

                            case UICamera.Mouse.Button.Flags.Middle:
                                this.MiddleValue = value;
                                return;
                        }
                        throw new ArgumentOutOfRangeException("button", "button should not be None or a Combination of multiple buttons");
                    }
                }
                public T this[int i]
                {
                    get
                    {
                        return this[UICamera.Mouse.Button.Flags.Left];
                    }
                    set
                    {
                        this[UICamera.Mouse.Button.Flags.Left] = value;
                    }
                }
                public IEnumerable<UICamera.Mouse.Button.Pair<T>> this[UICamera.Mouse.Button.PressState state]
                {
                    get
                    {
                        return new <>c__Iterator4D<T> { state = state, <$>state = state, <>f__this = *((UICamera.Mouse.Button.RefCollection<T>*) this), $PC = -2 };
                    }
                }
                [DebuggerHidden]
                public unsafe IEnumerator<UICamera.Mouse.Button.Pair<T>> GetEnumerator()
                {
                    return new <GetEnumerator>c__Iterator4E<T> { <>f__this = *((UICamera.Mouse.Button.RefCollection<T>*) this) };
                }
                [CompilerGenerated]
                private sealed class <>c__Iterator4D : IDisposable, IEnumerator, IEnumerable, IEnumerable<UICamera.Mouse.Button.Pair<T>>, IEnumerator<UICamera.Mouse.Button.Pair<T>>
                {
                    internal UICamera.Mouse.Button.Pair<T> $current;
                    internal int $PC;
                    internal UICamera.Mouse.Button.PressState <$>state;
                    internal UICamera.Mouse.Button.PressState.Enumerator <$s_598>__0;
                    internal UICamera.Mouse.Button.RefCollection<T> <>f__this;
                    internal UICamera.Mouse.Button.Flags <flag>__1;
                    internal UICamera.Mouse.Button.PressState state;

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
                                    if (this.<$s_598>__0 == null)
                                    {
                                    }
                                    this.<$s_598>__0.Dispose();
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
                                this.<$s_598>__0 = this.state.GetEnumerator();
                                num = 0xfffffffd;
                                break;

                            case 1:
                                break;

                            default:
                                goto Label_00BE;
                        }
                        try
                        {
                            while (this.<$s_598>__0.MoveNext())
                            {
                                this.<flag>__1 = this.<$s_598>__0.Current;
                                this.$current = new UICamera.Mouse.Button.Pair<T>(this.<flag>__1, this.<>f__this[this.<flag>__1]);
                                this.$PC = 1;
                                flag = true;
                                return true;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.<$s_598>__0 == null)
                            {
                            }
                            this.<$s_598>__0.Dispose();
                        }
                        this.$PC = -1;
                    Label_00BE:
                        return false;
                    }

                    [DebuggerHidden]
                    public void Reset()
                    {
                        throw new NotSupportedException();
                    }

                    [DebuggerHidden]
                    IEnumerator<UICamera.Mouse.Button.Pair<T>> IEnumerable<UICamera.Mouse.Button.Pair<T>>.GetEnumerator()
                    {
                        if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                        {
                            return this;
                        }
                        return new UICamera.Mouse.Button.RefCollection<T>.<>c__Iterator4D { <>f__this = this.<>f__this, state = this.<$>state };
                    }

                    [DebuggerHidden]
                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.System.Collections.Generic.IEnumerable<UICamera.Mouse.Button.Pair<T>>.GetEnumerator();
                    }

                    UICamera.Mouse.Button.Pair<T> IEnumerator<UICamera.Mouse.Button.Pair<T>>.Current
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

                [CompilerGenerated]
                private sealed class <GetEnumerator>c__Iterator4E : IDisposable, IEnumerator, IEnumerator<UICamera.Mouse.Button.Pair<T>>
                {
                    internal UICamera.Mouse.Button.Pair<T> $current;
                    internal int $PC;
                    internal UICamera.Mouse.Button.RefCollection<T> <>f__this;

                    [DebuggerHidden]
                    public void Dispose()
                    {
                        this.$PC = -1;
                    }

                    public bool MoveNext()
                    {
                        uint num = (uint) this.$PC;
                        this.$PC = -1;
                        switch (num)
                        {
                            case 0:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Left, this.<>f__this.LeftValue);
                                this.$PC = 1;
                                goto Label_009B;

                            case 1:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Right, this.<>f__this.RightValue);
                                this.$PC = 2;
                                goto Label_009B;

                            case 2:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Middle, this.<>f__this.MiddleValue);
                                this.$PC = 3;
                                goto Label_009B;

                            case 3:
                                this.$PC = -1;
                                break;
                        }
                        return false;
                    Label_009B:
                        return true;
                    }

                    [DebuggerHidden]
                    public void Reset()
                    {
                        throw new NotSupportedException();
                    }

                    UICamera.Mouse.Button.Pair<T> IEnumerator<UICamera.Mouse.Button.Pair<T>>.Current
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

            public sealed class Sampler
            {
                public readonly UICamera.Mouse.Button.Flags Button;
                public ulong ClickCount;
                public UICamera.ClickNotification ClickNotification;
                public readonly UICamera.CursorSampler Cursor;
                public bool DidHit;
                public bool DragClick;
                public ulong DragClickNumber;
                public bool Held;
                public UIHotSpot.Hit Hit;
                public bool Once;
                public Vector2 Point;
                public GameObject Pressed;
                public bool PressedNow;
                public float PressTime;
                public bool ReleasedNow;
                public float ReleaseTime;
                public Vector2 TotalDelta;

                public Sampler(UICamera.Mouse.Button.Flags Button, UICamera.CursorSampler Cursor)
                {
                    this.Button = Button;
                    this.Cursor = Cursor;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ValCollection<T> : IEnumerable, IEnumerable<UICamera.Mouse.Button.Pair<T>>
            {
                public T LeftValue;
                public T RightValue;
                public T MiddleValue;
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public T this[UICamera.Mouse.Button.Flags button]
                {
                    get
                    {
                        switch (button)
                        {
                            case UICamera.Mouse.Button.Flags.Left:
                                return this.LeftValue;

                            case UICamera.Mouse.Button.Flags.Right:
                                return this.RightValue;

                            case UICamera.Mouse.Button.Flags.Middle:
                                return this.MiddleValue;
                        }
                        throw new ArgumentOutOfRangeException("button", "button should not be None or a Combination of multiple buttons");
                    }
                    set
                    {
                        switch (button)
                        {
                            case UICamera.Mouse.Button.Flags.Left:
                                this.LeftValue = value;
                                return;

                            case UICamera.Mouse.Button.Flags.Right:
                                this.RightValue = value;
                                return;

                            case UICamera.Mouse.Button.Flags.Middle:
                                this.MiddleValue = value;
                                return;
                        }
                        throw new ArgumentOutOfRangeException("button", "button should not be None or a Combination of multiple buttons");
                    }
                }
                public T this[int i]
                {
                    get
                    {
                        return this[UICamera.Mouse.Button.Flags.Left];
                    }
                    set
                    {
                        this[UICamera.Mouse.Button.Flags.Left] = value;
                    }
                }
                public IEnumerable<UICamera.Mouse.Button.Pair<T>> this[UICamera.Mouse.Button.PressState state]
                {
                    get
                    {
                        return new <>c__Iterator4B<T> { state = state, <$>state = state, <>f__this = *((UICamera.Mouse.Button.ValCollection<T>*) this), $PC = -2 };
                    }
                }
                [DebuggerHidden]
                public unsafe IEnumerator<UICamera.Mouse.Button.Pair<T>> GetEnumerator()
                {
                    return new <GetEnumerator>c__Iterator4C<T> { <>f__this = *((UICamera.Mouse.Button.ValCollection<T>*) this) };
                }
                [CompilerGenerated]
                private sealed class <>c__Iterator4B : IDisposable, IEnumerator, IEnumerable, IEnumerable<UICamera.Mouse.Button.Pair<T>>, IEnumerator<UICamera.Mouse.Button.Pair<T>>
                {
                    internal UICamera.Mouse.Button.Pair<T> $current;
                    internal int $PC;
                    internal UICamera.Mouse.Button.PressState <$>state;
                    internal UICamera.Mouse.Button.PressState.Enumerator <$s_597>__0;
                    internal UICamera.Mouse.Button.ValCollection<T> <>f__this;
                    internal UICamera.Mouse.Button.Flags <flag>__1;
                    internal UICamera.Mouse.Button.PressState state;

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
                                    if (this.<$s_597>__0 == null)
                                    {
                                    }
                                    this.<$s_597>__0.Dispose();
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
                                this.<$s_597>__0 = this.state.GetEnumerator();
                                num = 0xfffffffd;
                                break;

                            case 1:
                                break;

                            default:
                                goto Label_00BE;
                        }
                        try
                        {
                            while (this.<$s_597>__0.MoveNext())
                            {
                                this.<flag>__1 = this.<$s_597>__0.Current;
                                this.$current = new UICamera.Mouse.Button.Pair<T>(this.<flag>__1, this.<>f__this[this.<flag>__1]);
                                this.$PC = 1;
                                flag = true;
                                return true;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.<$s_597>__0 == null)
                            {
                            }
                            this.<$s_597>__0.Dispose();
                        }
                        this.$PC = -1;
                    Label_00BE:
                        return false;
                    }

                    [DebuggerHidden]
                    public void Reset()
                    {
                        throw new NotSupportedException();
                    }

                    [DebuggerHidden]
                    IEnumerator<UICamera.Mouse.Button.Pair<T>> IEnumerable<UICamera.Mouse.Button.Pair<T>>.GetEnumerator()
                    {
                        if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                        {
                            return this;
                        }
                        return new UICamera.Mouse.Button.ValCollection<T>.<>c__Iterator4B { <>f__this = this.<>f__this, state = this.<$>state };
                    }

                    [DebuggerHidden]
                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.System.Collections.Generic.IEnumerable<UICamera.Mouse.Button.Pair<T>>.GetEnumerator();
                    }

                    UICamera.Mouse.Button.Pair<T> IEnumerator<UICamera.Mouse.Button.Pair<T>>.Current
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

                [CompilerGenerated]
                private sealed class <GetEnumerator>c__Iterator4C : IDisposable, IEnumerator, IEnumerator<UICamera.Mouse.Button.Pair<T>>
                {
                    internal UICamera.Mouse.Button.Pair<T> $current;
                    internal int $PC;
                    internal UICamera.Mouse.Button.ValCollection<T> <>f__this;

                    [DebuggerHidden]
                    public void Dispose()
                    {
                        this.$PC = -1;
                    }

                    public bool MoveNext()
                    {
                        uint num = (uint) this.$PC;
                        this.$PC = -1;
                        switch (num)
                        {
                            case 0:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Left, this.<>f__this.LeftValue);
                                this.$PC = 1;
                                goto Label_009B;

                            case 1:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Right, this.<>f__this.RightValue);
                                this.$PC = 2;
                                goto Label_009B;

                            case 2:
                                this.$current = new UICamera.Mouse.Button.Pair<T>(UICamera.Mouse.Button.Flags.Middle, this.<>f__this.MiddleValue);
                                this.$PC = 3;
                                goto Label_009B;

                            case 3:
                                this.$PC = -1;
                                break;
                        }
                        return false;
                    Label_009B:
                        return true;
                    }

                    [DebuggerHidden]
                    public void Reset()
                    {
                        throw new NotSupportedException();
                    }

                    UICamera.Mouse.Button.Pair<T> IEnumerator<UICamera.Mouse.Button.Pair<T>>.Current
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
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct State
        {
            public Vector2 Point;
            public Vector2 Delta;
            public Vector2 Scroll;
            public UICamera.Mouse.Button.PressState Buttons;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RaycastCheckWork
    {
        public Ray ray;
        public RaycastHit hit;
        public float dist;
        public int mask;
        public bool Check()
        {
            bool flag;
            UIPanel panel = UIPanel.Find(this.hit.collider.transform, false);
            if (panel == null)
            {
                return true;
            }
            if (panel.enabled && ((panel.clipping == UIDrawCall.Clipping.None) || UICamera.CheckRayEnterClippingRect(this.ray, panel.transform, panel.clipRange)))
            {
                return true;
            }
            Collider collider = this.hit.collider;
            try
            {
                collider.enabled = false;
                if (Physics.Raycast(this.ray, out this.hit, this.dist, this.mask))
                {
                    return this.Check();
                }
                flag = false;
            }
            finally
            {
                collider.enabled = true;
            }
            return flag;
        }
    }
}

