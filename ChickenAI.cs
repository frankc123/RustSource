using System;
using uLink;
using UnityEngine;

public class ChickenAI : BasicWildLifeAI
{
    [SerializeField]
    protected Material ChickenMatA;
    [SerializeField]
    protected Material ChickenMatB;
    [SerializeField]
    protected Renderer chickenRenderer;
    protected bool isMale;
    protected string lastMoveAnim;
    [SerializeField]
    protected Material roosterMat;

    protected void SetGender(bool male, bool alt)
    {
        this.isMale = male;
        if (this.isMale)
        {
            this.chickenRenderer.material = this.roosterMat;
        }
        else if (!alt)
        {
            this.chickenRenderer.material = this.ChickenMatA;
        }
        else
        {
            this.chickenRenderer.material = this.ChickenMatB;
        }
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        int id = info.networkView.viewID.id;
        this.SetGender(((id & 14) >> 1) <= 2, (id & 1) == 1);
        base.uLink_OnNetworkInstantiate(info);
    }

    protected void Update()
    {
        if (!base._takeDamage.dead)
        {
            string animation = "idleEat";
            float num = 1f;
            float moveSpeedForAnim = base.GetMoveSpeedForAnim();
            if (moveSpeedForAnim <= 0.001f)
            {
                animation = "idleEat";
            }
            else if (moveSpeedForAnim <= 2f)
            {
                animation = "walk";
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

