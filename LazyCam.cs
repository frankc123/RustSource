using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LazyCam : MonoBehaviour, ICameraFX
{
    private bool _allow;
    [NonSerialized]
    private Matrix4x4 _cam2world;
    [NonSerialized]
    private Matrix4x4 _world2cam;
    private Quaternion add;
    private Quaternion aim;
    [NonSerialized]
    private Camera camera;
    public float damp = 0.01f;
    public float disableSeconds = 0.1f;
    private float enableFraction;
    public float enableSeconds = 0.1f;
    private bool hasViewModel;
    [NonSerialized]
    private bool isActivelyLazy;
    public float maxAngle = 10f;
    private Quaternion sub;
    public float targetAngle = 10f;
    [NonSerialized]
    private Transform transform;
    [NonSerialized]
    private float vel;
    private Quaternion view;
    private ViewModel viewModel;
    [NonSerialized]
    private bool wasActivelyLazy;

    private void Awake()
    {
        this.transform = base.transform;
        this.camera = base.camera;
        if (this.camera == null)
        {
            Debug.LogError("No camera detected");
        }
    }

    void ICameraFX.OnViewModelChange(ViewModel viewModel)
    {
        if (this.hasViewModel && (this.viewModel != null))
        {
            this.viewModel.lazyCam = null;
        }
        this.viewModel = viewModel;
        this.hasViewModel = (bool) this.viewModel;
        if (this.hasViewModel)
        {
            this.viewModel.lazyCam = this;
            this.targetAngle = this.viewModel.lazyAngle;
            this.allow = true;
        }
        else
        {
            this.allow = false;
        }
    }

    void ICameraFX.PostRender()
    {
        if (this.wasActivelyLazy = this.isActivelyLazy)
        {
            this.isActivelyLazy = false;
            this.transform.rotation *= this.sub;
        }
    }

    void ICameraFX.PreCull()
    {
        this.aim = this.transform.rotation;
        this.add = this.sub = Quaternion.identity;
        if (this._allow)
        {
            this.enableFraction += Time.deltaTime / this.enableSeconds;
            if (this.enableFraction >= 1f)
            {
                this.enableFraction = 1f;
            }
        }
        else
        {
            this.enableFraction -= Time.deltaTime / this.disableSeconds;
            if (this.enableFraction <= 0f)
            {
                this.enableFraction = 0f;
            }
        }
        this.maxAngle = Mathf.SmoothDampAngle(this.maxAngle, this.targetAngle * this.enableFraction, ref this.vel, this.damp);
        if (Mathf.Approximately(this.maxAngle, 0f))
        {
            this.view = this.aim;
            if (!this._allow)
            {
                base.enabled = false;
            }
            if (this.hasViewModel)
            {
                this.viewModel.lazyRotation = Quaternion.identity;
            }
        }
        else
        {
            this.isActivelyLazy = true;
            float num = Quaternion.Angle(this.aim, this.view);
            if (num >= this.maxAngle)
            {
                float t = 1f - (this.maxAngle / num);
                this.view = Quaternion.Slerp(this.view, this.aim, t);
            }
            this.sub = Quaternion.Inverse(this.add = Quaternion.Inverse(this.aim) * this.view);
            this.transform.rotation = this.view;
            this._world2cam = this.camera.worldToCameraMatrix;
            this._cam2world = this.camera.cameraToWorldMatrix;
            if (this.hasViewModel)
            {
                this.viewModel.lazyRotation = this.sub;
            }
        }
    }

    private void Start()
    {
        this.view = this.transform.rotation;
    }

    public bool allow
    {
        get
        {
            return this._allow;
        }
        set
        {
            if (value)
            {
                base.enabled = true;
                this._allow = true;
            }
            else
            {
                this._allow = false;
            }
        }
    }

    public Matrix4x4 cameraToWorldMatrix
    {
        get
        {
            return (!this.wasActivelyLazy ? this.camera.cameraToWorldMatrix : this._cam2world);
        }
    }

    public Matrix4x4 worldToCameraMatrix
    {
        get
        {
            return (!this.wasActivelyLazy ? this.camera.worldToCameraMatrix : this._world2cam);
        }
    }
}

