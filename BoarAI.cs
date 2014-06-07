using System;
using UnityEngine;

public class BoarAI : BasicWildLifeAI
{
    protected string lastMoveAnim;

    protected void Update()
    {
        if (!base._takeDamage.dead)
        {
            string animation = "idle1";
            float num = 1f;
            float moveSpeedForAnim = base.GetMoveSpeedForAnim();
            if (moveSpeedForAnim <= 0.001f)
            {
                animation = "idle1";
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
            if (animation != this.lastMoveAnim)
            {
                base.animation.CrossFade(animation, 0.25f, PlayMode.StopSameLayer);
            }
            base.animation[animation].speed = num;
            this.lastMoveAnim = animation;
        }
    }
}

