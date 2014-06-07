using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(dfInputManager)), RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(MeshFilter)), AddComponentMenu("Daikon Forge/User Interface/GUI Manager"), ExecuteInEditMode]
public class dfGUIManager : MonoBehaviour
{
    [CompilerGenerated]
    private static Predicate<dfRenderData> <>f__am$cache26;
    [CompilerGenerated]
    private static Func<dfRenderData, bool> <>f__am$cache27;
    [CompilerGenerated]
    private static Action<dfControl> <>f__am$cache28;
    private static dfControl activeControl = null;
    private int activeRenderMesh;
    private bool? applyHalfPixelOffset;
    [SerializeField]
    protected dfAtlas atlas;
    private int cachedChildCount;
    private Vector2 cachedScreenSize;
    private Stack<ClipRegion> clipStack = new Stack<ClipRegion>();
    [SerializeField]
    protected bool consumeMouseEvents = true;
    private Vector3[] corners = new Vector3[4];
    [SerializeField]
    protected dfFont defaultFont;
    private dfList<dfRenderData> drawCallBuffers = new dfList<dfRenderData>();
    private int drawCallCount;
    [SerializeField]
    protected int fixedHeight = 600;
    [SerializeField]
    protected int fixedWidth = -1;
    [SerializeField]
    protected bool generateNormals;
    private dfGUICamera guiCamera;
    [SerializeField]
    protected dfInputManager inputManager;
    private bool isDirty;
    private static dfRenderData masterBuffer = new dfRenderData(0x1000);
    [SerializeField]
    protected bool mergeMaterials;
    private MeshRenderer meshRenderer;
    private static Stack<ModalControlReference> modalControlStack = new Stack<ModalControlReference>();
    private dfList<Rect> occluders = new dfList<Rect>(0x100);
    [SerializeField]
    protected bool overrideCamera;
    [SerializeField]
    protected bool pixelPerfectMode = true;
    [SerializeField]
    protected Camera renderCamera;
    private MeshFilter renderFilter;
    private Mesh[] renderMesh;
    [SerializeField]
    protected int renderQueueBase = 0xbb8;
    private List<int> submeshes = new List<int>();
    private Vector2 uiOffset = Vector2.zero;
    [SerializeField]
    protected float uiScale = 1f;

    public static  event RenderCallback AfterRender;

    public static  event RenderCallback BeforeRender;

    public T AddControl<T>() where T: dfControl
    {
        return (T) this.AddControl(typeof(T));
    }

    public dfControl AddControl(Type type)
    {
        if (!typeof(dfControl).IsAssignableFrom(type))
        {
            throw new InvalidCastException();
        }
        Type[] components = new Type[] { type };
        GameObject obj2 = new GameObject(type.Name, components) {
            transform = { parent = base.transform },
            layer = base.gameObject.layer
        };
        dfControl component = obj2.GetComponent(type) as dfControl;
        component.ZOrder = this.getMaxZOrder() + 1;
        return component;
    }

    public virtual void Awake()
    {
        dfRenderData.FlushObjectPool();
    }

