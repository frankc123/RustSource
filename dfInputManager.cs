using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Input Manager")]
public class dfInputManager : MonoBehaviour
{
    [CompilerGenerated]
    private static Func<Component, bool> <>f__am$cacheF;
    private IInputAdapter adapter;
    [SerializeField]
    protected float axisPollingInterval = 0.15f;
    private dfControl buttonDownTarget;
    private static dfControl controlUnderMouse = null;
    [SerializeField]
    protected string horizontalAxis = "Horizontal";
    [SerializeField]
    protected KeyCode joystickClickButton = KeyCode.Joystick1Button1;
    private float lastAxisCheck;
    private MouseInputManager mouseHandler;
    [SerializeField]
    protected Camera renderCamera;
    [SerializeField]
    protected bool retainFocus;
    [SerializeField]
    protected int touchClickRadius = 20;
    [SerializeField]
    protected bool useJoystick;
    [SerializeField]
    protected bool useTouch = true;
    [SerializeField]
    protected string verticalAxis = "Vertical";
    private static KeyCode[] wasd = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow };

    public void Awake()
    {
    }

    internal dfControl clipCast(RaycastHit[] hits)
    {
        if ((hits == null) || (hits.Length == 0))
        {
            return null;
        }
        dfControl control = null;
        dfControl modalControl = dfGUIManager.GetModalControl();
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            RaycastHit hit = hits[i];
            dfControl component = hit.transform.GetComponent<dfControl>();
            if (((((component != null) && ((modalControl == null) || component.transform.IsChildOf(modalControl.transform))) && ((component.enabled && (combinedOpacity(component) > 0.01f)) && (component.IsEnabled && component.IsVisible))) && component.transform.IsChildOf(base.transform)) && (isInsideClippingRegion(hit, component) && ((control == null) || (component.RenderOrder > control.RenderOrder))))
            {
                control = component;
            }
        }
        return control;
    }

    private static float combinedOpacity(dfControl control)
    {
        float num = 1f;
        while (control != null)
        {
            num *= control.Opacity;
            control = control.Parent;
        }
        return num;
    }

    internal static bool isInsideClippingRegion(RaycastHit hit, dfControl control)
    {
        Vector3 point = hit.point;
        while (control != null)
        {
            Plane[] planeArray = !control.ClipChildren ? null : control.GetClippingPlanes();
            if ((planeArray != null) && (planeArray.Length > 0))
            {
                for (int i = 0; i < planeArray.Length; i++)
                {
                    if (!planeArray[i].GetSide(point))
                    {
                        return false;
                    }
                }
            }
            control = control.Parent;
        }
        return true;
    }

    public void OnEnable()
    {
        this.mouseHandler = new MouseInputManager();
        if (this.adapter == null)
        {
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = c => typeof(IInputAdapter).IsAssignableFrom(c.GetType());
            }
            Component component = Enumerable.Where<Component>(base.GetComponents(typeof(MonoBehaviour)), <>f__am$cacheF).FirstOrDefault<Component>();
            IInputAdapter adapter1 = (IInputAdapter) component;
            if (adapter1 == null)
            {
            }
            this.adapter = new DefaultInput();
        }
    }

    public void OnGUI()
    {
        Event current = Event.current;
        if ((current != null) && (current.isKey && (current.keyCode != KeyCode.None)))
        {
            this.processKeyEvent(current.type, current.keyCode, current.modifiers);
        }
    }

    private void processJoystick()
    {
        try
        {
            dfControl activeControl = dfGUIManager.ActiveControl;
            if ((activeControl != null) && activeControl.transform.IsChildOf(base.transform))
            {
                float axis = this.adapter.GetAxis(this.horizontalAxis);
                float f = this.adapter.GetAxis(this.verticalAxis);
                if ((Mathf.Abs(axis) < 0.5f) && (Mathf.Abs(f) <= 0.5f))
                {
                    this.lastAxisCheck = Time.deltaTime - this.axisPollingInterval;
                }
                if ((Time.realtimeSinceStartup - this.lastAxisCheck) > this.axisPollingInterval)
                {
                    if (Mathf.Abs(axis) >= 0.5f)
                    {
                        this.lastAxisCheck = Time.realtimeSinceStartup;
                        KeyCode key = (axis <= 0f) ? KeyCode.LeftArrow : KeyCode.RightArrow;
                        activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, key, false, false, false));
                    }
                    if (Mathf.Abs(f) >= 0.5f)
                    {
                        this.lastAxisCheck = Time.realtimeSinceStartup;
                        KeyCode code2 = (f <= 0f) ? KeyCode.DownArrow : KeyCode.UpArrow;
                        activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, code2, false, false, false));
                    }
                }
                if (this.joystickClickButton != KeyCode.None)
                {
                    if (this.adapter.GetKeyDown(this.joystickClickButton))
                    {
                        Vector3 center = activeControl.GetCenter();
                        Camera camera = activeControl.GetCamera();
                        Ray ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(center));
                        dfMouseEventArgs args = new dfMouseEventArgs(activeControl, dfMouseButtons.Left, 0, ray, center, 0f);
                        activeControl.OnMouseDown(args);
                        this.buttonDownTarget = activeControl;
                    }
                    if (this.adapter.GetKeyUp(this.joystickClickButton))
                    {
                        if (this.buttonDownTarget == activeControl)
                        {
                            activeControl.DoClick();
                        }
                        Vector3 position = activeControl.GetCenter();
                        Camera camera2 = activeControl.GetCamera();
                        Ray ray2 = camera2.ScreenPointToRay(camera2.WorldToScreenPoint(position));
                        dfMouseEventArgs args2 = new dfMouseEventArgs(activeControl, dfMouseButtons.Left, 0, ray2, position, 0f);
                        activeControl.OnMouseUp(args2);
                        this.buttonDownTarget = null;
                    }
                }
                for (KeyCode code3 = KeyCode.Joystick1Button0; code3 <= KeyCode.Joystick1Button19; code3 += 1)
                {
                    if (this.adapter.GetKeyDown(code3))
                    {
                        activeControl.OnKeyDown(new dfKeyEventArgs(activeControl, code3, false, false, false));
                    }
                }
            }
        }
        catch (UnityException exception)
        {
            Debug.LogError(exception.ToString(), this);
            this.useJoystick = false;
        }
    }

    private bool processKeyboard()
    {
        dfControl activeControl = dfGUIManager.ActiveControl;
        if (((activeControl == null) || string.IsNullOrEmpty(Input.inputString)) || !activeControl.transform.IsChildOf(base.transform))
        {
            return false;
        }
        foreach (char ch in Input.inputString)
        {
            switch (ch)
            {
                case '\b':
                case '\n':
                    break;

                default:
                {
                    KeyCode key = (KeyCode) ch;
                    dfKeyEventArgs args = new dfKeyEventArgs(activeControl, key, false, false, false) {
                        Character = ch
                    };
                    activeControl.OnKeyPress(args);
                    break;
                }
            }
        }
        return true;
    }

    private void processKeyEvent(EventType eventType, KeyCode keyCode, EventModifiers modifiers)
    {
        dfControl activeControl = dfGUIManager.ActiveControl;
        if (((activeControl != null) && activeControl.IsEnabled) && activeControl.transform.IsChildOf(base.transform))
        {
            bool control = (modifiers & EventModifiers.Control) == EventModifiers.Control;
            bool shift = (modifiers & EventModifiers.Shift) == EventModifiers.Shift;
            bool alt = (modifiers & EventModifiers.Alt) == EventModifiers.Alt;
            dfKeyEventArgs args = new dfKeyEventArgs(activeControl, keyCode, control, shift, alt);
            if ((keyCode >= KeyCode.Space) && (keyCode <= KeyCode.Z))
            {
                char c = (char) ((ushort) keyCode);
                args.Character = !shift ? char.ToLower(c) : char.ToUpper(c);
            }
            if (eventType == EventType.KeyDown)
            {
                activeControl.OnKeyDown(args);
            }
            else if (eventType == EventType.KeyUp)
            {
                activeControl.OnKeyUp(args);
            }
            if (args.Used || (eventType == EventType.KeyUp))
            {
            }
        }
    }

    private void processMouseInput()
    {
        Vector2 mousePosition = this.adapter.GetMousePosition();
        Ray ray = this.renderCamera.ScreenPointToRay((Vector3) mousePosition);
        float distance = this.renderCamera.farClipPlane - this.renderCamera.nearClipPlane;
        RaycastHit[] array = Physics.RaycastAll(ray, distance, this.renderCamera.cullingMask);
        Array.Sort<RaycastHit>(array, new Comparison<RaycastHit>(dfInputManager.raycastHitSorter));
        controlUnderMouse = this.clipCast(array);
        this.mouseHandler.ProcessInput(this.adapter, ray, controlUnderMouse, this.retainFocus);
    }

    internal static int raycastHitSorter(RaycastHit lhs, RaycastHit rhs)
    {
        return lhs.distance.CompareTo(rhs.distance);
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Application.isPlaying)
        {
            dfControl activeControl = dfGUIManager.ActiveControl;
            this.processMouseInput();
            if (((activeControl != null) && !this.processKeyboard()) && this.useJoystick)
            {
                for (int i = 0; i < wasd.Length; i++)
                {
                    if ((Input.GetKey(wasd[i]) || Input.GetKeyDown(wasd[i])) || Input.GetKeyUp(wasd[i]))
                    {
                        return;
                    }
                }
                this.processJoystick();
            }
        }
    }

    public IInputAdapter Adapter
    {
        get
        {
            return this.adapter;
        }
        set
        {
            if (value == null)
            {
            }
            this.adapter = new DefaultInput();
        }
    }

    public static dfControl ControlUnderMouse
    {
        get
        {
            return controlUnderMouse;
        }
    }

    public string HorizontalAxis
    {
        get
        {
            return this.horizontalAxis;
        }
        set
        {
            this.horizontalAxis = value;
        }
    }

    public KeyCode JoystickClickButton
    {
        get
        {
            return this.joystickClickButton;
        }
        set
        {
            this.joystickClickButton = value;
        }
    }

    public Camera RenderCamera
    {
        get
        {
            return this.renderCamera;
        }
        set
        {
            this.renderCamera = value;
        }
    }

    public bool RetainFocus
    {
        get
        {
            return this.retainFocus;
        }
        set
        {
            this.retainFocus = value;
        }
    }

    public int TouchClickRadius
    {
        get
        {
            return this.touchClickRadius;
        }
        set
        {
            this.touchClickRadius = Mathf.Max(0, value);
        }
    }

    public bool UseJoystick
    {
        get
        {
            return this.useJoystick;
        }
        set
        {
            this.useJoystick = value;
        }
    }

    public bool UseTouch
    {
        get
        {
            return this.useTouch;
        }
        set
        {
            this.useTouch = value;
        }
    }

    public string VerticalAxis
    {
        get
        {
            return this.verticalAxis;
        }
        set
        {
            this.verticalAxis = value;
        }
    }

    private class DefaultInput : IInputAdapter
    {
        public float GetAxis(string axisName)
        {
            return Input.GetAxis(axisName);
        }

        public bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }

        public bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button);
        }

        public bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button);
        }

        public Vector2 GetMousePosition()
        {
            return Input.mousePosition;
        }
    }

    private class MouseInputManager
    {
        private dfControl activeControl;
        private dfMouseButtons buttonsDown;
        private dfMouseButtons buttonsPressed;
        private dfMouseButtons buttonsReleased;
        private const float DOUBLECLICK_TIME = 0.25f;
        private const int DRAG_START_DELTA = 2;
        private object dragData;
        private dfDragDropState dragState;
        private const float HOVER_NOTIFICATION_BEGIN = 0.25f;
        private const float HOVER_NOTIFICATION_FREQUENCY = 0.1f;
        private float lastClickTime;
        private dfControl lastDragControl;
        private float lastHoverTime;
        private Vector2 lastPosition = ((Vector2) (Vector2.one * -2.147484E+09f));
        private Vector2 mouseMoveDelta = Vector2.zero;
        private const string scrollAxisName = "Mouse ScrollWheel";

        private static void getMouseButtonInfo(IInputAdapter adapter, ref dfMouseButtons buttonsDown, ref dfMouseButtons buttonsReleased, ref dfMouseButtons buttonsPressed)
        {
            for (int i = 0; i < 3; i++)
            {
                if (adapter.GetMouseButton(i))
                {
                    buttonsDown |= ((int) 1) << i;
                }
                if (adapter.GetMouseButtonUp(i))
                {
                    buttonsReleased |= ((int) 1) << i;
                }
                if (adapter.GetMouseButtonDown(i))
                {
                    buttonsPressed |= ((int) 1) << i;
                }
            }
        }

        public void ProcessInput(IInputAdapter adapter, Ray ray, dfControl control, bool retainFocusSetting)
        {
            Vector2 mousePosition = adapter.GetMousePosition();
            this.buttonsDown = dfMouseButtons.None;
            this.buttonsReleased = dfMouseButtons.None;
            this.buttonsPressed = dfMouseButtons.None;
            getMouseButtonInfo(adapter, ref this.buttonsDown, ref this.buttonsReleased, ref this.buttonsPressed);
            float axis = adapter.GetAxis("Mouse ScrollWheel");
            if (!Mathf.Approximately(axis, 0f))
            {
                axis = Mathf.Sign(axis) * Mathf.Max(1f, Mathf.Abs(axis));
            }
            this.mouseMoveDelta = mousePosition - this.lastPosition;
            this.lastPosition = mousePosition;
            if (this.dragState == dfDragDropState.Dragging)
            {
                if (this.buttonsReleased == dfMouseButtons.None)
                {
                    if (control != this.activeControl)
                    {
                        if (control != this.lastDragControl)
                        {
                            if (this.lastDragControl != null)
                            {
                                dfDragEventArgs args = new dfDragEventArgs(this.lastDragControl, this.dragState, this.dragData, ray, mousePosition);
                                this.lastDragControl.OnDragLeave(args);
                            }
                            if (control != null)
                            {
                                dfDragEventArgs args2 = new dfDragEventArgs(control, this.dragState, this.dragData, ray, mousePosition);
                                control.OnDragEnter(args2);
                            }
                            this.lastDragControl = control;
                        }
                        else if ((control != null) && (Vector2.Distance(mousePosition, this.lastPosition) > 1f))
                        {
                            dfDragEventArgs args3 = new dfDragEventArgs(control, this.dragState, this.dragData, ray, mousePosition);
                            control.OnDragOver(args3);
                        }
                    }
                }
                else
                {
                    if ((control != null) && (control != this.activeControl))
                    {
                        dfDragEventArgs args4 = new dfDragEventArgs(control, dfDragDropState.Dragging, this.dragData, ray, mousePosition);
                        control.OnDragDrop(args4);
                        if (!args4.Used || (args4.State == dfDragDropState.Dragging))
                        {
                            args4.State = dfDragDropState.Cancelled;
                        }
                        args4 = new dfDragEventArgs(this.activeControl, args4.State, args4.Data, ray, mousePosition) {
                            Target = control
                        };
                        this.activeControl.OnDragEnd(args4);
                    }
                    else
                    {
                        dfDragDropState state = (control != null) ? dfDragDropState.Cancelled : dfDragDropState.CancelledNoTarget;
                        dfDragEventArgs args5 = new dfDragEventArgs(this.activeControl, state, this.dragData, ray, mousePosition);
                        this.activeControl.OnDragEnd(args5);
                    }
                    this.dragState = dfDragDropState.None;
                    this.lastDragControl = null;
                    this.activeControl = null;
                    this.lastClickTime = 0f;
                    this.lastHoverTime = 0f;
                    this.lastPosition = mousePosition;
                }
            }
            else if (this.buttonsReleased != dfMouseButtons.None)
            {
                this.lastHoverTime = Time.realtimeSinceStartup + 0.25f;
                if (this.activeControl == null)
                {
                    this.setActive(control, mousePosition, ray);
                }
                else
                {
                    if ((this.activeControl == control) && (this.buttonsDown == dfMouseButtons.None))
                    {
                        if ((Time.realtimeSinceStartup - this.lastClickTime) < 0.25f)
                        {
                            this.lastClickTime = 0f;
                            this.activeControl.OnDoubleClick(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 1, ray, mousePosition, axis));
                        }
                        else
                        {
                            this.lastClickTime = Time.realtimeSinceStartup;
                            this.activeControl.OnClick(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 1, ray, mousePosition, axis));
                        }
                    }
                    this.activeControl.OnMouseUp(new dfMouseEventArgs(this.activeControl, this.buttonsReleased, 0, ray, mousePosition, axis));
                    if ((this.buttonsDown == dfMouseButtons.None) && (this.activeControl != control))
                    {
                        this.setActive(null, mousePosition, ray);
                    }
                }
            }
            else if (this.buttonsPressed != dfMouseButtons.None)
            {
                this.lastHoverTime = Time.realtimeSinceStartup + 0.25f;
                if (this.activeControl != null)
                {
                    this.activeControl.OnMouseDown(new dfMouseEventArgs(this.activeControl, this.buttonsPressed, 0, ray, mousePosition, axis));
                }
                else
                {
                    this.setActive(control, mousePosition, ray);
                    if (control != null)
                    {
                        control.OnMouseDown(new dfMouseEventArgs(control, this.buttonsPressed, 0, ray, mousePosition, axis));
                    }
                    else if (!retainFocusSetting)
                    {
                        dfControl activeControl = dfGUIManager.ActiveControl;
                        if (activeControl != null)
                        {
                            activeControl.Unfocus();
                        }
                    }
                }
            }
            else
            {
                if (((this.activeControl != null) && (this.activeControl == control)) && ((this.mouseMoveDelta.magnitude == 0f) && ((Time.realtimeSinceStartup - this.lastHoverTime) > 0.1f)))
                {
                    this.activeControl.OnMouseHover(new dfMouseEventArgs(this.activeControl, this.buttonsDown, 0, ray, mousePosition, axis));
                    this.lastHoverTime = Time.realtimeSinceStartup;
                }
                if (this.buttonsDown == dfMouseButtons.None)
                {
                    if ((axis != 0f) && (control != null))
                    {
                        this.setActive(control, mousePosition, ray);
                        control.OnMouseWheel(new dfMouseEventArgs(control, this.buttonsDown, 0, ray, mousePosition, axis));
                        return;
                    }
                    this.setActive(control, mousePosition, ray);
                }
                else if (this.activeControl != null)
                {
                    if ((control != null) && (control.RenderOrder > this.activeControl.RenderOrder))
                    {
                    }
                    if (((this.mouseMoveDelta.magnitude >= 2f) && ((this.buttonsDown & (dfMouseButtons.Right | dfMouseButtons.Left)) != dfMouseButtons.None)) && (this.dragState != dfDragDropState.Denied))
                    {
                        dfDragEventArgs args6 = new dfDragEventArgs(this.activeControl) {
                            Position = mousePosition
                        };
                        this.activeControl.OnDragStart(args6);
                        if (args6.State == dfDragDropState.Dragging)
                        {
                            this.dragState = dfDragDropState.Dragging;
                            this.dragData = args6.Data;
                            return;
                        }
                        this.dragState = dfDragDropState.Denied;
                    }
                }
                if ((this.activeControl != null) && (this.mouseMoveDelta.magnitude >= 1f))
                {
                    dfMouseEventArgs args7 = new dfMouseEventArgs(this.activeControl, this.buttonsDown, 0, ray, mousePosition, axis) {
                        MoveDelta = this.mouseMoveDelta
                    };
                    this.activeControl.OnMouseMove(args7);
                }
            }
        }

        private void setActive(dfControl control, Vector2 position, Ray ray)
        {
            dfMouseEventArgs args;
            if ((this.activeControl != null) && (this.activeControl != control))
            {
                args = new dfMouseEventArgs(this.activeControl) {
                    Position = position,
                    Ray = ray
                };
                this.activeControl.OnMouseLeave(args);
            }
            if ((control != null) && (control != this.activeControl))
            {
                this.lastClickTime = 0f;
                this.lastHoverTime = Time.realtimeSinceStartup + 0.25f;
                args = new dfMouseEventArgs(control) {
                    Position = position,
                    Ray = ray
                };
                control.OnMouseEnter(args);
            }
            this.activeControl = control;
            this.lastPosition = position;
            this.dragState = dfDragDropState.None;
        }
    }

    private class TouchInputManager
    {
        [CompilerGenerated]
        private static Func<ControlTouchTracker, bool> <>f__am$cache3;
        private dfInputManager manager;
        private List<ControlTouchTracker> tracked;
        private List<int> untracked;

        private TouchInputManager()
        {
            this.tracked = new List<ControlTouchTracker>();
            this.untracked = new List<int>();
        }

        public TouchInputManager(dfInputManager manager)
        {
            this.tracked = new List<ControlTouchTracker>();
            this.untracked = new List<int>();
            this.manager = manager;
        }

        private dfControl clipCast(Transform transform, RaycastHit[] hits)
        {
            if ((hits == null) || (hits.Length == 0))
            {
                return null;
            }
            dfControl control = null;
            dfControl modalControl = dfGUIManager.GetModalControl();
            for (int i = hits.Length - 1; i >= 0; i--)
            {
                RaycastHit hit = hits[i];
                dfControl component = hit.transform.GetComponent<dfControl>();
                if (((((component != null) && ((modalControl == null) || component.transform.IsChildOf(modalControl.transform))) && ((component.enabled && (component.Opacity >= 0.01f)) && (component.IsEnabled && component.IsVisible))) && component.transform.IsChildOf(transform)) && (this.isInsideClippingRegion(hit, component) && ((control == null) || (component.RenderOrder > control.RenderOrder))))
                {
                    control = component;
                }
            }
            return control;
        }

        private bool isInsideClippingRegion(RaycastHit hit, dfControl control)
        {
            Vector3 point = hit.point;
            while (control != null)
            {
                Plane[] planeArray = !control.ClipChildren ? null : control.GetClippingPlanes();
                if ((planeArray != null) && (planeArray.Length > 0))
                {
                    for (int i = 0; i < planeArray.Length; i++)
                    {
                        if (!planeArray[i].GetSide(point))
                        {
                            return false;
                        }
                    }
                }
                control = control.Parent;
            }
            return true;
        }

        internal void Process(Transform transform, Camera renderCamera, dfList<Touch> touches, bool retainFocus)
        {
            for (int i = 0; i < touches.Count; i++)
            {
                <Process>c__AnonStorey57 storey = new <Process>c__AnonStorey57();
                Touch touch = touches[i];
                Ray ray = renderCamera.ScreenPointToRay((Vector3) touch.position);
                float distance = renderCamera.farClipPlane - renderCamera.nearClipPlane;
                RaycastHit[] hits = Physics.RaycastAll(ray, distance, renderCamera.cullingMask);
                dfInputManager.controlUnderMouse = this.clipCast(transform, hits);
                if ((dfInputManager.controlUnderMouse == null) && (touch.phase == TouchPhase.Began))
                {
                    this.untracked.Add(touch.fingerId);
                    continue;
                }
                if (this.untracked.Contains(touch.fingerId))
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        this.untracked.Remove(touch.fingerId);
                    }
                    continue;
                }
                storey.info = new TouchRaycast(dfInputManager.controlUnderMouse, touch, ray);
                ControlTouchTracker tracker = Enumerable.FirstOrDefault<ControlTouchTracker>(this.tracked, new Func<ControlTouchTracker, bool>(storey.<>m__26));
                if (tracker != null)
                {
                    tracker.Process(storey.info);
                    continue;
                }
                bool flag = false;
                for (int j = 0; j < this.tracked.Count; j++)
                {
                    if (this.tracked[j].Process(storey.info))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag && (dfInputManager.controlUnderMouse != null))
                {
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = x => x.control == dfInputManager.controlUnderMouse;
                    }
                    if (!Enumerable.Any<ControlTouchTracker>(this.tracked, <>f__am$cache3))
                    {
                        if (dfInputManager.controlUnderMouse == null)
                        {
                            Debug.Log("Tracking touch with no control: " + touch.fingerId);
                        }
                        ControlTouchTracker item = new ControlTouchTracker(this.manager, dfInputManager.controlUnderMouse);
                        this.tracked.Add(item);
                        item.Process(storey.info);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Process>c__AnonStorey57
        {
            internal dfInputManager.TouchInputManager.TouchRaycast info;

            internal bool <>m__26(dfInputManager.TouchInputManager.ControlTouchTracker x)
            {
                return x.IsTrackingFinger(this.info.FingerID);
            }
        }

        private class ControlTouchTracker
        {
            [CompilerGenerated]
            private static Func<KeyValuePair<int, dfInputManager.TouchInputManager.TouchRaycast>, Touch> <>f__am$cache6;
            public List<int> capture = new List<int>();
            public dfControl control;
            private object dragData;
            private dfDragDropState dragState;
            private dfInputManager manager;
            public Dictionary<int, dfInputManager.TouchInputManager.TouchRaycast> touches = new Dictionary<int, dfInputManager.TouchInputManager.TouchRaycast>();

            public ControlTouchTracker(dfInputManager manager, dfControl control)
            {
                this.manager = manager;
                this.control = control;
            }

            private bool canFireClickEvent(dfInputManager.TouchInputManager.TouchRaycast info, dfInputManager.TouchInputManager.TouchRaycast touch)
            {
                return ((this.manager.TouchClickRadius <= 0) || (Vector2.Distance(info.position, touch.position) < this.manager.TouchClickRadius));
            }

            private List<Touch> getActiveTouches()
            {
                <getActiveTouches>c__AnonStorey58 storey = new <getActiveTouches>c__AnonStorey58();
                Touch[] touches = Input.touches;
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = x => x.Value.touch;
                }
                storey.result = Enumerable.Select<KeyValuePair<int, dfInputManager.TouchInputManager.TouchRaycast>, Touch>(this.touches, <>f__am$cache6).ToList<Touch>();
                <getActiveTouches>c__AnonStorey59 storey2 = new <getActiveTouches>c__AnonStorey59 {
                    <>f__ref$88 = storey,
                    i = 0
                };
                while (storey2.i < storey.result.Count)
                {
                    storey.result[storey2.i] = Enumerable.First<Touch>(touches, new Func<Touch, bool>(storey2.<>m__29));
                    storey2.i++;
                }
                return storey.result;
            }

            public bool IsTrackingFinger(int fingerID)
            {
                return this.touches.ContainsKey(fingerID);
            }

            public bool Process(dfInputManager.TouchInputManager.TouchRaycast info)
            {
                if (this.IsDragging)
                {
                    if (!this.capture.Contains(info.FingerID))
                    {
                        return false;
                    }
                    if (info.Phase != TouchPhase.Stationary)
                    {
                        if (info.Phase == TouchPhase.Canceled)
                        {
                            this.control.OnDragEnd(new dfDragEventArgs(this.control, dfDragDropState.Cancelled, this.dragData, info.ray, info.position));
                            this.dragState = dfDragDropState.None;
                            this.touches.Clear();
                            this.capture.Clear();
                            return true;
                        }
                        if (info.Phase != TouchPhase.Ended)
                        {
                            return true;
                        }
                        if ((info.control == null) || (info.control == this.control))
                        {
                            this.control.OnDragEnd(new dfDragEventArgs(this.control, dfDragDropState.CancelledNoTarget, this.dragData, info.ray, info.position));
                            this.dragState = dfDragDropState.None;
                            this.touches.Clear();
                            this.capture.Clear();
                            return true;
                        }
                        dfDragEventArgs args = new dfDragEventArgs(info.control, dfDragDropState.Dragging, this.dragData, info.ray, info.position);
                        info.control.OnDragDrop(args);
                        if (!args.Used || (args.State != dfDragDropState.Dropped))
                        {
                            args.State = dfDragDropState.Cancelled;
                        }
                        dfDragEventArgs args2 = new dfDragEventArgs(this.control, args.State, this.dragData, info.ray, info.position) {
                            Target = info.control
                        };
                        this.control.OnDragEnd(args2);
                        this.dragState = dfDragDropState.None;
                        this.touches.Clear();
                        this.capture.Clear();
                    }
                    return true;
                }
                if (!this.touches.ContainsKey(info.FingerID))
                {
                    if (info.control != this.control)
                    {
                        return false;
                    }
                    this.touches[info.FingerID] = info;
                    if (this.touches.Count == 1)
                    {
                        this.control.OnMouseEnter((dfMouseEventArgs) info);
                        if (info.Phase == TouchPhase.Began)
                        {
                            this.capture.Add(info.FingerID);
                            this.control.OnMouseDown((dfMouseEventArgs) info);
                        }
                        return true;
                    }
                    if (info.Phase == TouchPhase.Began)
                    {
                        this.control.OnMouseUp((dfMouseEventArgs) info);
                        this.control.OnMouseLeave((dfMouseEventArgs) info);
                        List<Touch> touches = this.getActiveTouches();
                        dfTouchEventArgs args3 = new dfTouchEventArgs(this.control, touches, info.ray);
                        this.control.OnMultiTouch(args3);
                    }
                    return true;
                }
                if ((info.Phase == TouchPhase.Canceled) || (info.Phase == TouchPhase.Ended))
                {
                    dfInputManager.TouchInputManager.TouchRaycast touch = this.touches[info.FingerID];
                    this.touches.Remove(info.FingerID);
                    if (this.touches.Count == 0)
                    {
                        if (this.capture.Contains(info.FingerID))
                        {
                            if (this.canFireClickEvent(info, touch) && (info.control == this.control))
                            {
                                if (info.touch.tapCount > 1)
                                {
                                    this.control.OnDoubleClick((dfMouseEventArgs) info);
                                }
                                else
                                {
                                    this.control.OnClick((dfMouseEventArgs) info);
                                }
                            }
                            this.control.OnMouseUp((dfMouseEventArgs) info);
                        }
                        this.control.OnMouseLeave((dfMouseEventArgs) info);
                        this.capture.Remove(info.FingerID);
                        return true;
                    }
                    this.capture.Remove(info.FingerID);
                    if (this.touches.Count == 1)
                    {
                        dfTouchEventArgs args4 = this.touches.Values.First<dfInputManager.TouchInputManager.TouchRaycast>();
                        this.control.OnMouseEnter(args4);
                        this.control.OnMouseDown(args4);
                        return true;
                    }
                }
                if (this.touches.Count > 1)
                {
                    List<Touch> list2 = this.getActiveTouches();
                    dfTouchEventArgs args5 = new dfTouchEventArgs(this.control, list2, info.ray);
                    this.control.OnMultiTouch(args5);
                    return true;
                }
                if (!this.IsDragging && (info.Phase == TouchPhase.Stationary))
                {
                    if (info.control == this.control)
                    {
                        this.control.OnMouseHover((dfMouseEventArgs) info);
                        return true;
                    }
                    return false;
                }
                if ((this.capture.Contains(info.FingerID) && (this.dragState == dfDragDropState.None)) && (info.Phase == TouchPhase.Moved))
                {
                    dfDragEventArgs args6 = (dfDragEventArgs) info;
                    this.control.OnDragStart(args6);
                    if ((args6.State == dfDragDropState.Dragging) && args6.Used)
                    {
                        this.dragState = dfDragDropState.Dragging;
                        this.dragData = args6.Data;
                        return true;
                    }
                    this.dragState = dfDragDropState.Denied;
                }
                if ((info.control != this.control) && !this.capture.Contains(info.FingerID))
                {
                    this.control.OnMouseLeave((dfMouseEventArgs) info);
                    this.touches.Remove(info.FingerID);
                    return true;
                }
                this.control.OnMouseMove((dfMouseEventArgs) info);
                return true;
            }

            public bool IsDragging
            {
                get
                {
                    return (this.dragState == dfDragDropState.Dragging);
                }
            }

            public int TouchCount
            {
                get
                {
                    return this.touches.Count;
                }
            }

            [CompilerGenerated]
            private sealed class <getActiveTouches>c__AnonStorey58
            {
                internal List<Touch> result;
            }

            [CompilerGenerated]
            private sealed class <getActiveTouches>c__AnonStorey59
            {
                internal dfInputManager.TouchInputManager.ControlTouchTracker.<getActiveTouches>c__AnonStorey58 <>f__ref$88;
                internal int i;

                internal bool <>m__29(Touch x)
                {
                    Touch touch = this.<>f__ref$88.result[this.i];
                    return (x.fingerId == touch.fingerId);
                }
            }
        }

        private class TouchRaycast
        {
            public dfControl control;
            public Vector2 position;
            public Ray ray;
            public Touch touch;

            public TouchRaycast(dfControl control, Touch touch, Ray ray)
            {
                this.control = control;
                this.touch = touch;
                this.ray = ray;
                this.position = touch.position;
            }

            public static implicit operator dfDragEventArgs(dfInputManager.TouchInputManager.TouchRaycast touch)
            {
                return new dfDragEventArgs(touch.control, dfDragDropState.None, null, touch.ray, touch.position);
            }

            public static implicit operator dfTouchEventArgs(dfInputManager.TouchInputManager.TouchRaycast touch)
            {
                return new dfTouchEventArgs(touch.control, touch.touch, touch.ray);
            }

            public int FingerID
            {
                get
                {
                    return this.touch.fingerId;
                }
            }

            public TouchPhase Phase
            {
                get
                {
                    return this.touch.phase;
                }
            }
        }
    }
}

