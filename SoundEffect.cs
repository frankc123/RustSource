using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SoundEffect : ScriptableObject
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Levels
    {
        public float volume;
        public float pitch;
        public float pan;
        public float doppler;
        public float spread;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MinMax
    {
        public float min;
        public float max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Parameters
    {
        public AudioClip clip;
        public SoundEffect.Parent parent;
        public SoundEffect.Levels levels;
        public SoundEffect.Rolloff rolloff;
        public int priority;
        public bool bypassEffects;
        public bool bypassListenerVolume;
        public Vector3 positionalValue;
        public Quaternion rotationalValue;
        public Vector3 position
        {
            get
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainLocal)
                {
                    return this.parent.transform.TransformPoint(this.positionalValue);
                }
                return this.positionalValue;
            }
            set
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainLocal)
                {
                    this.positionalValue = this.parent.transform.InverseTransformPoint(value);
                }
                else
                {
                    this.positionalValue = value;
                }
            }
        }
        public Vector3 localPosition
        {
            get
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode != SoundEffect.ParentMode.RetainWorld)
                {
                    return this.positionalValue;
                }
                return this.parent.transform.InverseTransformPoint(this.positionalValue);
            }
            set
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainWorld)
                {
                    this.positionalValue = this.parent.transform.TransformPoint(value);
                }
                else
                {
                    this.positionalValue = value;
                }
            }
        }
        private static Quaternion TransformQuaternion(Transform transform, Quaternion rotation)
        {
            return (transform.rotation * rotation);
        }

        private static Quaternion InverseTransformQuaternion(Transform transform, Quaternion rotation)
        {
            return (rotation * Quaternion.Inverse(transform.rotation));
        }

        public Quaternion rotation
        {
            get
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainLocal)
                {
                    return TransformQuaternion(this.parent.transform, this.rotationalValue);
                }
                return this.rotationalValue;
            }
            set
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainLocal)
                {
                    this.rotationalValue = InverseTransformQuaternion(this.parent.transform, value);
                }
                else
                {
                    this.rotationalValue = value;
                }
            }
        }
        public Quaternion localRotation
        {
            get
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode != SoundEffect.ParentMode.RetainWorld)
                {
                    return this.rotationalValue;
                }
                return InverseTransformQuaternion(this.parent.transform, this.rotationalValue);
            }
            set
            {
                SoundEffect.ParentMode mode = this.parent.mode & SoundEffect.ParentMode.RetainWorld;
                if (mode == SoundEffect.ParentMode.RetainWorld)
                {
                    this.rotationalValue = TransformQuaternion(this.parent.transform, value);
                }
                else
                {
                    this.rotationalValue = value;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Parent
    {
        public Transform transform;
        public SoundEffect.ParentMode mode;
    }

    public enum ParentMode
    {
        CameraLocally = 9,
        CameraWorld = 10,
        None = 0,
        RetainLocal = 1,
        RetainWorld = 3,
        StartLocally = 5,
        StartWorld = 6
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rolloff
    {
        public const float kCutoffVolume = 0.001f;
        public SoundEffect.MinMax distance;
        public float? manualCutoffDistance;
        public bool logarithmic;
    }
}

