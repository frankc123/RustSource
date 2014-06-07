using System;
using UnityEngine;

public class HUDDirectionalDamage : HUDIndicator
{
    [NonSerialized]
    public double damageAmount;
    [NonSerialized]
    public double damageTime;
    [NonSerialized]
    public double duration;
    private bool inverseX;
    private bool inverseY;
    private bool inverseZ;
    private Vector4 lastBoundMax;
    private Vector4 lastBoundMin;
    private const string materialProp_MAX = "_MaxChannels";
    private const string materialProp_MIN = "_MinChannels";
    [SerializeField]
    private UIPanel panel;
    private UIPanelMaterialPropertyBlock propBlock = new UIPanelMaterialPropertyBlock();
    private Vector4 randMax;
    private Vector4 randMin;
    [SerializeField]
    private Material skeletonMaterial;
    private double speedModW;
    private double speedModX;
    private double speedModY;
    private double speedModZ;
    private bool swapX;
    private bool swapY;
    private bool swapZ;
    [SerializeField]
    private UITexture texture;
    [NonSerialized]
    public Vector3 worldDirection = Vector3.left;

    private void Awake()
    {
        this.lastBoundMin = this.skeletonMaterial.GetVector("_MinChannels");
        this.lastBoundMax = this.skeletonMaterial.GetVector("_MaxChannels");
    }

    protected sealed override bool Continue()
    {
        Vector4 vector;
        Vector4 vector2;
        float x;
        if (this.duration <= 0.0)
        {
            return false;
        }
        double num = HUDIndicator.stepTime - this.damageTime;
        if (num > this.duration)
        {
            return false;
        }
        this.propBlock.Clear();
        double num2 = num / this.duration;
        double t = num2 * this.speedModX;
        double num4 = num2 * this.speedModY;
        double num5 = num2 * this.speedModZ;
        if (t > 1.0)
        {
            t = 1.0 - (t - 1.0);
        }
        if (num4 > 1.0)
        {
            num4 = 1.0 - (num4 - 1.0);
        }
        if (num5 > 1.0)
        {
            num5 = 1.0 - (num5 - 1.0);
        }
        double num6 = TransitionFunctions.Spline(t, (double) 1.0, (double) 0.0);
        double num7 = (num4 >= 0.10000000149011612) ? TransitionFunctions.Spline((double) ((num4 - 0.1) / 0.9), (double) 1.0, (double) 0.0) : TransitionFunctions.Spline((double) (num4 / 0.1), (double) 0.0, (double) 1.0);
        double num8 = Math.Cos(num5 * 1.5707963267948966);
        vector.x = (float) num8;
        vector.y = (float) num6;
        vector.z = (float) num7;
        vector.w = (float) num2;
        vector2.x = (float) (num8 / this.speedModX);
        vector2.y = (float) (num6 / this.speedModY);
        vector2.z = (float) (num7 / this.speedModZ);
        vector2.w = (float) (1.0 - num2);
        if (this.inverseX)
        {
            vector2.x = 1f - vector2.x;
        }
        if (this.inverseY)
        {
            vector2.y = 1f - vector2.y;
        }
        if (this.inverseZ)
        {
            vector2.z = 1f - vector2.z;
        }
        if (this.swapX)
        {
            x = vector2.x;
            vector2.x = vector.x;
            vector.x = x;
        }
        if (this.swapY)
        {
            x = vector2.y;
            vector2.y = vector.y;
            vector.y = x;
        }
        if (this.swapZ)
        {
            x = vector2.z;
            vector2.z = vector.z;
            vector.z = x;
        }
        if (vector != this.lastBoundMin)
        {
            this.lastBoundMin = vector;
            this.propBlock.Set("_MinChannels", this.lastBoundMin);
        }
        if (vector2 != this.lastBoundMax)
        {
            this.lastBoundMax = vector2;
            this.propBlock.Set("_MaxChannels", this.lastBoundMax);
        }
        Vector3 vector3 = HUDIndicator.worldToCameraLocalMatrix.MultiplyVector(this.worldDirection);
        vector3.Normalize();
        if ((vector3.y * vector3.y) <= 0.99f)
        {
            base.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(vector3.z, vector3.x) * 57.29578f);
        }
        this.panel.propertyBlock = this.propBlock;
        return true;
    }

    public static void CreateIndicator(Vector3 worldDamageDirection, double damageAmount, double timestamp, double duration, HUDDirectionalDamage prefab)
    {
        HUDDirectionalDamage damage = (HUDDirectionalDamage) HUDIndicator.InstantiateIndicator(HUDIndicator.ScratchTarget.CenteredFixed3000Tall, prefab, HUDIndicator.PlacementSpace.DoNotModify, Vector3.zero);
        damage.damageTime = timestamp;
        damage.duration = duration;
        damage.damageAmount = damageAmount;
        damage.worldDirection = -worldDamageDirection;
        damage.transform.localPosition = Vector3.zero;
        damage.transform.localRotation = Quaternion.identity;
        damage.transform.localScale = Vector3.one;
        damage.texture.ForceReloadMaterial();
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
    }

    protected void Start()
    {
        this.randMin.x = Random.Range((float) 0f, (float) 0.06f);
        this.randMin.y = Random.Range((float) 0f, (float) 0.06f);
        this.randMin.z = Random.Range((float) 0f, (float) 0.06f);
        this.randMin.w = Random.Range((float) 0f, (float) 0.06f);
        this.randMax.x = Random.Range((float) 0.94f, (float) 1f);
        this.randMax.y = Random.Range((float) 0.94f, (float) 1f);
        this.randMax.z = Random.Range((float) 0.94f, (float) 1f);
        this.randMax.w = Random.Range((float) 0.94f, (float) 1f);
        int num = Random.Range(0, 0x40);
        this.swapX = (num & 1) == 1;
        this.inverseX = (num & 8) == 8;
        this.swapY = (num & 2) == 2;
        this.inverseY = (num & 0x10) == 0x10;
        this.swapZ = (num & 4) == 4;
        this.inverseZ = (num & 0x20) == 0x20;
        this.speedModX = 1.12 - (1.0 - (this.randMax.x - this.randMin.x));
        this.speedModY = 1.12 - (1.0 - (this.randMax.y - this.randMin.y));
        this.speedModZ = 1.12 - (1.0 - (this.randMax.z - this.randMin.z));
        this.speedModW = 1.12 - (1.0 - (this.randMax.w - this.randMin.w));
        this.speedModX /= this.speedModW;
        this.speedModY /= this.speedModW;
        this.speedModZ /= this.speedModW;
        this.speedModW = 1.0;
        base.Start();
    }
}

