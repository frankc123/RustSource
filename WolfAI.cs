using System;
using UnityEngine;

public class WolfAI : HostileWildlifeAI
{
    public Material[] mats;
    public Renderer wolfRenderer;

    public override string GetAttackAnim()
    {
        return "bite";
    }

    public void Start()
    {
        this.wolfRenderer.material = this.mats[Random.Range(0, this.mats.Length)];
    }

    protected void Update()
    {
        if (!base._takeDamage.dead)
        {
            string animation = "idle";
            float num = 1f;
            float moveSpeedForAnim = base.GetMoveSpeedForAnim();
            if (moveSpeedForAnim <= 0.001f)
            {
                animation = "idle";
            }
            else if (moveSpeedForAnim <= 2f)
            {
                animation = "walk";
                num = moveSpeedForAnim / base.GetWalkAnimScalar();
            }
            else if (moveSpeedForAnim > 2f)
            {
                animation = "run";
                num = moveSpeedForAnim / base.GetRunAnimScalar();
            }
            if (animation != base.lastMoveAnim)
            {
                base.animation.CrossFade(animation, 0.25f, PlayMode.StopSameLayer);
            }
            base.animation[animation].speed = num;
            base.lastMoveAnim = animation;
        }
    }
}

