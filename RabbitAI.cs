using System;
using UnityEngine;

public class RabbitAI : BasicWildLifeAI
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
                animation = "hop";
                num = moveSpeedForAnim / 0.75f;
            }
            else if (moveSpeedForAnim > 2f)
            {
                animation = "run";
                num = moveSpeedForAnim / 3f;
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

