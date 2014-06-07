using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FitRequirements : ScriptableObject
{
    [HideInInspector, SerializeField]
    private string assetPreview;
    [SerializeField]
    private Condition[] Conditions;

    public bool Test(Matrix4x4 placePosition)
    {
        if (!object.ReferenceEquals(this.Conditions, null))
        {
            foreach (Condition condition in this.Conditions)
            {
                if (!condition.Check(ref placePosition))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool Test(Vector3 origin, Quaternion rotation)
    {
        return this.Test(Matrix4x4.TRS(origin, rotation, Vector3.one));
    }

    public bool Test(Vector3 origin, Quaternion rotation, Vector3 scale)
    {
        return this.Test(Matrix4x4.TRS(origin, rotation, scale));
    }

    [Serializable]
    public class Condition
    {
        [SerializeField]
        private bool failPass;
        [SerializeField]
        private Color flt0;
        [SerializeField]
        private Color flt1;
        [SerializeField]
        private Color flt2;
        [SerializeField]
        private FitRequirements.Instruction instruction;
        [SerializeField]
        private LayerMask mask;

        public Condition()
        {
            this.flt0.a = 1f;
            this.flt1 = (Color) Vector3.up;
            this.flt1.a = 0.5f;
            this.flt2 = (Color) Vector3.up;
            this.mask = 0x20000400;
        }

        public bool Check(ref Matrix4x4 matrix)
        {
            bool flag;
            Vector3 vector;
            float magnitude;
            switch (this.instruction)
            {
                case FitRequirements.Instruction.Raycast:
                    flag = Physics.Raycast(matrix.MultiplyPoint3x4(this.center), vector = matrix.MultiplyVector(this.direction), (float) (matrix.MultiplyVector(vector.normalized).magnitude * this.distance), (int) this.mask);
                    break;

                case FitRequirements.Instruction.SphereCast:
                {
                    Ray ray = new Ray(matrix.MultiplyPoint3x4(this.center), matrix.MultiplyVector(this.direction));
                    magnitude = matrix.MultiplyVector(ray.direction).magnitude;
                    flag = Physics.SphereCast(ray, magnitude * this.radius, magnitude * this.distance, (int) this.mask);
                    break;
                }
                case FitRequirements.Instruction.CapsuleCast:
                    vector = matrix.MultiplyVector(this.direction);
                    magnitude = matrix.MultiplyVector(vector.normalized).magnitude;
                    flag = Physics.CapsuleCast(matrix.MultiplyPoint3x4(this.capStart), matrix.MultiplyPoint3x4(this.capEnd), magnitude * this.radius, vector, magnitude * this.distance, (int) this.mask);
                    break;

                case FitRequirements.Instruction.CheckCapsule:
                    flag = Physics.CheckCapsule(matrix.MultiplyPoint3x4(this.capStart), matrix.MultiplyPoint3x4(this.capEnd), matrix.MultiplyVector(matrix.MultiplyVector(Vector3.one).normalized).magnitude * this.radius, (int) this.mask);
                    break;

                case FitRequirements.Instruction.CheckSphere:
                    flag = Physics.CheckSphere(matrix.MultiplyPoint3x4(this.center), matrix.MultiplyVector(matrix.MultiplyVector(Vector3.one).normalized).magnitude * this.radius, (int) this.mask);
                    break;

                default:
                    return true;
            }
            return (flag != this.passOnFail);
        }

        public void DrawGizmo(ref Matrix4x4 matrix)
        {
            switch (this.instruction)
            {
                case FitRequirements.Instruction.Raycast:
                {
                    Vector3 from = matrix.MultiplyPoint3x4(this.center);
                    Vector3 normalized = matrix.MultiplyVector(this.direction).normalized;
                    Gizmos.DrawLine(from, from + ((Vector3) (normalized * (matrix.MultiplyVector(normalized).magnitude * this.distance))));
                    break;
                }
                case FitRequirements.Instruction.SphereCast:
                    GizmoCapsuleAxis(ref matrix, this.center, this.radius, this.distance, this.direction, null, null);
                    break;

                case FitRequirements.Instruction.CapsuleCast:
                    GizmoCapsuleAxis(ref matrix, this.capStart, this.radius, this.distance, this.direction, null, null);
                    GizmoCapsuleAxis(ref matrix, this.capEnd, this.radius, this.distance, this.direction, null, null);
                    GizmoCapsulePoles(ref matrix, this.capStart, this.radius, this.capEnd);
                    break;

                case FitRequirements.Instruction.CheckCapsule:
                    GizmoCapsulePoles(ref matrix, this.capStart, this.radius, this.capEnd);
                    break;

                case FitRequirements.Instruction.CheckSphere:
                    Gizmos.DrawSphere(matrix.MultiplyPoint3x4(this.center), matrix.MultiplyVector(matrix.MultiplyVector(Vector3.one).normalized).magnitude * this.radius);
                    break;
            }
        }

        private static void GizmoCapsuleAxis(ref Matrix4x4 matrix, Vector3 start, float radius, float distance, Vector3 direction, float? unitValueRadius = new float?(), float? unitValueHeight = new float?())
        {
            Vector3 pos = matrix.MultiplyPoint3x4(start);
            Vector3 normalized = matrix.MultiplyVector(direction).normalized;
            float? nullable = null;
            float num = !unitValueRadius.HasValue ? (nullable = new float?(matrix.MultiplyVector(normalized).magnitude)).Value : unitValueRadius.Value;
            float num2 = !unitValueHeight.HasValue ? (!nullable.HasValue ? matrix.MultiplyVector(normalized).magnitude : nullable.Value) : unitValueHeight.Value;
            Matrix4x4 matrixx = Gizmos.matrix;
            Gizmos.matrix = matrixx * Matrix4x4.TRS(pos, Quaternion.LookRotation(normalized, matrix.MultiplyVector(Vector3.up)), Vector3.one);
            radius = num * radius;
            float height = num2 * (distance + (radius * 2f));
            Gizmos2.DrawWireCapsule(new Vector3(0f, 0f, (height * 0.5f) - radius), radius, height, 2);
            Gizmos.matrix = matrixx;
        }

        private static void GizmoCapsulePoles(ref Matrix4x4 matrix, Vector3 start, float radius, Vector3 end)
        {
            Vector3 vector2 = end - start;
            Vector3 normalized = vector2.normalized;
            start = matrix.MultiplyPoint3x4(start);
            end = matrix.MultiplyPoint3x4(end);
            float magnitude = matrix.MultiplyVector(normalized).magnitude;
            radius *= magnitude;
            Vector3 vector4 = end - start;
            normalized = vector4.normalized;
            start -= (Vector3) (normalized * radius);
            end += (Vector3) (normalized * radius);
            normalized = end - start;
            Matrix4x4 matrixx = Gizmos.matrix;
            Gizmos.matrix = matrixx * Matrix4x4.TRS(start, Quaternion.LookRotation(normalized, matrix.MultiplyVector(Vector3.up)), Vector3.one);
            Vector3 vector5 = end - start;
            float height = vector5.magnitude;
            Gizmos2.DrawWireCapsule(new Vector3(0f, 0f, height * 0.5f), radius, height, 2);
            Gizmos.matrix = matrixx;
        }

        public Vector3 capEnd
        {
            get
            {
                return (Vector3) this.flt2;
            }
            set
            {
                this.flt2.r = value.x;
                this.flt2.g = value.y;
                this.flt2.b = value.z;
            }
        }

        public Vector3 capStart
        {
            get
            {
                return (Vector3) this.flt0;
            }
            set
            {
                this.flt0.r = value.x;
                this.flt0.g = value.y;
                this.flt0.b = value.z;
            }
        }

        public Vector3 center
        {
            get
            {
                return (Vector3) this.flt0;
            }
            set
            {
                this.flt0.r = value.x;
                this.flt0.g = value.y;
                this.flt0.b = value.z;
            }
        }

        public Vector3 direction
        {
            get
            {
                return (Vector3) this.flt1;
            }
            set
            {
                this.flt1.r = value.x;
                this.flt1.g = value.y;
                this.flt1.b = value.z;
            }
        }

        public float distance
        {
            get
            {
                return this.flt0.a;
            }
            set
            {
                this.flt0.a = value;
            }
        }

        public bool passOnFail
        {
            get
            {
                return this.failPass;
            }
            set
            {
                this.failPass = value;
            }
        }

        public float radius
        {
            get
            {
                return this.flt1.a;
            }
            set
            {
                this.flt1.a = value;
            }
        }
    }

    public enum Instruction
    {
        Raycast,
        SphereCast,
        CapsuleCast,
        CheckCapsule,
        CheckSphere
    }
}

