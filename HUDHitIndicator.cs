using System;
using UnityEngine;

public class HUDHitIndicator : HUDIndicator
{
    public AnimationCurve curve;
    private bool followPoint;
    private const float kMAX = 0.9960784f;
    private const float kMIN = 0.003921569f;
    private UIMaterial material;
    private double startTime;
    public UITexture texture;
    private Vector3 worldPosition;

    private void Awake()
    {
        this.startTime = NetCull.time;
        this.material = this.texture.material.Clone();
        this.texture.material = this.material;
    }

    protected override bool Continue()
    {
        float time = (float) (HUDIndicator.stepTime - this.startTime);
        Keyframe keyframe = this.curve[this.curve.length - 1];
        if (time > keyframe.time)
        {
            return false;
        }
        this.material.Set("_AlphaValue", Mathf.Clamp(this.curve.Evaluate(time), 0.003921569f, 0.9960784f));
        if (this.followPoint)
        {
            Vector3 position = base.transform.position;
            Vector3 point = base.GetPoint(HUDIndicator.PlacementSpace.World, this.worldPosition);
            if (position.z != point.z)
            {
                float num2;
                Plane plane = new Plane(-base.transform.forward, position);
                Ray ray = new Ray(point, Vector3.forward);
                if (plane.Raycast(ray, out num2))
                {
                    point = ray.GetPoint(num2);
                }
                else
                {
                    ray.direction = -ray.direction;
                    if (plane.Raycast(ray, out num2))
                    {
                        point = ray.GetPoint(num2);
                    }
                    else
                    {
                        point = position;
                    }
                }
            }
            if (point != position)
            {
                base.transform.position = point;
            }
        }
        return true;
    }

    public static void CreateIndicator(Vector3 worldPoint, bool followPoint, HUDHitIndicator prefab)
    {
        HUDHitIndicator indicator = (HUDHitIndicator) HUDIndicator.InstantiateIndicator(HUDIndicator.ScratchTarget.CenteredAuto, prefab, HUDIndicator.PlacementSpace.World, worldPoint);
        indicator.worldPosition = worldPoint;
        indicator.followPoint = followPoint;
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        if (this.material != null)
        {
            Object.Destroy(this.material);
            this.material = null;
        }
    }
}

