using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class dfClippingUtil
{
    private static ClipTriangle[] clipDest = initClipBuffer(0x400);
    private static ClipTriangle[] clipSource = initClipBuffer(0x400);
    private static int[] inside = new int[3];

    public static void Clip(IList<Plane> planes, dfRenderData source, dfRenderData dest)
    {
        dest.EnsureCapacity(dest.Vertices.Count + source.Vertices.Count);
        for (int i = 0; i < source.Triangles.Count; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                int num3 = source.Triangles[i + j];
                dfClippingUtil.clipSource[0].corner[j] = source.Transform.MultiplyPoint(source.Vertices[num3]);
                dfClippingUtil.clipSource[0].uv[j] = source.UV[num3];
                dfClippingUtil.clipSource[0].color[j] = source.Colors[num3];
            }
            int count = 1;
            for (int k = 0; k < planes.Count; k++)
            {
                count = clipToPlane(planes[k], dfClippingUtil.clipSource, clipDest, count);
                ClipTriangle[] clipSource = dfClippingUtil.clipSource;
                dfClippingUtil.clipSource = clipDest;
                clipDest = clipSource;
            }
            for (int m = 0; m < count; m++)
            {
                dfClippingUtil.clipSource[m].CopyTo(dest);
            }
        }
    }

    private static int clipToPlane(Plane plane, ClipTriangle[] source, ClipTriangle[] dest, int count)
    {
        int destIndex = 0;
        for (int i = 0; i < count; i++)
        {
            destIndex += clipToPlane(plane, source[i], dest, destIndex);
        }
        return destIndex;
    }

    private static int clipToPlane(Plane plane, ClipTriangle triangle, ClipTriangle[] dest, int destIndex)
    {
        Vector3[] corner = triangle.corner;
        int num = 0;
        int num2 = 0;
        Vector3 normal = plane.normal;
        float distance = plane.distance;
        for (int i = 0; i < 3; i++)
        {
            if ((Vector3.Dot(normal, corner[i]) + distance) > 0f)
            {
                inside[num++] = i;
            }
            else
            {
                num2 = i;
            }
        }
        switch (num)
        {
            case 3:
                triangle.CopyTo(dest[destIndex]);
                return 1;

            case 0:
                return 0;

            case 1:
            {
                int num5 = inside[0];
                int num6 = (num5 + 1) % 3;
                int num7 = (num5 + 2) % 3;
                Vector3 vector2 = corner[num5];
                Vector3 vector3 = corner[num6];
                Vector3 vector4 = corner[num7];
                Vector2 vector5 = triangle.uv[num5];
                Vector2 vector6 = triangle.uv[num6];
                Vector2 vector7 = triangle.uv[num7];
                Color32 color = triangle.color[num5];
                Color32 color2 = triangle.color[num6];
                Color32 color3 = triangle.color[num7];
                float num8 = 0f;
                Vector3 vector8 = vector3 - vector2;
                Ray ray = new Ray(vector2, vector8.normalized);
                plane.Raycast(ray, out num8);
                float num9 = num8 / vector8.magnitude;
                Vector3 vector9 = ray.origin + ((Vector3) (ray.direction * num8));
                Vector2 vector10 = Vector2.Lerp(vector5, vector6, num9);
                Color color4 = Color.Lerp((Color) color, (Color) color2, num9);
                vector8 = vector4 - vector2;
                ray = new Ray(vector2, vector8.normalized);
                plane.Raycast(ray, out num8);
                num9 = num8 / vector8.magnitude;
                Vector3 vector11 = ray.origin + ((Vector3) (ray.direction * num8));
                Vector2 vector12 = Vector2.Lerp(vector5, vector7, num9);
                Color color5 = Color.Lerp((Color) color, (Color) color3, num9);
                dest[destIndex].corner[0] = vector2;
                dest[destIndex].corner[1] = vector9;
                dest[destIndex].corner[2] = vector11;
                dest[destIndex].uv[0] = vector5;
                dest[destIndex].uv[1] = vector10;
                dest[destIndex].uv[2] = vector12;
                dest[destIndex].color[0] = color;
                dest[destIndex].color[1] = color4;
                dest[destIndex].color[2] = color5;
                return 1;
            }
        }
        int index = num2;
        int num11 = (index + 1) % 3;
        int num12 = (index + 2) % 3;
        Vector3 origin = corner[index];
        Vector3 vector14 = corner[num11];
        Vector3 vector15 = corner[num12];
        Vector2 from = triangle.uv[index];
        Vector2 to = triangle.uv[num11];
        Vector2 vector18 = triangle.uv[num12];
        Color32 color6 = triangle.color[index];
        Color32 color7 = triangle.color[num11];
        Color32 color8 = triangle.color[num12];
        Vector3 vector19 = vector14 - origin;
        Ray ray2 = new Ray(origin, vector19.normalized);
        float enter = 0f;
        plane.Raycast(ray2, out enter);
        float t = enter / vector19.magnitude;
        Vector3 vector20 = ray2.origin + ((Vector3) (ray2.direction * enter));
        Vector2 vector21 = Vector2.Lerp(from, to, t);
        Color color9 = Color.Lerp((Color) color6, (Color) color7, t);
        vector19 = vector15 - origin;
        ray2 = new Ray(origin, vector19.normalized);
        plane.Raycast(ray2, out enter);
        t = enter / vector19.magnitude;
        Vector3 vector22 = ray2.origin + ((Vector3) (ray2.direction * enter));
        Vector2 vector23 = Vector2.Lerp(from, vector18, t);
        Color color10 = Color.Lerp((Color) color6, (Color) color8, t);
        dest[destIndex].corner[0] = vector20;
        dest[destIndex].corner[1] = vector14;
        dest[destIndex].corner[2] = vector22;
        dest[destIndex].uv[0] = vector21;
        dest[destIndex].uv[1] = to;
        dest[destIndex].uv[2] = vector23;
        dest[destIndex].color[0] = color9;
        dest[destIndex].color[1] = color7;
        dest[destIndex].color[2] = color10;
        destIndex++;
        dest[destIndex].corner[0] = vector22;
        dest[destIndex].corner[1] = vector14;
        dest[destIndex].corner[2] = vector15;
        dest[destIndex].uv[0] = vector23;
        dest[destIndex].uv[1] = to;
        dest[destIndex].uv[2] = vector18;
        dest[destIndex].color[0] = color10;
        dest[destIndex].color[1] = color7;
        dest[destIndex].color[2] = color8;
        return 2;
    }

    private static ClipTriangle[] initClipBuffer(int size)
    {
        ClipTriangle[] triangleArray = new ClipTriangle[size];
        for (int i = 0; i < size; i++)
        {
            triangleArray[i].corner = new Vector3[3];
            triangleArray[i].uv = new Vector2[3];
            triangleArray[i].color = new Color32[3];
        }
        return triangleArray;
    }

    [StructLayout(LayoutKind.Sequential)]
    protected struct ClipTriangle
    {
        public Vector3[] corner;
        public Vector2[] uv;
        public Color32[] color;
        public void CopyTo(dfClippingUtil.ClipTriangle target)
        {
            Array.Copy(this.corner, target.corner, 3);
            Array.Copy(this.uv, target.uv, 3);
            Array.Copy(this.color, target.color, 3);
        }

        public void CopyTo(dfRenderData buffer)
        {
            int count = buffer.Vertices.Count;
            buffer.Vertices.AddRange(this.corner);
            buffer.UV.AddRange(this.uv);
            buffer.Colors.AddRange(this.color);
            buffer.Triangles.Add(count);
            buffer.Triangles.Add(count + 1);
            buffer.Triangles.Add(count + 2);
        }
    }
}

