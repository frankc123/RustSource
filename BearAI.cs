using System;
using UnityEngine;

public class BearAI : HostileWildlifeAI
{
    public override string GetAttackAnim()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                return "4LegsClawsAttackL";

            case 1:
                return "4LegsClawsAttackR";
        }
        return "4LegsBiteAttack";
    }

    public override string GetDeathAnim()
    {
        return "4LegsDeath";
    }

    protected void Update()
    {
        if (!base._takeDamage.dead)
        {
            string animation = "idle4legs";
            float num = 1f;
            float moveSpeedForAnim = base.GetMoveSpeedForAnim();
            if (moveSpeedForAnim <= 0.001f)
            {
                animation = "idle4Legs";
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

