using Facepunch.Precision;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFX : IDRemote
{
    [SerializeField]
    private MonoBehaviour[] _effects;
    private static bool _hasMainCamera;
    private static bool _hasMainCameraFX;
    private static Camera _mainCamera;
    private static CameraFX _mainCameraFX;
    private static bool _mainIsMount;
    private static MountedCamera _mainMountedCamera;
    [NonSerialized]
    private AdaptiveNearPlane adaptiveNearPlane;
    private bool awoke;
    [SerializeField]
    private float baseFieldOfView = 60f;
    [NonSerialized]
    public Camera camera;
    private Matrix4x4G cameraToWorldMatrix;
    private Matrix4x4G cameraToWorldMatrixUnAltered;
    private ICameraFX[] effects;
    private static CameraTransitionData g_trans = CameraTransitionData.identity;
    private IHeldItem item;
    private Matrix4x4G localToWorldMatrix;
    private Vector3 preLocalPosition;
    private Matrix4x4 preProjectionMatrix;
    private Quaternion preRotation;
    private Matrix4x4G projectionMatrix;
    private Matrix4x4G projectionMatrixUnAltered;
    private MatrixHelper.ProjectHelperG projectScreen;
    private MatrixHelper.ProjectHelperG projectViewport;
    [SerializeField]
    private bool recalcViewMatrix = true;
    private ItemRepresentation rep;
    private Vector3 restoreViewModelPosition;
    private ViewModel viewModel;
    [SerializeField]
    private Material viewModelPostdrawMaterial;
    [SerializeField]
    private Material viewModelPredrawMaterial;
    private static Transform viewModelRootTransform;
    private static bool vm_flip = false;
    private static bool vm_projuse = false;
    private Matrix4x4G worldToCameraMatrix;
    private Matrix4x4G worldToCameraMatrixUnAltered;
    private Matrix4x4G worldToLocalMatrix;

    internal static void ApplyTransitionAlterations(Camera camera, CameraFX fx, bool useFX)
    {
        if (useFX)
        {
            int num = g_trans.Update(ref fx.worldToCameraMatrix, ref fx.projectionMatrix);
            if ((num & 1) == 1)
            {
                camera.worldToCameraMatrix = fx.worldToCameraMatrix.f;
                Matrix4x4G.Inverse(ref fx.worldToCameraMatrix, out fx.cameraToWorldMatrix);
            }
            if ((num & 2) == 2)
            {
                camera.projectionMatrix = fx.projectionMatrix.f;
            }
        }
        else
        {
            Matrix4x4G matrixxg;
            Matrix4x4G matrixxg2;
            camera.ExtractCameraMatrixWorldToCamera(out matrixxg);
            camera.ExtractCameraMatrixProjection(out matrixxg2);
            int num2 = g_trans.Update(ref matrixxg, ref matrixxg2);
            if ((num2 & 1) == 1)
            {
                camera.ResetWorldToCameraMatrix();
                camera.worldToCameraMatrix = matrixxg.f;
            }
            if ((num2 & 2) == 2)
            {
                camera.ResetProjectionMatrix();
                camera.projectionMatrix = matrixxg2.f;
            }
        }
    }

    protected void Awake()
    {
        this.camera = base.camera;
        this.adaptiveNearPlane = base.GetComponent<AdaptiveNearPlane>();
        int newSize = 0;
        if ((this._effects != null) && (this._effects.Length != 0))
        {
            for (int i = 0; i < this._effects.Length; i++)
            {
                if ((this._effects[i] != null) && (this._effects[i] is ICameraFX))
                {
                    this._effects[newSize++] = this._effects[i];
                }
                else
                {
                    Debug.LogWarning("effect at index " + i + " is missing, null or not a ICameraFX", this);
                }
            }
        }
        Array.Resize<MonoBehaviour>(ref this._effects, newSize);
        Array.Resize<ICameraFX>(ref this.effects, newSize);
        if (newSize == 0)
        {
            Debug.LogWarning("There are no effects", this);
        }
        else
        {
            for (int j = 0; j < newSize; j++)
            {
                this.effects[j] = (ICameraFX) this._effects[j];
            }
        }
        this.awoke = true;
        if (this.viewModel != null)
        {
            ViewModel viewModel = this.viewModel;
            this.viewModel = null;
            ItemRepresentation rep = this.rep;
            this.rep = null;
            IHeldItem item = this.item;
            this.item = null;
            this.SetViewModel(viewModel, rep, item);
        }
        base.Awake();
    }

    private static bool Bind()
    {
        return (bool) mainCameraFX;
    }

    public Vector3 InverseTransformDirection(Vector3 v)
    {
        return this.worldToLocalMatrix.f.MultiplyVector(v);
    }

    public Vector3 InverseTransformPoint(Vector3 v)
    {
        return this.worldToLocalMatrix.f.MultiplyPoint3x4(v);
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        if (_mainCameraFX == this)
        {
            _mainCamera = null;
            _mainCameraFX = null;
            _hasMainCameraFX = false;
        }
    }

    public void PostPostRender()
    {
        base.transform.localPosition = this.preLocalPosition;
        base.transform.rotation = this.preRotation;
        this.camera.projectionMatrix = this.preProjectionMatrix;
    }

    public void PostPreCull()
    {
        Matrix4x4G projectionMatrix;
        PerspectiveMatrixBuilder builder;
        Vector4 vector2;
        Matrix4x4G matrixxg5;
        Matrix4x4G matrixxg6;
        Matrix4x4G matrixxg7;
        Matrix4x4G matrixxg8;
        Matrix4x4G matrixxg9;
        Matrix4x4G matrixxg10;
        Matrix4x4G matrixxg11;
        Matrix4x4G matrixxg12;
        if (viewModelRootTransform != null)
        {
            Quaternion localRotation = base.transform.localRotation;
            Vector3 localPosition = base.transform.localPosition;
            if (this.viewModel != null)
            {
                this.viewModel.ModifyAiming(new Ray(base.transform.parent.position, base.transform.parent.forward), ref localPosition, ref localRotation);
            }
            viewModelRootTransform.localRotation = Quaternion.Inverse(localRotation);
            viewModelRootTransform.localPosition = -localPosition;
        }
        this.camera.transform.ExtractLocalToWorldToLocal(out this.localToWorldMatrix, out this.worldToLocalMatrix);
        if (this.adaptiveNearPlane != null)
        {
            Vector3G vectorg2;
            Matrix4x4G matrixxg;
            Matrix4x4G matrixxg2;
            int layerMask = (this.camera.cullingMask & ~this.adaptiveNearPlane.ignoreLayers.value) | this.adaptiveNearPlane.forceLayers.value;
            Vector3G a = new Vector3G();
            this.localToWorldMatrix.MultiplyPoint(ref a, out vectorg2);
            Collider[] colliderArray = Physics.OverlapSphere(vectorg2.f, this.adaptiveNearPlane.minNear + this.adaptiveNearPlane.maxNear, layerMask);
            int num2 = -1;
            float positiveInfinity = float.PositiveInfinity;
            double fieldOfView = this.camera.fieldOfView;
            double aspect = this.camera.aspect;
            double minNear = this.adaptiveNearPlane.minNear;
            double zfar = this.adaptiveNearPlane.maxNear + this.adaptiveNearPlane.threshold;
            float num8 = this.adaptiveNearPlane.minNear;
            float num9 = (this.adaptiveNearPlane.maxNear + this.adaptiveNearPlane.threshold) - num8;
            Matrix4x4G.Perspective(ref fieldOfView, ref aspect, ref minNear, ref zfar, out matrixxg);
            Matrix4x4G.Inverse(ref matrixxg, out matrixxg2);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector3G vectorg3;
                    Vector3G vectorg4;
                    Vector3G vectorg5;
                    Vector3G vectorg6;
                    Vector3G vectorg7;
                    Vector3G vectorg8;
                    vectorg3.x = (i - 3.5) / 3.5;
                    vectorg3.y = (j - 3.5) / 3.5;
                    vectorg3.z = 0.0;
                    matrixxg2.MultiplyPoint(ref vectorg3, out vectorg4);
                    vectorg3.z = 1.0;
                    matrixxg2.MultiplyPoint(ref vectorg3, out vectorg5);
                    vectorg4.x = -vectorg4.x;
                    vectorg4.y = -vectorg4.y;
                    vectorg4.z = -vectorg4.z;
                    vectorg5.x = -vectorg5.x;
                    vectorg5.y = -vectorg5.y;
                    vectorg5.z = -vectorg5.z;
                    this.localToWorldMatrix.MultiplyPoint(ref vectorg4, out vectorg6);
                    this.localToWorldMatrix.MultiplyPoint(ref vectorg5, out vectorg7);
                    vectorg8.x = vectorg7.x - vectorg6.x;
                    vectorg8.y = vectorg7.y - vectorg6.y;
                    vectorg8.z = vectorg7.z - vectorg6.z;
                    float num12 = (float) Math.Sqrt(((vectorg8.x * vectorg8.x) + (vectorg8.y * vectorg8.y)) + (vectorg8.z * vectorg8.z));
                    float distance = num12;
                    Ray ray = new Ray(vectorg6.f, vectorg8.f);
                    for (int k = 0; k < colliderArray.Length; k++)
                    {
                        RaycastHit hit;
                        Collider collider = colliderArray[k];
                        if (collider.Raycast(ray, out hit, distance))
                        {
                            float num15 = hit.distance;
                            if (num15 < distance)
                            {
                                distance = num15;
                                float num16 = num8 + ((num15 / num12) * num9);
                                if (positiveInfinity > num16)
                                {
                                    positiveInfinity = num16;
                                    num2 = k;
                                }
                            }
                        }
                    }
                }
            }
            if (float.IsInfinity(positiveInfinity))
            {
                this.camera.nearClipPlane = this.adaptiveNearPlane.maxNear;
            }
            else
            {
                positiveInfinity -= this.adaptiveNearPlane.threshold;
                if (positiveInfinity >= this.adaptiveNearPlane.maxNear)
                {
                    this.camera.nearClipPlane = this.adaptiveNearPlane.maxNear;
                }
                else if (positiveInfinity <= this.adaptiveNearPlane.minNear)
                {
                    this.camera.nearClipPlane = this.adaptiveNearPlane.minNear;
                }
                else
                {
                    this.camera.nearClipPlane = positiveInfinity;
                }
            }
        }
        builder.fieldOfView = this.camera.fieldOfView;
        builder.aspectRatio = this.camera.aspect;
        builder.nearPlane = this.camera.nearClipPlane;
        builder.farPlane = this.camera.farClipPlane;
        PerspectiveMatrixBuilder perspective = builder;
        if (this.camera.isOrthoGraphic)
        {
            this.projectionMatrix.f = this.camera.projectionMatrix;
            projectionMatrix = this.projectionMatrix;
        }
        else
        {
            if (this.viewModel != null)
            {
                this.viewModel.ModifyPerspective(ref perspective);
            }
            if (vm_projuse)
            {
                perspective.ToProjectionMatrix(out this.projectionMatrix);
            }
            else
            {
                builder.ToProjectionMatrix(out this.projectionMatrix);
            }
            this.camera.projectionMatrix = this.projectionMatrix.f;
            perspective.ToProjectionMatrix(out projectionMatrix);
        }
        vector2.y = (float) perspective.nearPlane;
        vector2.z = (float) perspective.farPlane;
        vector2.w = (float) (1.0 / perspective.farPlane);
        if (vm_flip == PLATFORM_POLL.flipRequired)
        {
            vector2.x = 1f;
            Shader.SetGlobalMatrix("V_MUNITY_MATRIX_P", projectionMatrix.f);
        }
        else
        {
            PerspectiveMatrixBuilder builder3;
            Matrix4x4G matrixxg4;
            vector2.x = -1f;
            builder3.nearPlane = perspective.nearPlane;
            builder3.farPlane = perspective.farPlane;
            builder3.fieldOfView = -perspective.fieldOfView;
            builder3.aspectRatio = -perspective.aspectRatio;
            builder3.ToProjectionMatrix(out matrixxg4);
            Shader.SetGlobalMatrix("V_MUNITY_MATRIX_P", matrixxg4.f);
        }
        Shader.SetGlobalVector("V_M_ProjectionParams", vector2);
        if (this.recalcViewMatrix)
        {
            Vector3G vectorg9;
            QuaternionG ng;
            Vector3G vectorg10;
            this.camera.transform.ExtractWorldCoordinates(out vectorg9, out ng, out vectorg10);
            vectorg10.x = 1.0;
            vectorg10.y = 1.0;
            vectorg10.z = -1.0;
            Matrix4x4G.TRS(ref vectorg9, ref ng, ref vectorg10, out this.cameraToWorldMatrix);
            if (Matrix4x4G.Inverse(ref this.cameraToWorldMatrix, out this.worldToCameraMatrix))
            {
                this.camera.worldToCameraMatrix = this.worldToCameraMatrix.f;
            }
        }
        else
        {
            this.cameraToWorldMatrix.f = this.camera.cameraToWorldMatrix;
            this.worldToCameraMatrix.f = this.camera.worldToCameraMatrix;
        }
        this.worldToCameraMatrixUnAltered = this.worldToCameraMatrix;
        this.cameraToWorldMatrixUnAltered = this.cameraToWorldMatrix;
        this.projectionMatrixUnAltered = this.projectionMatrix;
        ApplyTransitionAlterations(this.camera, this, true);
        this.projectScreen.modelview = this.projectViewport.modelview = this.worldToCameraMatrix;
        this.projectScreen.projection = this.projectViewport.projection = this.projectionMatrix;
        Rect pixelRect = this.camera.pixelRect;
        this.projectScreen.offset.x = pixelRect.x;
        this.projectScreen.offset.y = pixelRect.y;
        this.projectScreen.size.x = pixelRect.width;
        this.projectScreen.size.y = pixelRect.height;
        pixelRect = this.camera.rect;
        this.projectViewport.offset.x = pixelRect.x;
        this.projectViewport.offset.y = pixelRect.y;
        this.projectViewport.size.x = pixelRect.width;
        this.projectViewport.size.y = pixelRect.height;
        Matrix4x4G.Mult(ref this.localToWorldMatrix, ref this.worldToCameraMatrix, out matrixxg8);
        Matrix4x4G.Mult(ref matrixxg8, ref this.projectionMatrix, out matrixxg5);
        Matrix4x4G.Inverse(ref matrixxg8, out matrixxg9);
        Matrix4x4G.Inverse(ref matrixxg5, out matrixxg6);
        Matrix4x4G.Inverse(ref this.localToWorldMatrix, out matrixxg11);
        Matrix4x4G.Transpose(ref matrixxg6, out matrixxg7);
        Matrix4x4G.Transpose(ref matrixxg9, out matrixxg10);
        Matrix4x4G.Transpose(ref matrixxg11, out matrixxg12);
        if (this.viewModel != null)
        {
            this.viewModel.UpdateProxies();
        }
        BoundHack.Achieve(base.transform.position);
        ContextSprite.UpdateSpriteFading(this.camera);
        PlayerClient localPlayerClient = PlayerClient.localPlayerClient;
        if (localPlayerClient != null)
        {
            localPlayerClient.ProcessLocalPlayerPreRender();
        }
        RPOS.BeforeSceneRender_Internal(this.camera);
    }

    public void PrePostRender()
    {
        this.camera.ResetWorldToCameraMatrix();
        for (int i = this._effects.Length - 1; i >= 0; i--)
        {
            if ((this._effects[i] != null) && this._effects[i].enabled)
            {
                this.effects[i].PostRender();
            }
        }
    }

    public void PrePreCull()
    {
        this.camera.ResetProjectionMatrix();
        this.preProjectionMatrix = this.camera.projectionMatrix;
        this.preLocalPosition = base.transform.localPosition;
        this.preRotation = base.transform.rotation;
        for (int i = 0; i < this._effects.Length; i++)
        {
            if ((this._effects[i] != null) && this._effects[i].enabled)
            {
                this.effects[i].PreCull();
            }
        }
    }

    public static void RemoveViewModel()
    {
        if (mainViewModel != null)
        {
            ReplaceViewModel(null, false);
        }
    }

    public static void RemoveViewModel(ref ViewModel vm, bool deleteEvenIfNotCurrent, bool removeCurrentIfNotVM)
    {
        if (vm == null)
        {
            if (removeCurrentIfNotVM)
            {
                RemoveViewModel();
            }
        }
        else if (mainViewModel == vm)
        {
            ReplaceViewModel(null, false);
            vm = null;
        }
        else
        {
            if (deleteEvenIfNotCurrent)
            {
                Object.Destroy(vm.gameObject);
                vm = null;
            }
            if (removeCurrentIfNotVM)
            {
                ReplaceViewModel(null, false);
            }
        }
    }

    public static void ReplaceViewModel(ViewModel vm, bool butDontDestroyOld)
    {
        ReplaceViewModel(vm, null, null, butDontDestroyOld);
    }

    public static void ReplaceViewModel(ViewModel vm, ItemRepresentation rep, IHeldItem item, bool butDontDestroyOld)
    {
        CameraFX mainCameraFX = CameraFX.mainCameraFX;
        if ((mainCameraFX != null) && (mainCameraFX.viewModel != vm))
        {
            ViewModel viewModel = mainCameraFX.viewModel;
            mainCameraFX.SetViewModel(vm, rep, item);
            if (!butDontDestroyOld && (viewModel != null))
            {
                Object.Destroy(viewModel.gameObject);
            }
        }
    }

    public static Ray? Screen2Ray(Vector3 point)
    {
        if (Bind())
        {
            return new Ray?(_mainCameraFX.ScreenPointToRay(point));
        }
        if (_hasMainCamera)
        {
            return new Ray?(_mainCamera.ScreenPointToRay(point));
        }
        return null;
    }

    public static Vector3? Screen2Viewport(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.ViewportToScreenPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.ViewportToScreenPoint(point));
        }
        return null;
    }

    public static Vector3? Screen2World(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.ScreenToWorldPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.ScreenToWorldPoint(point));
        }
        return null;
    }

    public Ray ScreenPointToRay(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg3;
        Vector3G win = new Vector3G(v);
        this.projectScreen.UnProject(ref win, out vectorg2);
        win.z++;
        this.projectScreen.UnProject(ref win, out vectorg3);
        return new Ray(vectorg2.f, new Vector3((float) (vectorg3.x - vectorg2.x), (float) (vectorg3.y - vectorg2.y), (float) (vectorg3.z - vectorg2.z)));
    }

    public Vector3 ScreenToViewportPoint(Vector3 v)
    {
        return this.camera.ScreenToViewportPoint(v);
    }

    public bool ScreenToWorldPoint(ref Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G((Vector3) v);
        bool flag = this.projectScreen.UnProject(ref win, out vectorg2);
        v = vectorg2.f;
        return flag;
    }

    public Vector3 ScreenToWorldPoint(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G(v);
        this.projectScreen.UnProject(ref win, out vectorg2);
        return vectorg2.f;
    }

    public bool ScreenToWorldPoint(ref Vector3 v, out Vector3 p)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G((Vector3) v);
        bool flag = this.projectScreen.UnProject(ref win, out vectorg2);
        p = vectorg2.f;
        return flag;
    }

    public void SetFieldOfView(float fieldOfView, float fraction)
    {
        this.camera.fieldOfView = (this.baseFieldOfView * (1f - fraction)) + (fieldOfView * fraction);
    }

    private void SetViewModel(ViewModel vm)
    {
        this.SetViewModel(vm, null, null);
    }

    private void SetViewModel(ViewModel vm, ItemRepresentation rep, IHeldItem item)
    {
        if (!this.awoke)
        {
            this.viewModel = vm;
            this.rep = rep;
            this.item = item;
        }
        else if (this.viewModel != vm)
        {
            if (this.viewModel != null)
            {
                if (this.viewModel.itemRep != null)
                {
                    try
                    {
                        this.viewModel.itemRep.UnBindViewModel(this.viewModel, this.viewModel.item);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, this.viewModel.itemRep);
                    }
                }
                this.viewModel.UnBindTransforms();
                this.viewModel.idMain = null;
            }
            this.viewModel = vm;
            if (vm != null)
            {
                if (viewModelRootTransform == null)
                {
                    Transform transform = new GameObject("__View Model Root").transform;
                    viewModelRootTransform = new GameObject("Eye Camera Difference").transform;
                    viewModelRootTransform.parent = transform;
                }
                vm.idMain = this.idMain;
                vm.transform.parent = viewModelRootTransform;
                if (rep != null)
                {
                    rep.PrepareViewModel(vm, item);
                }
                vm.BindTransforms(viewModelRootTransform, base.transform.parent);
                if (rep != null)
                {
                    rep.BindViewModel(vm, item);
                    vm.itemRep = rep;
                    vm.item = item;
                }
            }
            for (int i = this._effects.Length - 1; i >= 0; i--)
            {
                if (this._effects[i] != null)
                {
                    this.effects[i].OnViewModelChange(vm);
                }
            }
        }
    }

    public Vector3 TransformDirection(Vector3 v)
    {
        return this.localToWorldMatrix.f.MultiplyVector(v);
    }

    public Vector3 TransformPoint(Vector3 v)
    {
        return this.localToWorldMatrix.f.MultiplyPoint3x4(v);
    }

    public static void TransitionNow(float duration, TransitionFunction function)
    {
        if (duration <= 0f)
        {
            g_trans.end = g_trans.start = float.NegativeInfinity;
        }
        else
        {
            g_trans.Set(duration, function);
        }
    }

    public static Ray? Viewport2Ray(Vector3 point)
    {
        if (Bind())
        {
            return new Ray?(_mainCameraFX.ScreenPointToRay(point));
        }
        if (_hasMainCamera)
        {
            return new Ray?(_mainCamera.ScreenPointToRay(point));
        }
        return null;
    }

    public static Vector3? Viewport2Screen(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.ViewportToScreenPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.ViewportToScreenPoint(point));
        }
        return null;
    }

    public static Vector3? Viewport2World(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.ScreenToWorldPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.ScreenToWorldPoint(point));
        }
        return null;
    }

    public Ray ViewportPointToRay(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg3;
        Vector3G win = new Vector3G(v);
        this.projectViewport.UnProject(ref win, out vectorg2);
        win.z++;
        this.projectViewport.UnProject(ref win, out vectorg3);
        return new Ray(vectorg2.f, new Vector3((float) (vectorg3.x - vectorg2.x), (float) (vectorg3.y - vectorg2.y), (float) (vectorg3.z - vectorg2.z)));
    }

    public Vector3 ViewportToScreenPoint(Vector3 v)
    {
        return this.camera.ViewportToScreenPoint(v);
    }

    public bool ViewportToWorldPoint(ref Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G((Vector3) v);
        bool flag = this.projectViewport.UnProject(ref win, out vectorg2);
        v = vectorg2.f;
        return flag;
    }

    public Vector3 ViewportToWorldPoint(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G(v);
        this.projectViewport.UnProject(ref win, out vectorg2);
        return vectorg2.f;
    }

    public bool ViewportToWorldPoint(ref Vector3 v, out Vector3 p)
    {
        Vector3G vectorg2;
        Vector3G win = new Vector3G((Vector3) v);
        bool flag = this.projectViewport.UnProject(ref win, out vectorg2);
        p = vectorg2.f;
        return flag;
    }

    public static Vector3? World2Screen(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.WorldToScreenPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.WorldToScreenPoint(point));
        }
        return null;
    }

    public static Vector3? World2Viewport(Vector3 point)
    {
        if (Bind())
        {
            return new Vector3?(_mainCameraFX.WorldToViewportPoint(point));
        }
        if (_hasMainCamera)
        {
            return new Vector3?(_mainCamera.WorldToViewportPoint(point));
        }
        return null;
    }

    public bool WorldToScreenPoint(ref Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G((Vector3) v);
        bool flag = this.projectScreen.Project(ref vectorg, out vectorg2);
        v = vectorg2.f;
        return flag;
    }

    public Vector3 WorldToScreenPoint(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G(v);
        this.projectScreen.Project(ref vectorg, out vectorg2);
        return vectorg2.f;
    }

    public bool WorldToScreenPoint(ref Vector3 v, out Vector3 p)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G((Vector3) v);
        bool flag = this.projectScreen.Project(ref vectorg, out vectorg2);
        p = vectorg2.f;
        return flag;
    }

    public bool WorldToViewportPoint(ref Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G((Vector3) v);
        bool flag = this.projectViewport.Project(ref vectorg, out vectorg2);
        v = vectorg2.f;
        return flag;
    }

    public Vector3 WorldToViewportPoint(Vector3 v)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G(v);
        this.projectViewport.Project(ref vectorg, out vectorg2);
        return vectorg2.f;
    }

    public bool WorldToViewportPoint(ref Vector3 v, out Vector3 p)
    {
        Vector3G vectorg2;
        Vector3G vectorg = new Vector3G((Vector3) v);
        bool flag = this.projectViewport.Project(ref vectorg, out vectorg2);
        p = vectorg2.f;
        return flag;
    }

    public Character idMain
    {
        get
        {
            return (Character) base.idMain;
        }
    }

    public static CameraFX mainCameraFX
    {
        get
        {
            Camera main = Camera.main;
            if (_mainCamera != main)
            {
                _mainCamera = main;
                if (main != null)
                {
                    _hasMainCamera = true;
                    _mainIsMount = MountedCamera.IsMountedCamera(main);
                    if (_mainIsMount)
                    {
                        _mainMountedCamera = MountedCamera.main;
                        _hasMainCameraFX = (bool) (_mainCameraFX = _mainMountedCamera.cameraFX);
                    }
                    else
                    {
                        _mainMountedCamera = null;
                        _hasMainCameraFX = _mainCameraFX = main.GetComponent<CameraFX>();
                    }
                }
                else
                {
                    _hasMainCamera = false;
                    _mainIsMount = false;
                    _hasMainCameraFX = false;
                    _mainCameraFX = null;
                }
            }
            else if (_hasMainCamera && (main == null))
            {
                _hasMainCamera = false;
                _mainIsMount = false;
                _hasMainCameraFX = false;
                _mainCameraFX = null;
            }
            else if (_mainIsMount && (_mainCameraFX != _mainMountedCamera.cameraFX))
            {
                _mainCameraFX = _mainMountedCamera.cameraFX;
                _hasMainCameraFX = (bool) _mainCameraFX;
            }
            return (!_hasMainCamera ? null : (!_mainIsMount ? _mainCameraFX : MountedCamera.main.cameraFX));
        }
    }

    public static ViewModel mainViewModel
    {
        get
        {
            CameraFX mainCameraFX = CameraFX.mainCameraFX;
            return ((mainCameraFX == null) ? null : mainCameraFX.viewModel);
        }
    }

    public Material postdrawMaterial
    {
        get
        {
            return this.viewModelPostdrawMaterial;
        }
    }

    public Material predrawMaterial
    {
        get
        {
            return this.viewModelPredrawMaterial;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraTransitionData
    {
        public TransitionFunction func;
        public Matrix4x4G view;
        public Matrix4x4G proj;
        private Matrix4x4G lastView;
        private Matrix4x4G lastProj;
        public float start;
        public float end;
        public float lastTime;
        public static CameraFX.CameraTransitionData identity
        {
            get
            {
                CameraFX.CameraTransitionData data;
                return new CameraFX.CameraTransitionData { view = data.proj = data.lastView = data.lastProj = Matrix4x4G.identity, end = data.start = data.lastTime = float.NegativeInfinity, func = TransitionFunction.Linear };
            }
        }
        public int Update(ref Matrix4x4G currentView, ref Matrix4x4G currentProj)
        {
            int num3;
            try
            {
                float timeSource = CameraFX.CameraTransitionData.timeSource;
                if (this.end > timeSource)
                {
                    float t = Mathf.InverseLerp(this.start, this.end, timeSource);
                    if (t < 1f)
                    {
                        t = this.func.Evaluate(t);
                        Matrix4x4G a = TransitionFunctions.SlerpWorldToCamera((double) t, this.view, currentView);
                        Matrix4x4G matrixxg2 = TransitionFunctions.Linear((double) t, this.proj, currentProj);
                        this.lastTime = timeSource;
                        if (!Matrix4x4G.Equals(ref a, ref currentView))
                        {
                            currentView = a;
                            if (!Matrix4x4G.Equals(ref matrixxg2, ref currentProj))
                            {
                                currentProj = matrixxg2;
                                return 3;
                            }
                            return 1;
                        }
                        if (!Matrix4x4G.Equals(ref matrixxg2, ref currentProj))
                        {
                            currentProj = matrixxg2;
                            return 2;
                        }
                    }
                }
                num3 = 0;
            }
            finally
            {
                this.lastView = currentView;
                this.lastProj = currentProj;
            }
            return num3;
        }

        public static float timeSource
        {
            get
            {
                return Time.time;
            }
        }
        public void Set(float duration, TransitionFunction func)
        {
            this.start = timeSource;
            this.lastTime = this.start;
            this.end = this.start + duration;
            this.view = this.lastView;
            this.proj = this.lastProj;
            this.func = func;
        }
    }

    private static class PLATFORM_POLL
    {
        public static readonly bool flipRequired;

        static PLATFORM_POLL()
        {
            string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
            if (graphicsDeviceVersion != null)
            {
                if (graphicsDeviceVersion.StartsWith("OpenGL", StringComparison.InvariantCultureIgnoreCase))
                {
                    flipRequired = false;
                    return;
                }
                if (graphicsDeviceVersion.StartsWith("Direct3D", StringComparison.InvariantCultureIgnoreCase))
                {
                    flipRequired = true;
                    return;
                }
            }
            RuntimePlatform platform = Application.platform;
            switch (platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.WindowsEditor:
                    break;

                default:
                    switch (platform)
                    {
                        case RuntimePlatform.MetroPlayerX86:
                        case RuntimePlatform.MetroPlayerX64:
                        case RuntimePlatform.MetroPlayerARM:
                        case RuntimePlatform.XBOX360:
                            break;

                        default:
                            flipRequired = false;
                            return;
                    }
                    break;
            }
            flipRequired = true;
        }
    }
}

