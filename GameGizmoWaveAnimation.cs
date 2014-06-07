using System;
using UnityEngine;

public class GameGizmoWaveAnimation : GameGizmo
{
    [SerializeField]
    protected float amplitudeNegative = -0.1f;
    [SerializeField]
    protected float amplitudePositive = 0.15f;
    [SerializeField]
    protected Vector3 axis = Vector3.up;
    [SerializeField]
    protected float frequency = 1.256637f;
    [SerializeField]
    protected float phase;

    protected override GameGizmo.Instance ConstructInstance()
    {
        return new Instance(this);
    }

    public class Instance : GameGizmo.Instance
    {
        public double AmplitudeNegative;
        public double AmplitudePositive;
        public double arcSine;
        public Vector3 Axis;
        public double Frequency;
        private ulong lastRenderTime;
        public double Phase;
        public double sine;
        public bool Started;
        public double value;

        protected internal Instance(GameGizmoWaveAnimation gameGizmo) : base(gameGizmo)
        {
            this.Frequency = gameGizmo.frequency;
            this.Phase = gameGizmo.phase;
            this.AmplitudePositive = gameGizmo.amplitudePositive;
            this.AmplitudeNegative = gameGizmo.amplitudeNegative;
            this.Axis = gameGizmo.axis;
        }

        protected override void Render(bool useCamera, Camera camera)
        {
            double num2;
            double num3;
            double amplitudePositive;
            Vector3 vector;
            Matrix4x4? nullable2;
            ulong localTimeInMillis = NetCull.localTimeInMillis;
            if (!this.Started || (this.lastRenderTime >= localTimeInMillis))
            {
                this.Started = true;
                num2 = num3 = 0.0;
            }
            else
            {
                ulong num4 = localTimeInMillis - this.lastRenderTime;
                num2 = num4 * 0.001;
                num3 = 1000.0 / ((double) num4);
                this.Phase += this.Frequency * num2;
            }
            this.lastRenderTime = localTimeInMillis;
            if (this.Phase > 1.0)
            {
                if (double.IsPositiveInfinity(this.Phase))
                {
                    this.Phase = 0.0;
                }
                else
                {
                    this.Phase = this.Phase % 1.0;
                }
            }
            else if (this.Phase == 1.0)
            {
                this.Phase = 0.0;
            }
            else if (this.Phase < 0.0)
            {
                if (double.IsNegativeInfinity(this.Phase))
                {
                    this.Phase = 0.0;
                }
                else
                {
                    this.Phase = -this.Phase % 1.0;
                    if (this.Phase > 0.0)
                    {
                        this.Phase = 1.0 - this.Phase;
                    }
                }
            }
            if (this.Phase < 0.5)
            {
                this.arcSine = this.Phase * 6.2831853071795862;
                amplitudePositive = this.AmplitudePositive;
            }
            else
            {
                this.arcSine = ((this.Phase * 2.0) - 2.0) * 3.1415926535897931;
                amplitudePositive = this.AmplitudeNegative;
            }
            this.sine = Math.Sin(this.arcSine);
            this.value = this.sine * amplitudePositive;
            vector.x = (float) (this.Axis.x * this.value);
            vector.y = (float) (this.Axis.y * this.value);
            vector.z = (float) (this.Axis.z * this.value);
            Matrix4x4? ultimateMatrix = base.ultimateMatrix;
            base.ultimateMatrix = new Matrix4x4?((!ultimateMatrix.HasValue ? (!(nullable2 = base.overrideMatrix).HasValue ? base.DefaultMatrix() : nullable2.Value) : ultimateMatrix.Value) * Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one));
            base.Render(useCamera, camera);
            base.ultimateMatrix = ultimateMatrix;
        }
    }
}

