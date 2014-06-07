using System;
using System.Runtime.InteropServices;
using UnityEngine;

[NGCAutoAddScript]
public class MovingDoor : BasicDoor
{
    [NonSerialized]
    protected bool _gotRigidbody;
    [NonSerialized]
    protected Rigidbody _rigidbody;
    private Quaternion baseRot;
    [SerializeField]
    protected Vector3 closedPositionPivot;
    [SerializeField]
    protected float degrees;
    private const float kFixedTimeElapsedMinimum = 1.5f;
    [NonSerialized]
    private float kinematicFrameStart;
    [NonSerialized]
    private Vector3 lastLocalPosition;
    [NonSerialized]
    private Quaternion lastLocalRotation;
    [SerializeField]
    protected bool movementABS;
    [NonSerialized]
    private bool onceMoved;
    [SerializeField]
    protected Vector3 openMovement = Vector3.up;
    [SerializeField]
    protected bool rotationABS;
    [SerializeField]
    protected Vector3 rotationAxis = Vector3.up;
    [SerializeField]
    protected Vector3 rotationCross = Vector3.right;
    [SerializeField]
    protected bool rotationFirst;
    [SerializeField]
    protected bool slerpMovementByDegrees;
    [SerializeField]
    protected bool smooth;
    [NonSerialized]
    private int timesBoundKinematic;

    private static void DrawOpenGizmo(Vector3 closedPositionPivot, Vector3 rotationCross, Vector3 rotationAxis, float degrees, Vector3 openMovement, bool movementABS, bool rotationABS, bool rotationFirst, bool reversed)
    {
        Color color = Gizmos.color;
        Vector3 from = closedPositionPivot;
        Vector3 vector2 = from + rotationCross;
        bool flag = !Mathf.Approximately(degrees, 0f);
        bool flag2 = !Mathf.Approximately(openMovement.magnitude, 0f);
        Quaternion identity = Quaternion.identity;
        for (int i = 1; i < 0x15; i++)
        {
            Vector3 vector3;
            float t = ((float) i) / 20f;
            Quaternion quaternion2 = Quaternion.AngleAxis(degrees * ((!reversed || rotationABS) ? t : -t), rotationAxis);
            if (rotationFirst)
            {
                vector3 = closedPositionPivot + ((Vector3) (quaternion2 * (openMovement * ((!reversed || movementABS) ? t : -t))));
            }
            else
            {
                vector3 = closedPositionPivot + ((Vector3) (openMovement * ((!reversed || movementABS) ? t : -t)));
            }
            if (flag2)
            {
                Gizmos.color = Color.Lerp(Color.red, !reversed ? Color.green : Color.yellow, t);
                Gizmos.DrawLine(from, vector3);
            }
            from = vector3;
            Vector3 to = vector3 + (quaternion2 * rotationCross);
            if (flag)
            {
                Gizmos.color = Color.Lerp(Color.blue, !reversed ? Color.cyan : Color.magenta, t);
                Gizmos.DrawLine(vector2, to);
            }
            vector2 = to;
            identity = quaternion2;
        }
        if (flag)
        {
            Vector3 vector5 = closedPositionPivot + (!rotationFirst ? openMovement : (identity * openMovement));
            Gizmos.color = new Color(1f, !reversed ? 0f : 1f, 0f, 0.5f);
            Gizmos.DrawLine(Vector3.Lerp(vector5, vector2, 0.5f), vector2);
        }
        Gizmos.color = color;
    }

    protected override BasicDoor.IdealSide IdealSideForPoint(Vector3 worldPoint)
    {
        float f = Vector3.Dot(base.transform.InverseTransformPoint(worldPoint), Vector3.Cross(this.rotationCross, this.rotationAxis));
        if ((float.IsInfinity(f) || float.IsNaN(f)) || Mathf.Approximately(0f, f))
        {
            return BasicDoor.IdealSide.Unknown;
        }
        return ((f <= 0f) ? BasicDoor.IdealSide.Reverse : BasicDoor.IdealSide.Forward);
    }

    protected override void OnDoorFraction(double fractionOpen)
    {
        this.UpdateMovement(fractionOpen);
    }

    protected void UpdateMovement(double openFraction)
    {
        Vector3 vector;
        Quaternion quaternion;
        this.UpdateMovement(openFraction, out vector, out quaternion);
        base.transform.localPosition = vector;
        base.transform.localRotation = quaternion;
    }

    protected void UpdateMovement(double openFraction, out Vector3 localPosition, out Quaternion localRotation)
    {
        if (openFraction == 0.0)
        {
            localPosition = base.originalLocalPosition;
            localRotation = base.originalLocalRotation;
        }
        else
        {
            double num;
            double num2;
            double num3;
            if (this.smooth)
            {
                openFraction = (openFraction >= 0.0) ? ((double) Mathf.SmoothStep(0f, 1f, (float) openFraction)) : ((double) Mathf.SmoothStep(0f, -1f, (float) -openFraction));
            }
            if ((!this.slerpMovementByDegrees || (this.degrees == 0f)) || (((openFraction == 0.0) || (openFraction == 1.0)) || ((num2 = Math.Sin(num3 = (this.degrees * 3.1415926535897931) / 180.0)) == 0.0)))
            {
                num = openFraction;
            }
            else
            {
                num = Math.Sin(openFraction * num3) / num2;
            }
            Quaternion quaternion = (openFraction != 0.0) ? Quaternion.AngleAxis((float) (this.degrees * (!this.rotationABS ? openFraction : Math.Abs(openFraction))), this.rotationAxis) : Quaternion.identity;
            Vector3 vector = (Vector3) (this.openMovement * (!this.movementABS ? ((float) num) : ((float) Math.Abs(num))));
            Vector3 vector2 = ((Vector3) (quaternion * -this.closedPositionPivot)) + this.closedPositionPivot;
            if (this.rotationFirst)
            {
                localPosition = base.originalLocalPosition + (base.originalLocalRotation * (vector2 + (quaternion * vector)));
            }
            else
            {
                localPosition = base.originalLocalPosition + (base.originalLocalRotation * (vector2 + vector));
            }
            localRotation = (openFraction != 0.0) ? (base.originalLocalRotation * quaternion) : base.originalLocalRotation;
        }
    }

    public Rigidbody rigidbody
    {
        get
        {
            if (!this._gotRigidbody)
            {
                this._rigidbody = base.rigidbody;
                this._gotRigidbody = true;
            }
            return this._rigidbody;
        }
    }
}

