using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Explicit, Size=8)]
public struct Angle2
{
    private static readonly float[] eights360;
    private const float kEigth = 45f;
    private const double kF2I = 182.04444444444445;
    [FieldOffset(0)]
    public Vector2 m;
    [FieldOffset(0)]
    public float pitch;
    [FieldOffset(0)]
    public float x;
    [FieldOffset(4)]
    public float y;
    [FieldOffset(4)]
    public float yaw;
    public static readonly Angle2 zero;

    static Angle2()
    {
        zero = new Angle2();
        eights360 = new float[0x2000];
        for (long i = 0L; i < 0x2000L; i += 1L)
        {
            eights360[(int) ((IntPtr) i)] = (float) ((((double) i) / 65536.0) * 360.0);
        }
        uLinkAngle2Extensions.Register();
    }

    public Angle2(Angle2 angle)
    {
        this = angle;
    }

    public Angle2(Vector2 pitchYaw)
    {
        this = new Angle2();
        Angle2 angle = this;
        angle.m = pitchYaw;
        this = angle;
    }

    public Angle2(float pitch, float yaw)
    {
        this = new Angle2();
        Angle2 angle = this;
        angle.pitch = pitch;
        angle.yaw = yaw;
        this = angle;
    }

    public static float AngleDistance(Angle2 a, Angle2 b)
    {
        return Mathf.Sqrt(SquareAngleDistance(a, b));
    }

    public static float Decode360(int x)
    {
        int num = x / 0x2000;
        float num2 = (num * 45f) + eights360[x - (num * 0x2000)];
        return ((num2 >= 180f) ? (num2 - 360f) : num2);
    }

    public static Angle2 Delta(Angle2 a, Angle2 b)
    {
        return new Angle2(Mathf.DeltaAngle(a.x, b.x), Mathf.DeltaAngle(b.x, b.y));
    }

    public static Vector3 Direction(float pitch, float yaw)
    {
        return (Vector3) (Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward);
    }

    private static float DistAngle(float a, float b)
    {
        return Mathf.Abs(Mathf.DeltaAngle(a, b));
    }

