using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class MountedCamera : MonoBehaviour
{
    private CameraFX _cameraFX;
    public Camera camera;
    private Matrix4x4 lastProj;
    private Matrix4x4 lastView;
    private CameraMount mount;
    private static readonly Matrix4x4 negateZMatrix = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
    private bool once;
    private static MountedCamera singleton;
    private float transitionEnd;
    private TransitionFunction transitionFunc;
    private Matrix4x4 transitionProj;
    private float transitionStart;
    private Matrix4x4 transitionView;

    private void Awake()
    {
        this.camera = base.camera;
        singleton = this;
        CameraFXPre.mountedCamera = this;
        CameraFXPost.mountedCamera = this;
    }

    public static bool GetPoint(out Vector3 point)
    {
        if (((singleton != null) && (singleton.camera != null)) && singleton.camera.enabled)
        {
            point = singleton.camera.worldToCameraMatrix.MultiplyPoint(Vector3.zero);
            return true;
        }
        point = new Vector3();
        return false;
    }

    public static bool IsCameraBeingUsed(Camera camera)
    {
        return (((camera != null) && (singleton != null)) && (((singleton.camera != null) && singleton.camera.enabled) && ((camera == singleton.camera) || ((singleton.mount != null) && (singleton.mount.camera == camera)))));
    }

    public static bool IsMountedCamera(Camera camera)
    {
        return ((singleton != null) && ((singleton.camera == camera) || ((singleton.mount != null) && (singleton.mount.camera == camera))));
    }

    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    public void PreCullBegin()
    {
        CameraMount current = CameraMount.current;
        if (current != this.mount)
        {
            if (current != null)
            {
                this._cameraFX = current.cameraFX;
            }
            else
            {
                this._cameraFX = null;
            }
            CameraFXPre.cameraFX = this._cameraFX;
            CameraFXPost.cameraFX = this._cameraFX;
            this.mount = current;
        }
        if (this.mount != null)
        {
            Camera camera = this.mount.camera;
            camera.ResetAspect();
            camera.ResetProjectionMatrix();
            camera.ResetWorldToCameraMatrix();
            this.mount.OnPreMount(this);
        }
    }

    public void PreCullEnd(bool postCamFX)
    {
        if (this.mount != null)
        {
            Transform transform = this.mount.transform;
            base.transform.position = transform.position;
            base.transform.rotation = transform.rotation;
            CameraClearFlags clearFlags = this.camera.clearFlags;
            int cullingMask = this.camera.cullingMask;
            DepthTextureMode depthTextureMode = this.camera.depthTextureMode;
            this.camera.ResetProjectionMatrix();
            this.camera.ResetWorldToCameraMatrix();
            this.mount.camera.depthTextureMode = depthTextureMode;
            this.camera.CopyFrom(this.mount.camera);
            if (!postCamFX)
            {
                CameraFX.ApplyTransitionAlterations(this.camera, null, false);
            }
            this.camera.clearFlags = clearFlags;
            this.camera.cullingMask = cullingMask;
            if (this.camera.depthTextureMode != depthTextureMode)
            {
                Debug.Log("Yea this is changing depth texture mode!", this.mount);
                this.camera.depthTextureMode = depthTextureMode;
            }
            this.mount.OnPostMount(this);
            this.lastView = this.camera.worldToCameraMatrix;
            this.lastProj = this.camera.projectionMatrix;
            this.once = true;
        }
        else
        {
            if (!this.once)
            {
                this.lastView = this.camera.worldToCameraMatrix;
                this.lastProj = this.camera.projectionMatrix;
                this.once = true;
            }
            this.camera.ResetProjectionMatrix();
            this.camera.ResetWorldToCameraMatrix();
            this.camera.worldToCameraMatrix = this.lastView;
            this.camera.projectionMatrix = this.lastProj;
            if (!postCamFX)
            {
                CameraFX.ApplyTransitionAlterations(this.camera, null, false);
            }
        }
        Matrix4x4 cameraToWorldMatrix = this.camera.cameraToWorldMatrix;
        base.transform.position = cameraToWorldMatrix.MultiplyPoint(Vector3.zero);
        Vector3 forward = cameraToWorldMatrix.MultiplyVector(-Vector3.forward);
        base.transform.rotation = Quaternion.LookRotation(forward, cameraToWorldMatrix.MultiplyVector(Vector3.up));
        Shader.SetGlobalMatrix("_RUST_MATRIX_CAMERA_TO_WORLD", cameraToWorldMatrix * negateZMatrix);
        Shader.SetGlobalMatrix("_RUST_MATRIX_WORLD_TO_CAMERA", this.camera.worldToCameraMatrix * negateZMatrix);
    }

    public CameraFX cameraFX
    {
        get
        {
            return this._cameraFX;
        }
    }

    public static MountedCamera main
    {
        get
        {
            return singleton;
        }
    }
}

