using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class RecoilSimulation : IDLocalCharacter
{
    [NonSerialized]
    private GrabBag<Recoil> recoilImpulses;

    public void AddRecoil(float duration, Angle2 angle)
    {
        this.AddRecoil(duration, ref angle);
    }

    public void AddRecoil(float duration, float pitch)
    {
        Angle2 angle = new Angle2 {
            pitch = pitch
        };
        this.AddRecoil(duration, ref angle);
    }

    public void AddRecoil(float duration, ref Angle2 angle2)
    {
        if ((duration > 0f) && ((angle2.pitch != 0f) || (angle2.yaw != 0f)))
        {
            if (this.recoilImpulses == null)
            {
                this.recoilImpulses = new GrabBag<Recoil>(4);
                Debug.Log("Created GrabBag<Recoil>", this);
            }
            if (this.recoilImpulses.Add(new Recoil(ref angle2, duration)) == 0)
            {
                base.enabled = true;
            }
        }
    }

    public void AddRecoil(float duration, float pitch, float yaw)
    {
        Angle2 angle = new Angle2 {
            pitch = pitch,
            yaw = yaw
        };
        this.AddRecoil(duration, ref angle);
    }

    private bool ExtractRecoil(out Angle2 offset)
    {
        offset = new Angle2();
        if (this.recoilImpulses != null)
        {
            int count = this.recoilImpulses.Count;
            if (count > 0)
            {
                float deltaTime = Time.deltaTime;
                Recoil[] buffer = this.recoilImpulses.Buffer;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (buffer[i].Extract(ref offset, deltaTime))
                    {
                        this.recoilImpulses.RemoveAt(i);
                        while (--i >= 0)
                        {
                            if (buffer[i].Extract(ref offset, deltaTime))
                            {
                                this.recoilImpulses.RemoveAt(i);
                            }
                        }
                        if (this.recoilImpulses.Count == 0)
                        {
                            base.enabled = false;
                        }
                    }
                }
                return ((offset.pitch != 0f) || !(offset.yaw == 0f));
            }
        }
        return false;
    }

    private void LateUpdate()
    {
        Angle2 angle;
        if (this.ExtractRecoil(out angle))
        {
            base.ApplyAdditiveEyeAngles(angle);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Recoil
    {
        public Angle2 angle;
        public float fraction;
        public float timeScale;
        public Recoil(ref Angle2 angle, float duration)
        {
            this.angle = angle;
            this.timeScale = 1f / duration;
            this.fraction = 0f;
        }

        public bool Extract(ref Angle2 sum, float deltaTime)
        {
            float num = this.fraction + (this.fraction - (this.fraction * this.fraction));
            this.fraction += deltaTime * this.timeScale;
            if (this.fraction >= 1f)
            {
                num = 1f - num;
                sum.pitch += this.angle.pitch * num;
                sum.yaw += this.angle.yaw * num;
                return true;
            }
            num = (this.fraction + (this.fraction - (this.fraction * this.fraction))) - num;
            sum.pitch += this.angle.pitch * num;
            sum.yaw += this.angle.yaw * num;
            return false;
        }
    }
}