    public static int Encode360(float x)
    {
        x = Mathf.DeltaAngle(0f, x);
        if (x < 0f)
        {
            x += 360f;
        }
        switch ((Mathf.FloorToInt(x) / 0x2d))
        {
            case 0:
                return Mathf.RoundToInt((float) (x * 182.04444444444445));

            case 1:
                return (Mathf.RoundToInt((float) ((x - 45f) * 182.04444444444445)) + 0x2000);

            case 2:
                return (Mathf.RoundToInt((float) ((x - 90f) * 182.04444444444445)) + 0x4000);

            case 3:
                return (Mathf.RoundToInt((float) ((x - 135f) * 182.04444444444445)) + 0x6000);

            case 4:
                return (Mathf.RoundToInt((float) ((x - 180f) * 182.04444444444445)) + 0x8000);

            case 5:
                return (Mathf.RoundToInt((float) ((x - 225f) * 182.04444444444445)) + 0xa000);

            case 6:
                return (Mathf.RoundToInt((float) ((x - 270f) * 182.04444444444445)) + 0xc000);

            case 7:
                return (Mathf.RoundToInt((float) ((x - 315f) * 182.04444444444445)) + 0xe000);

            case 8:
                return 0;
        }
        return -1;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is Angle2)
        {
            return (this == ((Angle2) obj));
        }
        if (obj is Vector2)
        {
            return (this == ((Vector2) obj));
        }
        if (obj is Quaternion)
        {
            return (this == ((Quaternion) obj));
        }
        if (obj is Vector3)
        {
            return (this == ((Vector3) obj));
        }
        return obj.Equals(this);
    }

    public override int GetHashCode()
    {
        return this.normalized.m.GetHashCode();
    }

    public static Angle2 Lerp(Angle2 a, Angle2 b, float t)
    {
        return new Angle2(Mathf.LerpAngle(a.x, b.x, t), Mathf.LerpAngle(a.y, b.y, t));
    }

    public static Angle2 Lerp(Angle2 a, Angle2 b, Vector2 t)
    {
        return new Angle2(Mathf.LerpAngle(a.x, b.x, t.x), Mathf.LerpAngle(a.y, b.y, t.y));
    }

    public static Angle2 LookDirection(Vector3 v)
    {
        return (Angle2) v;
    }

    public static Angle2 MoveTowards(Angle2 current, Angle2 target, float maxAngleMove)
    {
        if (current.x == target.x)
        {
            current.y = Mathf.MoveTowardsAngle(current.y, target.y, maxAngleMove);
            return current;
        }
        if (current.y == target.y)
        {
            current.x = Mathf.MoveTowardsAngle(current.x, target.x, maxAngleMove);
            return current;
        }
        Vector2 vector = (Vector2) (NormMags(current, target) * maxAngleMove);
        current.x = Mathf.MoveTowardsAngle(current.x, target.x, vector.x);
        current.y = Mathf.MoveTowardsAngle(current.y, target.y, vector.y);
        return current;
    }

    public static Angle2 MoveTowards(Angle2 current, Angle2 target, Vector2 maxAngleMove)
    {
        current.x = Mathf.MoveTowardsAngle(current.x, target.y, maxAngleMove.x);
        current.y = Mathf.MoveTowardsAngle(current.x, target.y, maxAngleMove.y);
        return current;
    }

    public static Angle2 Normalize(Angle2 a)
    {
        return new Angle2 { x = NormAngle(a.x), y = NormAngle(a.y) };
    }

    public static Angle2 NormalizeAdd(Angle2 a, Angle2 b)
    {
        return new Angle2 { x = NormAngle(a.x + b.x), y = NormAngle(a.y + b.y) };
    }

    public static Angle2 NormalizeSubtract(Angle2 a, Angle2 b)
    {
        return new Angle2 { x = NormAngle(a.x - b.x), y = NormAngle(a.y - b.y) };
    }

    private static float NormAngle(float a)
    {
        a = Mathf.DeltaAngle(0f, a);
        return ((a <= 180f) ? a : (a - 360f));
    }

    private static Vector2 NormMags(Angle2 a, Angle2 b)
    {
        Vector2 vector = new Vector2(DistAngle(a.x, b.x), DistAngle(a.y, b.y));
        vector.Normalize();
        return vector;
    }

    public static Angle2 operator +(Angle2 L, Angle2 R)
    {
        L.m += R.m;
        return L;
    }

    public static Angle2 operator +(Angle2 L, Vector2 R)
    {
        L.m += R;
        return L;
    }

    public static Vector2 operator +(Vector2 L, Angle2 R)
    {
        L += R.m;
        return L;
    }

    public static Angle2 operator /(Angle2 L, Angle2 R)
    {
        L.m -= Delta(L, R).m;
        return L;
    }

    public static Angle2 operator /(Angle2 L, float R)
    {
        L.m = (Vector2) (L.m / R);
        return L;
    }

    public static Angle2 operator /(Angle2 L, Vector2 R)
    {
        L.x /= R.x;
        L.y /= R.y;
        return L;
    }

    public static Angle2 operator /(float L, Angle2 R)
    {
        R.m = (Vector2) (R.m / L);
        return R;
    }

    public static bool operator ==(Angle2 L, Angle2 R)
    {
        return (Normalize(L - R).sqrAngleMagnitude == 0f);
    }

    public static bool operator ==(Angle2 L, Quaternion R)
    {
        return (L.quat == R);
    }

    public static bool operator ==(Angle2 L, Vector2 R)
    {
        return (L.m == R);
    }

    public static bool operator ==(Angle2 L, Vector3 R)
    {
        return (L.forward == R);
    }

    public static bool operator ==(Quaternion L, Angle2 R)
    {
        return (L == R.quat);
    }

    public static bool operator ==(Vector2 L, Angle2 R)
    {
        return (L == R.m);
    }

    public static bool operator ==(Vector3 L, Angle2 R)
    {
        return (L == R.forward);
    }

    public static explicit operator Quaternion(Angle2 a)
    {
        return a.quat;
    }

    public static explicit operator Vector3(Angle2 a)
    {
        return a.forward;
    }

    public static explicit operator Angle2(Quaternion quat)
    {
        return new Angle2 { quat = quat };
    }

    public static explicit operator Angle2(Vector3 forward)
    {
        return new Angle2 { forward = forward };
    }

    public static implicit operator Vector2(Angle2 a)
    {
        return a.m;
    }

    public static implicit operator Angle2(Vector2 yawPitch)
    {
        return new Angle2 { m = yawPitch };
    }

    public static bool operator !=(Angle2 L, Angle2 R)
    {
        return !(Normalize(L - R).sqrAngleMagnitude == 0f);
    }

    public static bool operator !=(Angle2 L, Quaternion R)
    {
        return (L.quat != R);
    }

    public static bool operator !=(Angle2 L, Vector2 R)
    {
        return (L.m != R);
    }

    public static bool operator !=(Angle2 L, Vector3 R)
    {
        return (L.forward != R);
    }

    public static bool operator !=(Quaternion L, Angle2 R)
    {
        return (L != R.quat);
    }

    public static bool operator !=(Vector2 L, Angle2 R)
    {
        return (L != R.m);
    }

    public static bool operator !=(Vector3 L, Angle2 R)
    {
        return (L != R.forward);
    }

    public static Angle2 operator *(Angle2 L, Angle2 R)
    {
        L.m += Delta(L, R).m;
        return L;
    }

    public static Angle2 operator *(Angle2 L, float R)
    {
        L.m = (Vector2) (L.m * R);
        return L;
    }

    public static Angle2 operator *(Angle2 L, Quaternion R)
    {
        L.quat *= R;
        return L;
    }

    public static Angle2 operator *(Angle2 L, Vector2 R)
    {
        L.m = Vector2.Scale(L.m, R);
        return L;
    }

    public static Vector3 operator *(Angle2 L, Vector3 R)
    {
        return (Vector3) (L.quat * R);
    }

    public static Angle2 operator *(float L, Angle2 R)
    {
        R.m = (Vector2) (R.m * L);
        return R;
    }

    public static Quaternion operator *(Quaternion L, Angle2 R)
    {
        return (L * R.quat);
    }

    public static Angle2 operator -(Angle2 L, Angle2 R)
    {
        L.m -= R.m;
        return L;
    }

    public static Angle2 operator -(Angle2 L, Vector2 R)
    {
        L.m -= R;
        return L;
    }

    public static Vector2 operator -(Vector2 L, Angle2 R)
    {
        L -= R.m;
        return L;
    }

    public static Angle2 operator -(Angle2 negate)
    {
        negate.m = -negate.m;
        return negate;
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, float damping)
    {
        return SmoothDamp(current, target, ref velocity, damping, float.PositiveInfinity, Time.deltaTime);
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, Vector2 damping)
    {
        return SmoothDamp(current, target, ref velocity, damping, new Vector2(float.PositiveInfinity, float.PositiveInfinity), Time.deltaTime);
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, float damping, float maxAngleMove)
    {
        return SmoothDamp(current, target, ref velocity, damping, maxAngleMove, Time.deltaTime);
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, float damping, Vector2 maxAngleMove)
    {
        return SmoothDamp(current, target, ref velocity, damping, maxAngleMove, Time.deltaTime);
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, Vector2 damping, Vector2 maxAngleMove)
    {
        return SmoothDamp(current, target, ref velocity, damping, maxAngleMove, Time.deltaTime);
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, float damping, float maxAngleMove, float deltaTime)
    {
        if (current.x == target.x)
        {
            velocity.x = 0f;
            current.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, damping, maxAngleMove, deltaTime);
            return current;
        }
        if (current.y == target.y)
        {
            velocity.y = 0f;
            current.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, damping, maxAngleMove, deltaTime);
            return current;
        }
        Vector2 vector = (Vector2) (NormMags(current, target) * maxAngleMove);
        current.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, damping, vector.x, deltaTime);
        current.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, damping, vector.y, deltaTime);
        return current;
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, float damping, Vector2 maxAngleMove, float deltaTime)
    {
        current.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, damping, maxAngleMove.x, deltaTime);
        current.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, damping, maxAngleMove.y, deltaTime);
        return current;
    }

    public static Angle2 SmoothDamp(Angle2 current, Angle2 target, ref Vector2 velocity, Vector2 damping, Vector2 maxAngleMove, float deltaTime)
    {
        current.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, damping.x, maxAngleMove.x, deltaTime);
        current.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, damping.y, maxAngleMove.y, deltaTime);
        return current;
    }

    public static float SquareAngleDistance(Angle2 a, Angle2 b)
    {
        float num = Mathf.DeltaAngle(a.x, b.x);
        float num2 = Mathf.DeltaAngle(a.y, b.y);
        return ((num * num) + (num2 * num2));
    }

    public override string ToString()
    {
        return this.m.ToString();
    }

    public float angleMagnitude
    {
        get
        {
            return this.m.magnitude;
        }
    }

    public Vector3 back
    {
        get
        {
            return (Vector3) (this.quat * Vector3.back);
        }
        set
        {
            this.forward = -value;
        }
    }

    public Angle2 decoded
    {
        get
        {
            Angle2 angle = this;
            angle.encoded = this.encoded;
            return angle;
        }
    }

    public Vector3 down
    {
        get
        {
            return (Vector3) (this.quat * Vector3.down);
        }
    }

    public int encoded
    {
        get
        {
            return ((Encode360(this.y) << 0x10) | Encode360(this.x));
        }
        set
        {
            this.x = Decode360(value & 0xffff);
            this.y = Decode360((value >> 0x10) & 0xffff);
        }
    }

    public Vector3 eulerAngles
    {
        get
        {
            return new Vector3(-this.pitch, this.yaw);
        }
        set
        {
            this.pitch = -value.x;
            this.yaw = value.y;
        }
    }

    public Vector3 forward
    {
        get
        {
            return (Vector3) (this.quat * Vector3.forward);
        }
        set
        {
            this.quat = Quaternion.LookRotation(value);
        }
    }

    public float this[int index]
    {
        get
        {
            return this.m[index];
        }
        set
        {
            this.m[index] = value;
        }
    }

    public Vector3 left
    {
        get
        {
            return (Vector3) (this.quat * Vector3.left);
        }
    }

    public Angle2 normalized
    {
        get
        {
            return Normalize(this);
        }
    }

    public float normalizedAngleMagnitude
    {
        get
        {
            return Normalize(this).m.magnitude;
        }
    }

    public float normalizedSqrAngleMagnitude
    {
        get
        {
            return Normalize(this).m.sqrMagnitude;
        }
    }

    public Vector3 pitchEulerAngles
    {
        get
        {
            return new Vector3(-this.pitch, 0f);
        }
        set
        {
            this.pitch = -value.x;
        }
    }

    public Quaternion quat
    {
        get
        {
            return Quaternion.Euler(-this.pitch, this.yaw, 0f);
        }
        set
        {
            this.eulerAngles = value.eulerAngles;
        }
    }

    public Vector3 right
    {
        get
        {
            return (Vector3) (this.quat * Vector3.right);
        }
    }

    public float sqrAngleMagnitude
    {
        get
        {
            return this.m.sqrMagnitude;
        }
    }

    public Vector3 up
    {
        get
        {
            return (Vector3) (this.quat * Vector3.up);
        }
    }

    public Vector3 yawEulerAngles
    {
        get
        {
            return new Vector3(0f, this.yaw);
        }
        set
        {
            this.yaw = value.y;
        }
    }
}

