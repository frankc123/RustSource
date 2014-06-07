using Facepunch.Procedural;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public class HostileWildlifeAI : BasicWildLifeAI
{
    public Character _myChar;
    protected TakeDamage _targetTD;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map8;
    public float attackDamageMax;
    public float attackDamageMin;
    public float attackRange = 1f;
    public float attackRangeMax = 3f;
    public float attackRate;
    [SerializeField]
    protected AudioClipArray attackSounds;
    protected MillisClock attackStrikeClock;
    protected MillisClock chaseSoundClock;
    [SerializeField]
    protected AudioClipArray chaseSoundsClose;
    [SerializeField]
    protected AudioClipArray chaseSoundsFar;
    public string dropOnDeathString;
    public string lastMoveAnim;
    public float loseTargetRange = 100f;
    protected MillisClock nextAttackClock;
    public float nextScentListenTime;
    protected MillisClock nextTargetClock;
    protected MillisClock stuckClock;
    protected MillisClock targetReachClock;
    protected MillisClock warnClock;
    protected bool wasStuck;

    [RPC]
    public void CL_Attack(NetworkMessageInfo info)
    {
        InterpTimedEvent.Queue(this, "ATK", ref info);
    }

    public void DoClientAttack()
    {
        base.animation.CrossFade(this.GetAttackAnim(), 0.1f, PlayMode.StopSameLayer);
    }

    public virtual string GetAttackAnim()
    {
        return "bite";
    }

    public void GoScentBlind(float dur)
    {
        this.nextScentListenTime = Time.time + dur;
    }

    public bool IsScentBlind()
    {
        return (Time.time < this.nextScentListenTime);
    }

    protected override bool OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$map8 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                dictionary.Add("ATK", 0);
                <>f__switch$map8 = dictionary;
            }
            if (<>f__switch$map8.TryGetValue(tag, out num) && (num == 0))
            {
                this.DoClientAttack();
                return true;
            }
        }
        return base.OnInterpTimedEvent();
    }

    protected override bool PlaySnd(int type)
    {
        AudioClip clip = null;
        float volume = 1f;
        float minDistance = 5f;
        float maxDistance = 20f;
        bool flag = false;
        if (type == 5)
        {
            if (this.chaseSoundsFar != null)
            {
                clip = this.chaseSoundsFar[Random.Range(0, this.chaseSoundsFar.Length)];
            }
            volume = 1f;
            minDistance = 0.25f;
            maxDistance = 25f;
            flag = true;
        }
        else if (type == 6)
        {
            if (this.chaseSoundsClose != null)
            {
                clip = this.chaseSoundsClose[Random.Range(0, this.chaseSoundsClose.Length)];
            }
            volume = 1f;
            minDistance = 0f;
            maxDistance = 10f;
            flag = true;
        }
        else if (type == 2)
        {
            if (this.attackSounds != null)
            {
                clip = this.attackSounds[Random.Range(0, this.attackSounds.Length)];
            }
            volume = 1f;
            minDistance = 0f;
            maxDistance = 10f;
            flag = true;
        }
        if ((clip != null) && flag)
        {
            clip.PlayLocal(base.transform, Vector3.zero, volume, minDistance, maxDistance);
            return true;
        }
        return base.PlaySnd(type);
    }
}

