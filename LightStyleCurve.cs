using System;
using UnityEngine;

public class LightStyleCurve : LightStyle
{
    [SerializeField]
    private AnimationCurve curve;

    protected override LightStyle.Simulation ConstructSimulation(LightStylist stylist)
    {
        return new Simulation(this);
    }

    protected override bool DeconstructSimulation(LightStyle.Simulation simulation)
    {
        return true;
    }

    private float GetCurveValue(double relativeStartTime)
    {
        return this.curve.Evaluate((float) relativeStartTime);
    }

    protected class Simulation : LightStyle.Simulation<LightStyleCurve>
    {
        private float? lastValue;

        public Simulation(LightStyleCurve creator) : base(creator)
        {
            this.lastValue = null;
        }

        protected override void Simulate(double currentTime)
        {
            float curveValue = base.creator.GetCurveValue(currentTime - base.startTime);
            if (!this.lastValue.HasValue || (this.lastValue.Value != curveValue))
            {
                this.lastValue = new float?(curveValue);
                for (LightStyle.Mod.Element element = LightStyle.Mod.Element.Red; element < (LightStyle.Mod.Element.SpotAngle | LightStyle.Mod.Element.Green); element += 1)
                {
                    this.mod[element] = this.lastValue;
                }
            }
        }
    }
}

