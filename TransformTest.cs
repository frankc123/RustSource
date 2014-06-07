using Facepunch.Precision;
using System;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Matrix4x4G matrixxg;
        Vector3G vectorg;
        Vector3G vectorg2;
        Vector3G vectorg3;
        Vector3G vectorg4;
        Vector3G vectorg5;
        base.transform.ExtractLocalToWorld(out matrixxg);
        Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
        Vector3 from = localToWorldMatrix.MultiplyPoint(Vector3.zero);
        Vector3 to = localToWorldMatrix.MultiplyPoint(Vector3.forward);
        Vector3 vector3 = localToWorldMatrix.MultiplyPoint(Vector3.up);
        Vector3 vector4 = localToWorldMatrix.MultiplyPoint(Vector3.right);
        vectorg5.x = 1.0;
        vectorg5.y = 0.0;
        vectorg5.z = 0.0;
        Matrix4x4G.Mult(ref vectorg5, ref matrixxg, out vectorg);
        vectorg5.x = 0.0;
        vectorg5.y = 1.0;
        vectorg5.z = 0.0;
        Matrix4x4G.Mult(ref vectorg5, ref matrixxg, out vectorg2);
        vectorg5.x = 0.0;
        vectorg5.y = 0.0;
        vectorg5.z = 1.0;
        Matrix4x4G.Mult(ref vectorg5, ref matrixxg, out vectorg3);
        vectorg5.x = 0.0;
        vectorg5.y = 0.0;
        vectorg5.z = 0.0;
        Matrix4x4G.Mult(ref vectorg5, ref matrixxg, out vectorg4);
        Gizmos.color = Color.red * new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawLine(from, vector4);
        Gizmos.color = Color.green * new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawLine(from, vector3);
        Gizmos.color = Color.blue * new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawLine(from, to);
        Gizmos.color = Color.red * new Color(1f, 1f, 1f, 1f);
        Gizmos.DrawLine(vectorg4.f, vectorg.f);
        Gizmos.color = Color.green * new Color(1f, 1f, 1f, 1f);
        Gizmos.DrawLine(vectorg4.f, vectorg2.f);
        Gizmos.color = Color.blue * new Color(1f, 1f, 1f, 1f);
        Gizmos.DrawLine(vectorg4.f, vectorg3.f);
    }
}