    public void BringToFront(dfControl control)
    {
        if (control.Parent != null)
        {
            control = control.GetRootContainer();
        }
        using (dfList<dfControl> list = this.getTopLevelControls())
        {
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                dfControl control2 = list[i];
                if (control2 != control)
                {
                    control2.ZOrder = num++;
                }
            }
            control.ZOrder = num;
            this.Invalidate();
        }
    }

    private dfRenderData compileMasterBuffer()
    {
        this.submeshes.Clear();
        masterBuffer.Clear();
        for (int i = 0; i < this.drawCallCount; i++)
        {
            this.submeshes.Add(masterBuffer.Triangles.Count);
            dfRenderData buffer = this.drawCallBuffers[i];
            if (this.generateNormals && (buffer.Normals.Count == 0))
            {
                this.generateNormalsAndTangents(buffer);
            }
            masterBuffer.Merge(buffer, false);
        }
        masterBuffer.ApplyTransform(base.transform.worldToLocalMatrix);
        return masterBuffer;
    }

    public static bool ContainsFocus(dfControl control)
    {
        return ((activeControl == control) || (((activeControl != null) && (control != null)) && activeControl.transform.IsChildOf(control.transform)));
    }

    private dfGUICamera findCameraComponent()
    {
        if (this.guiCamera == null)
        {
            if (this.renderCamera == null)
            {
                return null;
            }
            this.guiCamera = this.renderCamera.GetComponent<dfGUICamera>();
            if (this.guiCamera == null)
            {
                this.guiCamera = this.renderCamera.gameObject.AddComponent<dfGUICamera>();
                this.guiCamera.transform.position = base.transform.position;
            }
        }
        return this.guiCamera;
    }

    private dfRenderData findDrawCallBufferByMaterial(Material material)
    {
        for (int i = 0; i < this.drawCallCount; i++)
        {
            if (this.drawCallBuffers[i].Material == material)
            {
                return this.drawCallBuffers[i];
            }
        }
        return null;
    }

    private Material[] gatherMaterials()
    {
        int renderQueueBase = this.renderQueueBase;
        MaterialCache.Reset();
        if (<>f__am$cache27 == null)
        {
            <>f__am$cache27 = buff => (buff != null) && (buff.Material != null);
        }
        int num2 = this.drawCallBuffers.Matching(<>f__am$cache27);
        int num3 = 0;
        Material[] materialArray = new Material[num2];
        for (int i = 0; i < this.drawCallBuffers.Count; i++)
        {
            if (this.drawCallBuffers[i].Material != null)
            {
                Material material = MaterialCache.Lookup(this.drawCallBuffers[i].Material);
                material.renderQueue = renderQueueBase++;
                materialArray[num3++] = material;
            }
        }
        return materialArray;
    }

    private void generateNormalsAndTangents(dfRenderData buffer)
    {
        Vector3 normalized = buffer.Transform.MultiplyVector(Vector3.back).normalized;
        Vector4 item = buffer.Transform.MultiplyVector(Vector3.right).normalized;
        item.w = -1f;
        for (int i = 0; i < buffer.Vertices.Count; i++)
        {
            buffer.Normals.Add(normalized);
            buffer.Tangents.Add(item);
        }
    }

    public virtual Plane[] GetClippingPlanes()
    {
        Vector3[] corners = this.GetCorners();
        Vector3 inNormal = base.transform.TransformDirection(Vector3.right);
        Vector3 vector2 = base.transform.TransformDirection(Vector3.left);
        Vector3 vector3 = base.transform.TransformDirection(Vector3.up);
        Vector3 vector4 = base.transform.TransformDirection(Vector3.down);
        return new Plane[] { new Plane(inNormal, corners[0]), new Plane(vector2, corners[1]), new Plane(vector3, corners[2]), new Plane(vector4, corners[0]) };
    }

    private Rect getControlOccluder(dfControl control)
    {
        Rect screenRect = control.GetScreenRect();
        Vector2 vector = new Vector2(screenRect.width * control.HotZoneScale.x, screenRect.height * control.HotZoneScale.y);
        Vector2 vector2 = (Vector2) (new Vector2(vector.x - screenRect.width, vector.y - screenRect.height) * 0.5f);
        return new Rect(screenRect.x - vector2.x, screenRect.y - vector2.y, vector.x, vector.y);
    }

    public Vector3[] GetCorners()
    {
        float num = this.PixelsToUnits();
        Vector2 vector = (Vector2) (this.GetScreenSize() * num);
        float x = vector.x;
        float y = vector.y;
        Vector3 v = new Vector3(-x * 0.5f, y * 0.5f);
        Vector3 vector3 = v + new Vector3(x, 0f);
        Vector3 vector4 = v + new Vector3(0f, -y);
        Vector3 vector5 = vector3 + new Vector3(0f, -y);
        Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
        this.corners[0] = localToWorldMatrix.MultiplyPoint(v);
        this.corners[1] = localToWorldMatrix.MultiplyPoint(vector3);
        this.corners[2] = localToWorldMatrix.MultiplyPoint(vector5);
        this.corners[3] = localToWorldMatrix.MultiplyPoint(vector4);
        return this.corners;
    }

    private dfRenderData getDrawCallBuffer(Material material)
    {
        dfRenderData item = null;
        if (this.MergeMaterials && (material != null))
        {
            item = this.findDrawCallBufferByMaterial(material);
            if (item != null)
            {
                return item;
            }
        }
        item = dfRenderData.Obtain();
        item.Material = material;
        this.drawCallBuffers.Add(item);
        this.drawCallCount++;
        return item;
    }

    public dfRenderData GetDrawCallBuffer(int drawCallNumber)
    {
        return this.drawCallBuffers[drawCallNumber];
    }

    private int getMaxZOrder()
    {
        int a = -1;
        using (dfList<dfControl> list = this.getTopLevelControls())
        {
            for (int i = 0; i < list.Count; i++)
            {
                a = Mathf.Max(a, list[i].ZOrder);
            }
        }
        return a;
    }

    public static dfControl GetModalControl()
    {
        return ((modalControlStack.Count <= 0) ? null : modalControlStack.Peek().control);
    }

    private Mesh getRenderMesh()
    {
        this.activeRenderMesh = (this.activeRenderMesh != 1) ? 1 : 0;
        return this.renderMesh[this.activeRenderMesh];
    }

    public Vector2 GetScreenSize()
    {
        Camera renderCamera = this.RenderCamera;
        if (Application.isPlaying && (renderCamera != null))
        {
            float num = !this.PixelPerfectMode ? ((renderCamera.pixelHeight / ((float) this.fixedHeight)) * this.uiScale) : 1f;
            return ((Vector2) (new Vector2(renderCamera.pixelWidth, renderCamera.pixelHeight) / num)).CeilToInt();
        }
        return new Vector2((float) this.FixedWidth, (float) this.FixedHeight);
    }

    private dfList<dfControl> getTopLevelControls()
    {
        int childCount = base.transform.childCount;
        dfList<dfControl> list = dfList<dfControl>.Obtain(childCount);
        for (int i = 0; i < childCount; i++)
        {
            dfControl component = base.transform.GetChild(i).GetComponent<dfControl>();
            if (component != null)
            {
                list.Add(component);
            }
        }
        list.Sort();
        return list;
    }

    public static bool HasFocus(dfControl control)
    {
        if (control == null)
        {
            return false;
        }
        return (activeControl == control);
    }

    public dfControl HitTest(Vector2 screenPosition)
    {
        Ray ray = this.renderCamera.ScreenPointToRay((Vector3) screenPosition);
        float distance = this.renderCamera.farClipPlane - this.renderCamera.nearClipPlane;
        RaycastHit[] array = Physics.RaycastAll(ray, distance, this.renderCamera.cullingMask);
        Array.Sort<RaycastHit>(array, new Comparison<RaycastHit>(dfInputManager.raycastHitSorter));
        return this.inputManager.clipCast(array);
    }

    private void initialize()
    {
        if (this.renderCamera == null)
        {
            Debug.LogError("No camera is assigned to the GUIManager");
        }
        else
        {
            this.meshRenderer = base.GetComponent<MeshRenderer>();
            this.meshRenderer.hideFlags = HideFlags.HideInInspector;
            this.renderFilter = base.GetComponent<MeshFilter>();
            this.renderFilter.hideFlags = HideFlags.HideInInspector;
            Mesh[] meshArray1 = new Mesh[2];
            Mesh mesh = new Mesh {
                hideFlags = HideFlags.DontSave
            };
            meshArray1[0] = mesh;
            mesh = new Mesh {
                hideFlags = HideFlags.DontSave
            };
            meshArray1[1] = mesh;
            this.renderMesh = meshArray1;
            this.renderMesh[0].MarkDynamic();
            this.renderMesh[1].MarkDynamic();
            if (this.fixedWidth < 0)
            {
                this.fixedWidth = Mathf.RoundToInt(this.fixedHeight * 1.33333f);
                if (<>f__am$cache28 == null)
                {
                    <>f__am$cache28 = x => x.ResetLayout(false, false);
                }
                base.GetComponentsInChildren<dfControl>().ToList<dfControl>().ForEach(<>f__am$cache28);
            }
        }
    }

    public void Invalidate()
    {
        if (!this.isDirty)
        {
            this.isDirty = true;
            this.updateRenderSettings();
        }
    }

    private void invalidateAllControls()
    {
        dfControl[] componentsInChildren = base.GetComponentsInChildren<dfControl>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].Invalidate();
        }
        this.updateRenderOrder(null);
    }

    public virtual void LateUpdate()
    {
        if ((this.renderMesh == null) || (this.renderMesh.Length == 0))
        {
            this.initialize();
        }
        if (!Application.isPlaying)
        {
            BoxCollider collider = base.collider as BoxCollider;
            if (collider != null)
            {
                Vector2 vector = (Vector2) (this.GetScreenSize() * this.PixelsToUnits());
                collider.center = Vector3.zero;
                collider.size = (Vector3) vector;
            }
        }
        if (this.isDirty)
        {
            this.isDirty = false;
            this.Render();
        }
    }

    private bool needHalfPixelOffset()
    {
        if (this.applyHalfPixelOffset.HasValue)
        {
            return this.applyHalfPixelOffset.Value;
        }
        RuntimePlatform platform = Application.platform;
        bool flag = (this.pixelPerfectMode && (((platform == RuntimePlatform.WindowsPlayer) || (platform == RuntimePlatform.WindowsWebPlayer)) || (platform == RuntimePlatform.WindowsEditor))) && (SystemInfo.graphicsShaderLevel < 40);
        this.applyHalfPixelOffset = new bool?(Application.isEditor || flag);
        return flag;
    }

    public virtual void OnDestroy()
    {
        if (this.meshRenderer != null)
        {
            this.renderFilter.sharedMesh = null;
            Object.DestroyImmediate(this.renderMesh[0]);
            Object.DestroyImmediate(this.renderMesh[1]);
            this.renderMesh = null;
        }
    }

    public virtual void OnDisable()
    {
        if (this.meshRenderer != null)
        {
            this.meshRenderer.enabled = false;
        }
    }

    public virtual void OnEnable()
    {
        this.FramesRendered = 0;
        this.TotalDrawCalls = 0;
        this.TotalTriangles = 0;
        if (this.meshRenderer != null)
        {
            this.meshRenderer.enabled = true;
        }
        if (Application.isPlaying)
        {
            this.onResolutionChanged();
        }
    }

    public void OnGUI()
    {
        if ((!this.overrideCamera && this.consumeMouseEvents) && (Application.isPlaying && (this.occluders != null)))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y;
            if (modalControlStack.Count > 0)
            {
                GUI.Box(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), GUIContent.none, GUIStyle.none);
            }
            for (int i = 0; i < this.occluders.Count; i++)
            {
                Rect rect = this.occluders[i];
                if (rect.Contains(mousePosition))
                {
                    GUI.Box(this.occluders[i], GUIContent.none, GUIStyle.none);
                    break;
                }
            }
            if (Event.current.isMouse && (Input.touchCount > 0))
            {
                int touchCount = Input.touchCount;
                for (int j = 0; j < touchCount; j++)
                {
                    Touch touch = Input.GetTouch(j);
                    if (touch.phase == TouchPhase.Began)
                    {
                        <OnGUI>c__AnonStorey55 storey = new <OnGUI>c__AnonStorey55 {
                            touchPosition = touch.position
                        };
                        storey.touchPosition.y = Screen.height - storey.touchPosition.y;
                        if (this.occluders.Any(new Func<Rect, bool>(storey.<>m__1F)))
                        {
                            Event.current.Use();
                            break;
                        }
                    }
                }
            }
        }
    }

    private void onResolutionChanged()
    {
        int currentSize = !Application.isPlaying ? this.FixedHeight : ((int) this.renderCamera.pixelHeight);
        this.onResolutionChanged(this.FixedHeight, currentSize);
    }

    private void onResolutionChanged(int oldSize, int currentSize)
    {
        float aspect = this.RenderCamera.aspect;
        float x = oldSize * aspect;
        float num3 = currentSize * aspect;
        Vector2 vector = new Vector2(x, (float) oldSize);
        Vector2 vector2 = new Vector2(num3, (float) currentSize);
        this.onResolutionChanged(vector, vector2);
    }

    private void onResolutionChanged(Vector2 oldSize, Vector2 currentSize)
    {
        this.cachedScreenSize = currentSize;
        this.applyHalfPixelOffset = null;
        float aspect = this.RenderCamera.aspect;
        float x = oldSize.y * aspect;
        float num3 = currentSize.y * aspect;
        Vector2 previousResolution = new Vector2(x, oldSize.y);
        Vector2 currentResolution = new Vector2(num3, currentSize.y);
        dfControl[] componentsInChildren = base.GetComponentsInChildren<dfControl>();
        Array.Sort<dfControl>(componentsInChildren, new Comparison<dfControl>(this.renderSortFunc));
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (this.pixelPerfectMode && (componentsInChildren[i].Parent == null))
            {
                componentsInChildren[i].MakePixelPerfect(true);
            }
            componentsInChildren[i].OnResolutionChanged(previousResolution, currentResolution);
        }
        for (int j = 0; j < componentsInChildren.Length; j++)
        {
            componentsInChildren[j].PerformLayout();
        }
        for (int k = 0; (k < componentsInChildren.Length) && this.pixelPerfectMode; k++)
        {
            if (componentsInChildren[k].Parent == null)
            {
                componentsInChildren[k].MakePixelPerfect(true);
            }
        }
    }

    public float PixelsToUnits()
    {
        float num = 2f / ((float) this.FixedHeight);
        return (num * this.UIScale);
    }

    public static void PopModal()
    {
        if (modalControlStack.Count == 0)
        {
            throw new InvalidOperationException("Modal stack is empty");
        }
        ModalControlReference reference = modalControlStack.Pop();
        if (reference.callback != null)
        {
            reference.callback(reference.control);
        }
    }

    private bool processRenderData(ref dfRenderData buffer, dfRenderData controlData, Bounds bounds, uint checksum, ClipRegion clipInfo)
    {
        if (((buffer == null) || (controlData.IsValid() && (!object.Equals(buffer.Shader, controlData.Shader) || ((controlData.Material != null) && !controlData.Material.Equals(buffer.Material))))) && controlData.IsValid())
        {
            buffer = this.getDrawCallBuffer(controlData.Material);
        }
        return (((controlData != null) && controlData.IsValid()) && clipInfo.PerformClipping(buffer, bounds, checksum, controlData));
    }

    public static void PushModal(dfControl control, ModalPoppedCallback callback = null)
    {
        if (control == null)
        {
            throw new NullReferenceException("Cannot call PushModal() with a null reference");
        }
        ModalControlReference item = new ModalControlReference {
            control = control,
            callback = callback
        };
        modalControlStack.Push(item);
    }

    public static void RefreshAll(bool force = false)
    {
        dfGUIManager[] managerArray = Object.FindObjectsOfType(typeof(dfGUIManager)) as dfGUIManager[];
        for (int i = 0; i < managerArray.Length; i++)
        {
            managerArray[i].invalidateAllControls();
            if (force || !Application.isPlaying)
            {
                managerArray[i].Render();
            }
        }
    }

    public void Render()
    {
        this.FramesRendered++;
        if (BeforeRender != null)
        {
            BeforeRender(this);
        }
        try
        {
            this.updateRenderSettings();
            this.ControlsRendered = 0;
            this.occluders.Clear();
            this.TotalDrawCalls = 0;
            this.TotalTriangles = 0;
            if ((this.RenderCamera == null) || !base.enabled)
            {
                if (this.meshRenderer != null)
                {
                    this.meshRenderer.enabled = false;
                }
            }
            else
            {
                if ((this.meshRenderer != null) && !this.meshRenderer.enabled)
                {
                    this.meshRenderer.enabled = true;
                }
                if ((this.renderMesh == null) || (this.renderMesh.Length == 0))
                {
                    Debug.LogError("GUI Manager not initialized before Render() called");
                }
                else
                {
                    this.resetDrawCalls();
                    dfRenderData buffer = null;
                    this.clipStack.Clear();
                    this.clipStack.Push(ClipRegion.Obtain());
                    uint checksum = dfChecksumUtil.START_VALUE;
                    using (dfList<dfControl> list = this.getTopLevelControls())
                    {
                        this.updateRenderOrder(list);
                        for (int i = 0; i < list.Count; i++)
                        {
                            dfControl control = list[i];
                            this.renderControl(ref buffer, control, checksum, 1f);
                        }
                    }
                    if (<>f__am$cache26 == null)
                    {
                        <>f__am$cache26 = x => x.Vertices.Count == 0;
                    }
                    this.drawCallBuffers.RemoveAll(<>f__am$cache26);
                    this.drawCallCount = this.drawCallBuffers.Count;
                    this.TotalDrawCalls = this.drawCallCount;
                    if (this.drawCallBuffers.Count == 0)
                    {
                        if (this.renderFilter.sharedMesh != null)
                        {
                            this.renderFilter.sharedMesh.Clear();
                        }
                    }
                    else
                    {
                        this.meshRenderer.sharedMaterials = this.gatherMaterials();
                        dfRenderData data2 = this.compileMasterBuffer();
                        this.TotalTriangles = data2.Triangles.Count / 3;
                        Mesh mesh2 = this.getRenderMesh();
                        this.renderFilter.sharedMesh = mesh2;
                        Mesh mesh = mesh2;
                        mesh.Clear();
                        mesh.vertices = data2.Vertices.Items;
                        mesh.uv = data2.UV.Items;
                        mesh.colors32 = data2.Colors.Items;
                        if (this.generateNormals && (data2.Normals.Items.Length == data2.Vertices.Items.Length))
                        {
                            mesh.normals = data2.Normals.Items;
                            mesh.tangents = data2.Tangents.Items;
                        }
                        mesh.subMeshCount = this.submeshes.Count;
                        for (int j = 0; j < this.submeshes.Count; j++)
                        {
                            int sourceIndex = this.submeshes[j];
                            int length = data2.Triangles.Count - sourceIndex;
                            if (j < (this.submeshes.Count - 1))
                            {
                                length = this.submeshes[j + 1] - sourceIndex;
                            }
                            int[] dest = new int[length];
                            data2.Triangles.CopyTo(sourceIndex, dest, 0, length);
                            mesh.SetTriangles(dest, j);
                        }
                        if (this.clipStack.Count != 1)
                        {
                            Debug.LogError("Clip stack not properly maintained");
                        }
                        this.clipStack.Pop().Release();
                        this.clipStack.Clear();
                    }
                }
            }
        }
        catch (dfAbortRenderingException)
        {
            this.isDirty = true;
        }
        finally
        {
            if (AfterRender != null)
            {
                AfterRender(this);
            }
        }
    }

    private void renderControl(ref dfRenderData buffer, dfControl control, uint checksum, float opacity)
    {
        if (control.GetIsVisibleRaw())
        {
            float num = opacity * control.Opacity;
            if (opacity > 0.005f)
            {
                ClipRegion clipInfo = this.clipStack.Peek();
                checksum = dfChecksumUtil.Calculate(checksum, control.Version);
                Bounds bounds = control.GetBounds();
                bool flag = false;
                if (control is IDFMultiRender)
                {
                    dfList<dfRenderData> list = ((IDFMultiRender) control).RenderMultiple();
                    if (list != null)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            dfRenderData controlData = list[j];
                            if (this.processRenderData(ref buffer, controlData, bounds, checksum, clipInfo))
                            {
                                flag = true;
                            }
                        }
                    }
                }
                else
                {
                    dfRenderData data = control.Render();
                    if (data == null)
                    {
                        return;
                    }
                    if (this.processRenderData(ref buffer, data, bounds, checksum, clipInfo))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.ControlsRendered++;
                    this.occluders.Add(this.getControlOccluder(control));
                }
                if (control.ClipChildren)
                {
                    clipInfo = ClipRegion.Obtain(clipInfo, control);
                    this.clipStack.Push(clipInfo);
                }
                for (int i = 0; i < control.Controls.Count; i++)
                {
                    dfControl control2 = control.Controls[i];
                    this.renderControl(ref buffer, control2, checksum, num);
                }
                if (control.ClipChildren)
                {
                    this.clipStack.Pop().Release();
                }
            }
        }
    }

    private int renderSortFunc(dfControl lhs, dfControl rhs)
    {
        return lhs.RenderOrder.CompareTo(rhs.RenderOrder);
    }

    private void resetDrawCalls()
    {
        this.drawCallCount = 0;
        for (int i = 0; i < this.drawCallBuffers.Count; i++)
        {
            this.drawCallBuffers[i].Release();
        }
        this.drawCallBuffers.Clear();
    }

    public Vector2 ScreenToGui(Vector2 position)
    {
        position.y = this.GetScreenSize().y - position.y;
        return position;
    }

    public void SendToBack(dfControl control)
    {
        if (control.Parent != null)
        {
            control = control.GetRootContainer();
        }
        using (dfList<dfControl> list = this.getTopLevelControls())
        {
            int num = 1;
            for (int i = 0; i < list.Count; i++)
            {
                dfControl control2 = list[i];
                if (control2 != control)
                {
                    control2.ZOrder = num++;
                }
            }
            control.ZOrder = 0;
            this.Invalidate();
        }
    }

    public static void SetFocus(dfControl control)
    {
        <SetFocus>c__AnonStorey56 storey = new <SetFocus>c__AnonStorey56();
        if (dfGUIManager.activeControl != control)
        {
            dfControl activeControl = dfGUIManager.activeControl;
            dfGUIManager.activeControl = control;
            storey.args = new dfFocusEventArgs(control, activeControl);
            storey.prevFocusChain = dfList<dfControl>.Obtain();
            if (activeControl != null)
            {
                for (dfControl control3 = activeControl; control3 != null; control3 = control3.Parent)
                {
                    storey.prevFocusChain.Add(control3);
                }
            }
            storey.newFocusChain = dfList<dfControl>.Obtain();
            if (control != null)
            {
                for (dfControl control4 = control; control4 != null; control4 = control4.Parent)
                {
                    storey.newFocusChain.Add(control4);
                }
            }
            if (activeControl != null)
            {
                storey.prevFocusChain.ForEach(new Action<dfControl>(storey.<>m__20));
                activeControl.OnLostFocus(storey.args);
            }
            if (control != null)
            {
                storey.newFocusChain.ForEach(new Action<dfControl>(storey.<>m__21));
                control.OnGotFocus(storey.args);
            }
            storey.newFocusChain.Release();
            storey.prevFocusChain.Release();
        }
    }

    public virtual void Start()
    {
        foreach (Camera camera1 in Object.FindObjectsOfType(typeof(Camera)) as Camera[])
        {
            camera1.eventMask &= ~(((int) 1) << base.gameObject.layer);
        }
        dfInputManager component = base.GetComponent<dfInputManager>();
        if (component == null)
        {
        }
        this.inputManager = base.gameObject.AddComponent<dfInputManager>();
        this.inputManager.RenderCamera = this.RenderCamera;
        this.FramesRendered = 0;
        this.invalidateAllControls();
        this.updateRenderOrder(null);
        if (this.meshRenderer != null)
        {
            this.meshRenderer.enabled = true;
        }
    }

    public virtual void Update()
    {
        if ((this.renderCamera == null) || !base.enabled)
        {
            if (this.meshRenderer != null)
            {
                this.meshRenderer.enabled = false;
            }
        }
        else
        {
            if ((this.renderMesh == null) || (this.renderMesh.Length == 0))
            {
                this.initialize();
                if (Application.isEditor && !Application.isPlaying)
                {
                    this.Render();
                }
            }
            if (this.cachedChildCount != base.transform.childCount)
            {
                this.cachedChildCount = base.transform.childCount;
                this.Invalidate();
            }
            Vector2 screenSize = this.GetScreenSize();
            Vector2 vector2 = screenSize - this.cachedScreenSize;
            if (vector2.sqrMagnitude > float.Epsilon)
            {
                this.onResolutionChanged(this.cachedScreenSize, screenSize);
                this.cachedScreenSize = screenSize;
            }
        }
    }

    private void updateRenderCamera(Camera camera)
    {
        if (Application.isPlaying && (camera.targetTexture != null))
        {
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
        }
        else
        {
            camera.clearFlags = CameraClearFlags.Depth;
        }
        Vector3 vector = !Application.isPlaying ? Vector3.zero : ((Vector3) (-this.uiOffset * this.PixelsToUnits()));
        if (camera.isOrthoGraphic)
        {
            camera.nearClipPlane = Mathf.Min(camera.nearClipPlane, -1f);
            camera.farClipPlane = Mathf.Max(camera.farClipPlane, 1f);
        }
        else
        {
            float num = camera.fieldOfView * 0.01745329f;
            Vector3[] corners = this.GetCorners();
            float num3 = Vector3.Distance(corners[3], corners[0]) / (2f * Mathf.Tan(num / 2f));
            Vector3 vector2 = (Vector3) (base.transform.TransformDirection(Vector3.back) * num3);
            camera.farClipPlane = Mathf.Max(num3 * 2f, camera.farClipPlane);
            vector += vector2;
        }
        if (Application.isPlaying && this.needHalfPixelOffset())
        {
            float pixelHeight = camera.pixelHeight;
            float num5 = (2f / pixelHeight) * (pixelHeight / ((float) this.FixedHeight));
            Vector3 vector3 = new Vector3(num5 * 0.5f, num5 * -0.5f, 0f);
            vector += vector3;
        }
        if (!this.overrideCamera)
        {
            if (Vector3.SqrMagnitude(camera.transform.localPosition - vector) > float.Epsilon)
            {
                camera.transform.localPosition = vector;
            }
            camera.transform.hasChanged = false;
        }
    }

    private void updateRenderOrder(dfList<dfControl> list = null)
    {
        dfList<dfControl> list2 = (list == null) ? this.getTopLevelControls() : list;
        list2.Sort();
        int order = 0;
        for (int i = 0; i < list2.Count; i++)
        {
            dfControl control = list2[i];
            if (control.Parent == null)
            {
                control.setRenderOrder(ref order);
            }
        }
    }

    private void updateRenderSettings()
    {
        Camera renderCamera = this.RenderCamera;
        if (renderCamera != null)
        {
            if (!this.overrideCamera)
            {
                this.updateRenderCamera(renderCamera);
            }
            if (base.transform.hasChanged)
            {
                Vector3 localScale = base.transform.localScale;
                if (((localScale.x < float.Epsilon) || !Mathf.Approximately(localScale.x, localScale.y)) || !Mathf.Approximately(localScale.x, localScale.z))
                {
                    localScale.y = localScale.z = localScale.x = Mathf.Max(localScale.x, 0.001f);
                    base.transform.localScale = localScale;
                }
            }
            if (!this.overrideCamera)
            {
                if (Application.isPlaying && this.PixelPerfectMode)
                {
                    float num = renderCamera.pixelHeight / ((float) this.fixedHeight);
                    renderCamera.orthographicSize = num;
                    renderCamera.fieldOfView = 60f * num;
                }
                else
                {
                    renderCamera.orthographicSize = 1f;
                    renderCamera.fieldOfView = 60f;
                }
            }
            renderCamera.transparencySortMode = TransparencySortMode.Orthographic;
            if (this.cachedScreenSize.sqrMagnitude <= float.Epsilon)
            {
                this.cachedScreenSize = new Vector2((float) this.FixedWidth, (float) this.FixedHeight);
            }
            base.transform.hasChanged = false;
        }
    }

    public Vector2 WorldPointToGUI(Vector3 worldPoint)
    {
        Vector2 screenSize = this.GetScreenSize();
        Camera main = Camera.main;
        Vector3 position = Camera.main.WorldToScreenPoint(worldPoint);
        position.x = screenSize.x * (position.x / main.pixelWidth);
        position.y = screenSize.y * (position.y / main.pixelHeight);
        return this.ScreenToGui(position);
    }

    public static dfControl ActiveControl
    {
        get
        {
            return activeControl;
        }
    }

    public bool ConsumeMouseEvents
    {
        get
        {
            return this.consumeMouseEvents;
        }
        set
        {
            this.consumeMouseEvents = value;
        }
    }

    public int ControlsRendered { get; private set; }

    public dfAtlas DefaultAtlas
    {
        get
        {
            return this.atlas;
        }
        set
        {
            if (!dfAtlas.Equals(value, this.atlas))
            {
                this.atlas = value;
                this.invalidateAllControls();
            }
        }
    }

    public dfFont DefaultFont
    {
        get
        {
            return this.defaultFont;
        }
        set
        {
            if (value != this.defaultFont)
            {
                this.defaultFont = value;
                this.invalidateAllControls();
            }
        }
    }

    public int FixedHeight
    {
        get
        {
            return this.fixedHeight;
        }
        set
        {
            if (value != this.fixedHeight)
            {
                int fixedHeight = this.fixedHeight;
                this.fixedHeight = value;
                this.onResolutionChanged(fixedHeight, value);
            }
        }
    }

    public int FixedWidth
    {
        get
        {
            return this.fixedWidth;
        }
        set
        {
            if (value != this.fixedWidth)
            {
                this.fixedWidth = value;
                this.onResolutionChanged();
            }
        }
    }

    public int FramesRendered { get; private set; }

    public bool GenerateNormals
    {
        get
        {
            return this.generateNormals;
        }
        set
        {
            if (value != this.generateNormals)
            {
                this.generateNormals = value;
                if (this.renderMesh != null)
                {
                    this.renderMesh[0].Clear();
                    this.renderMesh[1].Clear();
                }
                dfRenderData.FlushObjectPool();
                this.invalidateAllControls();
            }
        }
    }

    public bool MergeMaterials
    {
        get
        {
            return this.mergeMaterials;
        }
        set
        {
            if (value != this.mergeMaterials)
            {
                this.mergeMaterials = value;
                this.invalidateAllControls();
            }
        }
    }

    public bool OverrideCamera
    {
        get
        {
            return this.overrideCamera;
        }
        set
        {
            this.overrideCamera = value;
        }
    }

    public bool PixelPerfectMode
    {
        get
        {
            return this.pixelPerfectMode;
        }
        set
        {
            if (value != this.pixelPerfectMode)
            {
                this.pixelPerfectMode = value;
                this.onResolutionChanged();
                this.Invalidate();
            }
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
            if (!object.ReferenceEquals(this.renderCamera, value))
            {
                this.renderCamera = value;
                this.Invalidate();
                if ((value != null) && (value.gameObject.GetComponent<dfGUICamera>() == null))
                {
                    value.gameObject.AddComponent<dfGUICamera>();
                }
                if (this.inputManager != null)
                {
                    this.inputManager.RenderCamera = value;
                }
            }
        }
    }

    public int TotalDrawCalls { get; private set; }

    public int TotalTriangles { get; private set; }

    public Vector2 UIOffset
    {
        get
        {
            return this.uiOffset;
        }
        set
        {
            if (!object.Equals(this.uiOffset, value))
            {
                this.uiOffset = value;
                this.Invalidate();
            }
        }
    }

    public float UIScale
    {
        get
        {
            return this.uiScale;
        }
        set
        {
            if (!Mathf.Approximately(value, this.uiScale))
            {
                this.uiScale = value;
                this.onResolutionChanged();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <OnGUI>c__AnonStorey55
    {
        internal Vector2 touchPosition;

        internal bool <>m__1F(Rect x)
        {
            return x.Contains(this.touchPosition);
        }
    }

    [CompilerGenerated]
    private sealed class <SetFocus>c__AnonStorey56
    {
        internal dfFocusEventArgs args;
        internal dfList<dfControl> newFocusChain;
        internal dfList<dfControl> prevFocusChain;

        internal void <>m__20(dfControl c)
        {
            if (!this.newFocusChain.Contains(c))
            {
                c.OnLeaveFocus(this.args);
            }
        }

        internal void <>m__21(dfControl c)
        {
            if (!this.prevFocusChain.Contains(c))
            {
                c.OnEnterFocus(this.args);
            }
        }
    }

    private class ClipRegion
    {
        private dfList<Plane> planes = new dfList<Plane>();
        private static Queue<dfGUIManager.ClipRegion> pool = new Queue<dfGUIManager.ClipRegion>();

        private ClipRegion()
        {
        }

        public void clipToPlanes(dfList<Plane> planes, dfRenderData data, dfRenderData dest, uint controlChecksum)
        {
            if ((data != null) && (data.Vertices.Count != 0))
            {
                if ((planes == null) || (planes.Count == 0))
                {
                    dest.Merge(data, true);
                }
                else
                {
                    dfClippingUtil.Clip(planes, data, dest);
                }
            }
        }

        public static dfGUIManager.ClipRegion Obtain()
        {
            return ((pool.Count <= 0) ? new dfGUIManager.ClipRegion() : pool.Dequeue());
        }

        public static dfGUIManager.ClipRegion Obtain(dfGUIManager.ClipRegion parent, dfControl control)
        {
            dfGUIManager.ClipRegion region = (pool.Count <= 0) ? new dfGUIManager.ClipRegion() : pool.Dequeue();
            region.planes.AddRange(control.GetClippingPlanes());
            if (parent != null)
            {
                region.planes.AddRange(parent.planes);
            }
            return region;
        }

        public bool PerformClipping(dfRenderData dest, Bounds bounds, uint checksum, dfRenderData controlData)
        {
            dfIntersectionType type;
            if (controlData.Checksum == checksum)
            {
                if (controlData.Intersection == dfIntersectionType.Inside)
                {
                    dest.Merge(controlData, true);
                    return true;
                }
                if (controlData.Intersection == dfIntersectionType.None)
                {
                    return false;
                }
            }
            bool flag = false;
            using (dfList<Plane> list = this.TestIntersection(bounds, out type))
            {
                switch (type)
                {
                    case dfIntersectionType.Inside:
                        dest.Merge(controlData, true);
                        flag = true;
                        break;

                    case dfIntersectionType.Intersecting:
                        this.clipToPlanes(list, controlData, dest, checksum);
                        flag = true;
                        break;
                }
                controlData.Checksum = checksum;
                controlData.Intersection = type;
            }
            return flag;
        }

        public void Release()
        {
            this.planes.Clear();
            pool.Enqueue(this);
        }

        private static int sortClipPlanes(Plane lhs, Plane rhs)
        {
            return lhs.distance.CompareTo(rhs.distance);
        }

        public dfList<Plane> TestIntersection(Bounds bounds, out dfIntersectionType type)
        {
            if ((this.planes == null) || (this.planes.Count == 0))
            {
                type = dfIntersectionType.Inside;
                return null;
            }
            dfList<Plane> list = dfList<Plane>.Obtain(this.planes.Count);
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            bool flag = false;
            for (int i = 0; i < this.planes.Count; i++)
            {
                Plane item = this.planes[i];
                Vector3 normal = item.normal;
                float distance = item.distance;
                float num3 = ((extents.x * Mathf.Abs(normal.x)) + (extents.y * Mathf.Abs(normal.y))) + (extents.z * Mathf.Abs(normal.z));
                float f = Vector3.Dot(normal, center) + distance;
                if (Mathf.Abs(f) <= num3)
                {
                    flag = true;
                    list.Add(item);
                }
                else if (f < -num3)
                {
                    type = dfIntersectionType.None;
                    list.Release();
                    return null;
                }
            }
            if (flag)
            {
                type = dfIntersectionType.Intersecting;
                return list;
            }
            type = dfIntersectionType.Inside;
            list.Release();
            return null;
        }
    }

    private class MaterialCache
    {
        private static Dictionary<Material, Cache> cache = new Dictionary<Material, Cache>();

        public static Material Lookup(Material BaseMaterial)
        {
            if (BaseMaterial == null)
            {
                Debug.LogError("Cache lookup on null material");
                return null;
            }
            Cache cache = null;
            if (!dfGUIManager.MaterialCache.cache.TryGetValue(BaseMaterial, out cache))
            {
                Cache cache2 = new Cache(BaseMaterial);
                dfGUIManager.MaterialCache.cache[BaseMaterial] = cache2;
                cache = cache2;
            }
            return cache.Obtain();
        }

        public static void Reset()
        {
            Cache.ResetAll();
        }

        private class Cache
        {
            private Material baseMaterial;
            private static List<dfGUIManager.MaterialCache.Cache> cacheInstances = new List<dfGUIManager.MaterialCache.Cache>();
            private int currentIndex;
            private List<Material> instances;

            private Cache()
            {
                this.instances = new List<Material>(10);
                throw new NotImplementedException();
            }

            public Cache(Material BaseMaterial)
            {
                this.instances = new List<Material>(10);
                this.baseMaterial = BaseMaterial;
                this.instances.Add(BaseMaterial);
                cacheInstances.Add(this);
            }

            public Material Obtain()
            {
                if (this.currentIndex < this.instances.Count)
                {
                    return this.instances[this.currentIndex++];
                }
                this.currentIndex++;
                Material item = new Material(this.baseMaterial) {
                    hideFlags = HideFlags.DontSave,
                    name = string.Format("{0} (Copy {1})", this.baseMaterial.name, this.currentIndex)
                };
                this.instances.Add(item);
                return item;
            }

            public void Reset()
            {
                this.currentIndex = 0;
            }

            public static void ResetAll()
            {
                for (int i = 0; i < cacheInstances.Count; i++)
                {
                    cacheInstances[i].Reset();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ModalControlReference
    {
        public dfControl control;
        public dfGUIManager.ModalPoppedCallback callback;
    }

    [dfEventCategory("Modal Dialog")]
    public delegate void ModalPoppedCallback(dfControl control);

    [dfEventCategory("Global Callbacks")]
    public delegate void RenderCallback(dfGUIManager manager);
}

