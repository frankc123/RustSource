using System;
using UnityEngine;

public class LightStyleDefault : LightStyle
{
    private static LightStyleDefault singleton;
    private DefaultSimulation singletonSimulation;

    protected override LightStyle.Simulation ConstructSimulation(LightStylist stylist)
    {
        if (this.singletonSimulation == null)
        {
        }
        return (this.singletonSimulation = new DefaultSimulation(this));
    }

    protected override bool DeconstructSimulation(LightStyle.Simulation simulation)
    {
        return false;
    }

    private void OnDisable()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    private void OnEnable()
    {
        singleton = this;
    }

    public static LightStyleDefault Singleton
    {
        get
        {
            if (singleton != null)
            {
                return singleton;
            }
            return ScriptableObject.CreateInstance<LightStyleDefault>();
        }
    }

    private class DefaultSimulation : LightStyle.Simulation
    {
        public DefaultSimulation(LightStyleDefault def) : base(def)
        {
            float? nullable = 1f;
            for (LightStyle.Mod.Element element = LightStyle.Mod.Element.Red; element < (LightStyle.Mod.Element.SpotAngle | LightStyle.Mod.Element.Green); element += 1)
            {
                this.mod[element] = nullable;
            }
        }

        protected override void Simulate(double currentTime)
        {
        }
    }
}

