﻿using System;
using System.Reflection;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class CameraLayerDepths : MonoBehaviour
{
    [SerializeField]
    private float layer00;
    [NonSerialized]
    private float layer00_;
    [SerializeField]
    private float layer01;
    [NonSerialized]
    private float layer01_;
    [SerializeField]
    private float layer02;
    [NonSerialized]
    private float layer02_;
    [SerializeField]
    private float layer03;
    [NonSerialized]
    private float layer03_;
    [SerializeField]
    private float layer04;
    [NonSerialized]
    private float layer04_;
    [SerializeField]
    private float layer05;
    [NonSerialized]
    private float layer05_;
    [SerializeField]
    private float layer06;
    [NonSerialized]
    private float layer06_;
    [SerializeField]
    private float layer07;
    [NonSerialized]
    private float layer07_;
    [SerializeField]
    private float layer08;
    [NonSerialized]
    private float layer08_;
    [SerializeField]
    private float layer09;
    [NonSerialized]
    private float layer09_;
    [SerializeField]
    private float layer10;
    [NonSerialized]
    private float layer10_;
    [SerializeField]
    private float layer11;
    [NonSerialized]
    private float layer11_;
    [SerializeField]
    private float layer12;
    [NonSerialized]
    private float layer12_;
    [SerializeField]
    private float layer13;
    [NonSerialized]
    private float layer13_;
    [SerializeField]
    private float layer14;
    [NonSerialized]
    private float layer14_;
    [SerializeField]
    private float layer15;
    [NonSerialized]
    private float layer15_;
    [SerializeField]
    private float layer16;
    [NonSerialized]
    private float layer16_;
    [SerializeField]
    private float layer17;
    [NonSerialized]
    private float layer17_;
    [SerializeField]
    private float layer18;
    [NonSerialized]
    private float layer18_;
    [SerializeField]
    private float layer19;
    [NonSerialized]
    private float layer19_;
    [SerializeField]
    private float layer20;
    [NonSerialized]
    private float layer20_;
    [SerializeField]
    private float layer21;
    [NonSerialized]
    private float layer21_;
    [SerializeField]
    private float layer22;
    [NonSerialized]
    private float layer22_;
    [SerializeField]
    private float layer23;
    [NonSerialized]
    private float layer23_;
    [SerializeField]
    private float layer24;
    [NonSerialized]
    private float layer24_;
    [SerializeField]
    private float layer25;
    [NonSerialized]
    private float layer25_;
    [SerializeField]
    private float layer26;
    [NonSerialized]
    private float layer26_;
    [SerializeField]
    private float layer27;
    [NonSerialized]
    private float layer27_;
    [SerializeField]
    private float layer28;
    [NonSerialized]
    private float layer28_;
    [SerializeField]
    private float layer29;
    [NonSerialized]
    private float layer29_;
    [SerializeField]
    private float layer30;
    [NonSerialized]
    private float layer30_;
    [SerializeField]
    private float layer31;
    [NonSerialized]
    private float layer31_;
    [SerializeField]
    private bool spherical;
    [NonSerialized]
    private bool spherical_;

    private void Awake()
    {
        this.layer00_ = this.layer00;
        this.layer01_ = this.layer01;
        this.layer02_ = this.layer02;
        this.layer03_ = this.layer03;
        this.layer04_ = this.layer04;
        this.layer05_ = this.layer05;
        this.layer06_ = this.layer06;
        this.layer07_ = this.layer07;
        this.layer08_ = this.layer08;
        this.layer09_ = this.layer09;
        this.layer10_ = this.layer10;
        this.layer11_ = this.layer11;
        this.layer12_ = this.layer12;
        this.layer13_ = this.layer13;
        this.layer14_ = this.layer14;
        this.layer15_ = this.layer15;
        this.layer16_ = this.layer16;
        this.layer17_ = this.layer17;
        this.layer18_ = this.layer18;
        this.layer19_ = this.layer19;
        this.layer20_ = this.layer20;
        this.layer21_ = this.layer21;
        this.layer22_ = this.layer22;
        this.layer23_ = this.layer23;
        this.layer24_ = this.layer24;
        this.layer25_ = this.layer25;
        this.layer26_ = this.layer26;
        this.layer27_ = this.layer27;
        this.layer28_ = this.layer28;
        this.layer29_ = this.layer29;
        this.layer30_ = this.layer30;
        this.layer31_ = this.layer31;
        float[] numArray = new float[] { 
            this.layer00, this.layer01, this.layer02, this.layer03, this.layer04, this.layer05, this.layer06, this.layer07, this.layer08, this.layer09, this.layer10, this.layer11, this.layer12, this.layer13, this.layer14, this.layer15, 
            this.layer16, this.layer17, this.layer18, this.layer19, this.layer20, this.layer21, this.layer22, this.layer23, this.layer24, this.layer25, this.layer26, this.layer27, this.layer28, this.layer29, this.layer30, this.layer31
         };
        base.camera.layerCullDistances = numArray;
        base.camera.layerCullSpherical = this.spherical;
    }

    [ContextMenu("Ensure Layer Depths Set")]
    private void EnsureLayerDepthsSet()
    {
        float[] layerCullDistances = base.camera.layerCullDistances;
        if (layerCullDistances == null)
        {
            this.Awake();
        }
        else if (layerCullDistances.Length != 0x20)
        {
            this.Awake();
        }
        else
        {
            bool flag = false;
            for (int i = 0; i < 0x20; i++)
            {
                if (layerCullDistances[i] != this[i])
                {
                    flag = true;
                    this.Awake();
                    break;
                }
            }
            if (!flag)
            {
                return;
            }
        }
        if (this.spherical != base.camera.layerCullSpherical)
        {
            this.Awake();
        }
        else
        {
            return;
        }
        Debug.Log("Layer Depths Were Not Set", this);
    }

    private void OnPreCull()
    {
        if ((((((this.spherical != this.spherical_) || (this.layer00 != this.layer00_)) || ((this.layer01 != this.layer01_) || (this.layer02 != this.layer02_))) || (((this.layer03 != this.layer03_) || (this.layer04 != this.layer04_)) || ((this.layer05 != this.layer05_) || (this.layer06 != this.layer06_)))) || ((((this.layer07 != this.layer07_) || (this.layer08 != this.layer08_)) || ((this.layer09 != this.layer09_) || (this.layer10 != this.layer10_))) || (((this.layer11 != this.layer11_) || (this.layer12 != this.layer12_)) || ((this.layer13 != this.layer13_) || (this.layer14 != this.layer14_))))) || (((((this.layer15 != this.layer15_) || (this.layer16 != this.layer16_)) || ((this.layer17 != this.layer17_) || (this.layer18 != this.layer18_))) || (((this.layer19 != this.layer19_) || (this.layer20 != this.layer20_)) || ((this.layer21 != this.layer21_) || (this.layer22 != this.layer22_)))) || ((((this.layer23 != this.layer23_) || (this.layer24 != this.layer24_)) || ((this.layer25 != this.layer25_) || (this.layer26 != this.layer26_))) || (((this.layer27 != this.layer27_) || (this.layer28 != this.layer28_)) || (((this.layer29 != this.layer29_) || (this.layer30 != this.layer30_)) || (this.layer31 != this.layer31_))))))
        {
            this.Awake();
        }
    }

    private static bool Set(ref float m, float v)
    {
        if (m == v)
        {
            return false;
        }
        m = v;
        return true;
    }

    public float this[int layer]
    {
        get
        {
            switch (layer)
            {
                case 0:
                    return this.layer00;

                case 1:
                    return this.layer01;

                case 2:
                    return this.layer02;

                case 3:
                    return this.layer03;

                case 4:
                    return this.layer04;

                case 5:
                    return this.layer05;

                case 6:
                    return this.layer06;

                case 7:
                    return this.layer07;

                case 8:
                    return this.layer08;

                case 9:
                    return this.layer09;

                case 10:
                    return this.layer10;

                case 11:
                    return this.layer11;

                case 12:
                    return this.layer12;

                case 13:
                    return this.layer13;

                case 14:
                    return this.layer14;

                case 15:
                    return this.layer15;

                case 0x10:
                    return this.layer16;

                case 0x11:
                    return this.layer17;

                case 0x12:
                    return this.layer18;

                case 0x13:
                    return this.layer19;

                case 20:
                    return this.layer20;

                case 0x15:
                    return this.layer21;

                case 0x16:
                    return this.layer22;

                case 0x17:
                    return this.layer23;

                case 0x18:
                    return this.layer24;

                case 0x19:
                    return this.layer25;

                case 0x1a:
                    return this.layer26;

                case 0x1b:
                    return this.layer27;

                case 0x1c:
                    return this.layer28;

                case 0x1d:
                    return this.layer29;

                case 30:
                    return this.layer30;

                case 0x1f:
                    return this.layer31;
            }
            throw new ArgumentOutOfRangeException();
        }
        set
        {
            bool flag;
            switch (layer)
            {
                case 0:
                    flag = Set(ref this.layer00, value);
                    break;

                case 1:
                    flag = Set(ref this.layer01, value);
                    break;

                case 2:
                    flag = Set(ref this.layer02, value);
                    break;

                case 3:
                    flag = Set(ref this.layer03, value);
                    break;

                case 4:
                    flag = Set(ref this.layer04, value);
                    break;

                case 5:
                    flag = Set(ref this.layer05, value);
                    break;

                case 6:
                    flag = Set(ref this.layer06, value);
                    break;

                case 7:
                    flag = Set(ref this.layer07, value);
                    break;

                case 8:
                    flag = Set(ref this.layer08, value);
                    break;

                case 9:
                    flag = Set(ref this.layer09, value);
                    break;

                case 10:
                    flag = Set(ref this.layer10, value);
                    break;

                case 11:
                    flag = Set(ref this.layer11, value);
                    break;

                case 12:
                    flag = Set(ref this.layer12, value);
                    break;

                case 13:
                    flag = Set(ref this.layer13, value);
                    break;

                case 14:
                    flag = Set(ref this.layer14, value);
                    break;

                case 15:
                    flag = Set(ref this.layer15, value);
                    break;

                case 0x10:
                    flag = Set(ref this.layer16, value);
                    break;

                case 0x11:
                    flag = Set(ref this.layer17, value);
                    break;

                case 0x12:
                    flag = Set(ref this.layer18, value);
                    break;

                case 0x13:
                    flag = Set(ref this.layer19, value);
                    break;

                case 20:
                    flag = Set(ref this.layer20, value);
                    break;

                case 0x15:
                    flag = Set(ref this.layer21, value);
                    break;

                case 0x16:
                    flag = Set(ref this.layer22, value);
                    break;

                case 0x17:
                    flag = Set(ref this.layer23, value);
                    break;

                case 0x18:
                    flag = Set(ref this.layer24, value);
                    break;

                case 0x19:
                    flag = Set(ref this.layer25, value);
                    break;

                case 0x1a:
                    flag = Set(ref this.layer26, value);
                    break;

                case 0x1b:
                    flag = Set(ref this.layer27, value);
                    break;

                case 0x1c:
                    flag = Set(ref this.layer28, value);
                    break;

                case 0x1d:
                    flag = Set(ref this.layer29, value);
                    break;

                case 30:
                    flag = Set(ref this.layer30, value);
                    break;

                case 0x1f:
                    flag = Set(ref this.layer31, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (flag)
            {
                this.Awake();
            }
        }
    }
}

