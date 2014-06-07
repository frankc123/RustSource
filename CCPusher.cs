using Facepunch.Geometry;
using System;
using UnityEngine;

public class CCPusher : MonoBehaviour
{
    [SerializeField]
    private Vector3 pushDir0 = Vector3.back;
    [SerializeField]
    private Vector3 pushDir1 = Vector3.forward;
    [SerializeField]
    private Vector3 pushPoint0 = Vector3.forward;
    [SerializeField]
    private Vector3 pushPoint1 = Vector3.back;
    [SerializeField]
    private float pushSpeed = 3f;
    [SerializeField]
    private ShapeDefinition shape;

    private static void DrawPushPlane(Matrix4x4 trs, Vector3 point, Vector3 dir)
    {
        point = trs.MultiplyPoint3x4(point);
        dir = trs.MultiplyVector(dir);
        Vector3 to = point + ((Vector3) (dir.normalized * 0.1f));
        Gizmos.DrawLine(point, to);
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix = matrix * Matrix4x4.TRS(point, Quaternion.LookRotation(dir), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 1f, 0.0001f));
        Gizmos.matrix = matrix;
    }

    private void OnDrawGizmosSelected()
    {
        Collider collider = base.collider;
        if (collider != null)
        {
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.8f);
            this.shape.Transform(collider.ColliderToWorld()).Gizmo();
            Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
            Gizmos.color = new Color(0.9f, 0.5f, 1f, 0.8f);
            DrawPushPlane(localToWorldMatrix, this.pushPoint0, this.pushDir0);
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.8f);
            DrawPushPlane(localToWorldMatrix, this.pushPoint1, this.pushDir1);
        }
    }

    public bool Push(Sphere Sphere, ref Vector3 Velocity)
    {
        Vector vector;
        if (!this.shape.Shape.Intersects(Sphere))
        {
            return false;
        }
        Plane plane = Plane.DirectionPoint(this.pushDir0, this.pushPoint0);
        Plane plane2 = Plane.DirectionPoint(this.pushDir1, this.pushPoint1);
        float num = plane.DistanceTo(Sphere.Center + ((Point) (((Normal) plane.Direction) * Sphere.Radius)));
        float num2 = plane2.DistanceTo(Sphere.Center + ((Point) (((Normal) plane2.Direction) * Sphere.Radius)));
        if (num > num2)
        {
            vector = (Vector) (((Normal) plane.Direction) * (this.pushSpeed * Time.deltaTime));
        }
        else
        {
            vector = (Vector) (((Normal) plane2.Direction) * (this.pushSpeed * Time.deltaTime));
        }
        Velocity.x += vector.x;
        Velocity.y += vector.y;
        Velocity.z += vector.z;
        return true;
    }

    private void Reset()
    {
        Shape shape;
        if (this.shape == null)
        {
            this.shape = new ShapeDefinition();
        }
        if ((base.collider != null) && base.collider.GetGeometricShapeLocal(out shape))
        {
            this.shape.Shape = shape;
        }
    }
}

