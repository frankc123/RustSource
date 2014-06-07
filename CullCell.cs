using Facepunch;
using System;
using uLink;
using UnityEngine;

public class CullCell : MonoBehaviour
{
    [NonSerialized]
    public Bounds bounds;
    [NonSerialized]
    public Vector2 center;
    [NonSerialized]
    public float extent;
    [NonSerialized]
    public int groupID;
    private string groupString;
    private const float kFadeDistance = 10f;
    private const float kMaxDistance = 150f;
    [NonSerialized]
    public float size;
    [NonSerialized]
    private Transform t_mc;
    [NonSerialized]
    private Transform t_my;
    [NonSerialized]
    private Transform t_mY;
    [NonSerialized]
    private Transform t_xc;
    [NonSerialized]
    private Transform t_Xc;
    [NonSerialized]
    private Transform t_xy;
    [NonSerialized]
    private Transform t_xY;
    [NonSerialized]
    private Transform t_Xy;
    [NonSerialized]
    private Transform t_XY;
    [NonSerialized]
    public float y_mc;
    [NonSerialized]
    public float y_my;
    [NonSerialized]
    public float y_mY;
    [NonSerialized]
    public float y_xc;
    [NonSerialized]
    public float y_Xc;
    [NonSerialized]
    public float y_xy;
    [NonSerialized]
    public float y_xY;
    [NonSerialized]
    public float y_Xy;
    [NonSerialized]
    public float y_XY;

    private static float HeightCast(Vector2 point)
    {
        RaycastHit hit;
        return (!Physics.Raycast(new Vector3(point.x, 5000f, point.y), Vector3.down, out hit, float.PositiveInfinity, g.terrainMask) ? 0f : hit.point.y);
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Vector3 position = this.t_mc.position;
            Camera main = Camera.main;
            if (main != null)
            {
                Vector3 screenPoint = main.WorldToScreenPoint(position);
                if ((screenPoint.z > 0f) && (screenPoint.z < 150f))
                {
                    Vector2 vector3 = GUIUtility.ScreenToGUIPoint(screenPoint);
                    vector3.y = Screen.height - (vector3.y + 1f);
                    if (screenPoint.z > 10f)
                    {
                        GUI.color *= new Color(1f, 1f, 1f, 1f - ((screenPoint.z - 10f) / 140f));
                    }
                    Rect rect = new Rect(vector3.x - 64f, vector3.y - 12f, 128f, 24f);
                    if (string.IsNullOrEmpty(this.groupString))
                    {
                        this.groupString = base.networkView.group.ToString();
                    }
                    GUI.Label(rect, this.groupString);
                }
            }
        }
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        ushort num;
        ushort num2;
        NetworkView networkView = info.networkView;
        this.groupID = (int) info.networkView.group;
        this.center = CullGrid.Flat(networkView.position);
        this.size = networkView.position.y;
        this.extent = this.size / 2f;
        CullGrid.CellFromGroupID(this.groupID, out num, out num2);
        base.name = string.Format("GRID-CELL:{0:00000}-[{1},{2}]", this.groupID, num, num2);
        this.y_mc = HeightCast(this.center);
        this.y_xy = HeightCast(new Vector2(this.center.x - this.extent, this.center.y - this.extent));
        this.y_XY = HeightCast(new Vector2(this.center.x + this.extent, this.center.y + this.extent));
        this.y_Xy = HeightCast(new Vector2(this.center.x + this.extent, this.center.y - this.extent));
        this.y_xY = HeightCast(new Vector2(this.center.x - this.extent, this.center.y + this.extent));
        this.y_xc = HeightCast(new Vector2(this.center.x - this.extent, this.center.y));
        this.y_Xc = HeightCast(new Vector2(this.center.x + this.extent, this.center.y));
        this.y_my = HeightCast(new Vector2(this.center.x, this.center.y - this.extent));
        this.y_mY = HeightCast(new Vector2(this.center.x, this.center.y + this.extent));
        base.transform.position = new Vector3(this.center.x, this.y_mc, this.center.y);
        float[] values = new float[] { this.y_xy, this.y_XY, this.y_Xy, this.y_xY, this.y_xc, this.y_Xc, this.y_my, this.y_mY, this.y_mc };
        float num3 = Mathf.Min(values);
        float[] singleArray2 = new float[] { this.y_xy, this.y_XY, this.y_Xy, this.y_xY, this.y_xc, this.y_Xc, this.y_my, this.y_mY, this.y_mc };
        float y = Mathf.Max(singleArray2) - num3;
        this.bounds = new Bounds(new Vector3(this.center.x, num3 + (y * 0.5f), this.center.y), new Vector3(this.size, y, this.size));
        Transform transform = base.transform;
        this.t_xy = transform.FindChild("BL");
        this.t_XY = transform.FindChild("FR");
        this.t_Xy = transform.FindChild("BR");
        this.t_xY = transform.FindChild("FL");
        this.t_xc = transform.FindChild("ML");
        this.t_Xc = transform.FindChild("MR");
        this.t_my = transform.FindChild("BC");
        this.t_mY = transform.FindChild("FC");
        this.t_mc = transform.FindChild("MC");
        this.t_xy.position = new Vector3(this.center.x - this.extent, this.y_xy, this.center.y - this.extent);
        this.t_XY.position = new Vector3(this.center.x + this.extent, this.y_XY, this.center.y + this.extent);
        this.t_Xy.position = new Vector3(this.center.x + this.extent, this.y_Xy, this.center.y - this.extent);
        this.t_xY.position = new Vector3(this.center.x - this.extent, this.y_xY, this.center.y + this.extent);
        this.t_xc.position = new Vector3(this.center.x - this.extent, this.y_xc, this.center.y);
        this.t_Xc.position = new Vector3(this.center.x + this.extent, this.y_Xc, this.center.y);
        this.t_my.position = new Vector3(this.center.x, this.y_my, this.center.y - this.extent);
        this.t_mY.position = new Vector3(this.center.x, this.y_mY, this.center.y + this.extent);
        this.t_mc.position = new Vector3(this.center.x, this.y_mc, this.center.y);
        transform.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().localBounds = new Bounds(new Vector3(0f, this.y_mc - (num3 + (y * 0.5f)), 0f), new Vector3(this.size, y, this.size));
    }

    public static Quaternion instantiateRotation
    {
        get
        {
            return new Quaternion(0f, 0.7071068f, 0.7071068f, 0f);
        }
    }

    private static class g
    {
        public static readonly int terrainMask = (((int) 1) << LayerMask.NameToLayer("Terrain"));
    }
}

