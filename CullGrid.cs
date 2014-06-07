using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class CullGrid : MonoBehaviour
{
    private static bool cull_prebinding = true;
    private static CullGridRuntime grid;
    private static bool has_grid;
    [SerializeField]
    private CullGridSetup setup;

    private void Awake()
    {
        RegisterGrid(this);
    }

    public static bool CellContainsPoint(ushort cell, ref Vector2 flatPoint)
    {
        return (cell == grid.FlatCell(ref flatPoint));
    }

    public static bool CellContainsPoint(ushort cell, ref Vector3 worldPoint)
    {
        return (cell == grid.WorldCell(ref worldPoint));
    }

    public static bool CellContainsPoint(ushort cell, ref Vector2 flatPoint, out ushort cell_point)
    {
        cell_point = grid.FlatCell(ref flatPoint);
        return (cell == cell_point);
    }

    public static bool CellContainsPoint(ushort cell, ref Vector3 worldPoint, out ushort cell_point)
    {
        cell_point = grid.WorldCell(ref worldPoint);
        return (cell_point == cell);
    }

    public static ushort CellFromGroupID(int groupID)
    {
        if ((groupID < grid.groupBegin) || (groupID >= grid.groupEnd))
        {
            throw new ArgumentOutOfRangeException("groupID", groupID, "groupID < grid.groupBegin || groupID >= grid.groupEnd");
        }
        return (ushort) (groupID - grid.groupBegin);
    }

    public static ushort CellFromGroupID(int groupID, out ushort x, out ushort y)
    {
        ushort num = CellFromGroupID(groupID);
        x = (ushort) (num % grid.cellsWide);
        y = (ushort) (num / grid.cellsWide);
        return num;
    }

    private void DrawGizmosNow()
    {
    }

    private void DrawGrid(int cell)
    {
        if (cell != -1)
        {
            this.DrawGrid(this.GetCenterSetup(cell));
        }
    }

    private void DrawGrid(Vector3 center)
    {
        Vector3 vector = (Vector3) (base.transform.right * (((float) this.setup.cellSquareDimension) / 2f));
        Vector3 vector2 = (Vector3) (base.transform.forward * (((float) this.setup.cellSquareDimension) / 2f));
        DrawQuadRayCastDown((center + vector) + vector2, (center + vector) - vector2, (center - vector) - vector2, (center - vector) + vector2);
    }

    private void DrawGrid(int centerCell, int xOffset, int yOffset)
    {
        this.DrawGrid((int) ((centerCell + xOffset) + ((this.setup.cellsWide * 2) * yOffset)));
    }

    private void DrawGrid(Vector3 center, float sizeX, float sizeY)
    {
        Vector3 vector = (Vector3) (base.transform.right * (sizeX / 2f));
        Vector3 vector2 = (Vector3) (base.transform.forward * (sizeY / 2f));
        DrawQuadRayCastDown((center + vector) + vector2, (center + vector) - vector2, (center - vector) - vector2, (center - vector) + vector2);
    }

    private static void DrawQuadRayCastDown(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        RaycastDownVect(ref a);
        RaycastDownVect(ref b);
        RaycastDownVect(ref c);
        RaycastDownVect(ref d);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
        if (a.y > c.y)
        {
            if (b.y > d.y)
            {
                if ((b.y - d.y) > (a.y - c.y))
                {
                    Gizmos.DrawLine(b, d);
                }
                else
                {
                    Gizmos.DrawLine(a, c);
                }
            }
            else if ((d.y - b.y) > (a.y - c.y))
            {
                Gizmos.DrawLine(d, b);
            }
            else
            {
                Gizmos.DrawLine(a, c);
            }
        }
        else if (b.y > d.y)
        {
            if ((b.y - d.y) > (c.y - a.y))
            {
                Gizmos.DrawLine(b, d);
            }
            else
            {
                Gizmos.DrawLine(c, a);
            }
        }
        else if ((d.y - b.y) > (c.y - a.y))
        {
            Gizmos.DrawLine(d, b);
        }
        else
        {
            Gizmos.DrawLine(c, a);
        }
    }

    public static Vector2 Flat(Vector3 triD)
    {
        Vector2 vector;
        vector.x = triD.x;
        vector.y = triD.z;
        return vector;
    }

    public static ushort FlatCell(Vector2 flat)
    {
        return grid.FlatCell(ref flat);
    }

    public static ushort FlatCell(ref Vector2 flat)
    {
        return grid.FlatCell(ref flat);
    }

    public static int FlatGroupID(ref Vector2 flat)
    {
        return (grid.FlatCell(ref flat) + grid.groupBegin);
    }

    private Vector3 GetCenterSetup(int cell)
    {
        CullGridSetup setup = this.setup;
        return (Vector3) ((base.transform.position + (base.transform.forward * (((cell / setup.cellsWide) - ((((float) setup.cellsTall) / 2f) - (((float) (2 - (setup.cellsTall & 1))) / 2f))) * setup.cellSquareDimension))) + (base.transform.right * (((cell % setup.cellsWide) - ((((float) setup.cellsWide) / 2f) - (((float) (2 - (setup.cellsWide & 1))) / 2f))) * setup.cellSquareDimension)));
    }

    public static bool GroupIDContainsPoint(int groupID, ref Vector2 flatPoint)
    {
        return (((groupID >= grid.groupBegin) && (groupID < grid.groupEnd)) && CellContainsPoint((ushort) (groupID - grid.groupBegin), ref flatPoint));
    }

    public static bool GroupIDContainsPoint(int groupID, ref Vector3 worldPoint)
    {
        return (((groupID >= grid.groupBegin) && (groupID < grid.groupEnd)) && CellContainsPoint((ushort) (groupID - grid.groupBegin), ref worldPoint));
    }

    public static bool GroupIDContainsPoint(int groupID, ref Vector2 flatPoint, out int groupID_point)
    {
        ushort num;
        if ((groupID < grid.groupBegin) || (groupID >= grid.groupEnd))
        {
            groupID_point = NetworkGroup.unassigned.id;
            return false;
        }
        if (!CellContainsPoint((ushort) (groupID - grid.groupBegin), ref flatPoint, out num))
        {
            groupID_point = num + grid.groupBegin;
            return false;
        }
        groupID_point = groupID;
        return true;
    }

    public static bool GroupIDContainsPoint(int groupID, ref Vector3 worldPoint, out int groupID_point)
    {
        ushort num;
        if ((groupID < grid.groupBegin) || (groupID >= grid.groupEnd))
        {
            groupID_point = NetworkGroup.unassigned.id;
            return false;
        }
        if (!CellContainsPoint(CellFromGroupID(groupID), ref worldPoint, out num))
        {
            groupID_point = GroupIDFromCell(num);
            return false;
        }
        groupID_point = groupID;
        return true;
    }

    public static int GroupIDFromCell(ushort cell)
    {
        if (cell >= grid.numCells)
        {
            throw new ArgumentOutOfRangeException("cell", cell, "cell >= grid.numCells");
        }
        return (grid.groupBegin + cell);
    }

    public static bool IsCellGroup(NetworkGroup group)
    {
        return IsCellGroupID(group.id);
    }

    public static bool IsCellGroupID(int usedGroup)
    {
        return ((has_grid && (usedGroup >= grid.groupBegin)) && (usedGroup < grid.groupEnd));
    }

    private static void RaycastDownVect(ref Vector3 a)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(a.x, 10000f, a.z), Vector3.down, out hit, float.PositiveInfinity))
        {
            a = hit.point + ((Vector3) (Vector3.up * a.y));
        }
    }

    private static void RegisterGrid(CullGrid grid)
    {
        if (grid != null)
        {
            CullGrid.grid = new CullGridRuntime(grid);
            has_grid = true;
        }
    }

    public static ushort WorldCell(Vector3 world)
    {
        return grid.WorldCell(ref world);
    }

    public static ushort WorldCell(ref Vector3 world)
    {
        return grid.WorldCell(ref world);
    }

    public static int WorldGroupID(ref Vector3 world)
    {
        return (grid.WorldCell(ref world) + grid.groupBegin);
    }

    public static bool autoPrebindInInstantiate
    {
        get
        {
            return (has_grid && cull_prebinding);
        }
    }

    public static int Tall
    {
        get
        {
            return grid.cellsTall;
        }
    }

    public static int Wide
    {
        get
        {
            return grid.cellsWide;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size=2)]
    public struct CellID
    {
        [FieldOffset(0)]
        public ushort id;
        private const ushort kInvalidID = 0xffff;

        public CellID(ushort cellID)
        {
            this.id = cellID;
        }

        public bool ContainsFlatPoint(Vector2 flatPoint)
        {
            return CullGrid.grid.Contains(this.id, ref flatPoint);
        }

        public bool ContainsFlatPoint(ref Vector2 flatPoint)
        {
            return (this.valid && CullGrid.grid.Contains(this.id, ref flatPoint));
        }

        public bool ContainsWorldPoint(Vector3 worldPoint)
        {
            return CullGrid.grid.Contains(this.id, ref worldPoint);
        }

        public bool ContainsWorldPoint(ref Vector3 worldPoint)
        {
            return (this.valid && CullGrid.grid.Contains(this.id, ref worldPoint));
        }

        private static ushort NextDown(ushort id)
        {
            return (((id / CullGrid.grid.cellsWide) != 0) ? ((ushort) (id - CullGrid.grid.cellsWide)) : ((ushort) 0xffff));
        }

        private static ushort NextLeft(ushort id)
        {
            return (((id % CullGrid.grid.cellsWide) != 0) ? ((ushort) (id - 1)) : ((ushort) 0xffff));
        }

        private static ushort NextRight(ushort id)
        {
            return (((id % CullGrid.grid.cellsWide) != CullGrid.grid.cellWideLast) ? ((ushort) (id + 1)) : ((ushort) 0xffff));
        }

        private static ushort NextUp(ushort id)
        {
            return (((id / CullGrid.grid.cellsWide) != CullGrid.grid.cellTallLast) ? ((ushort) (id + CullGrid.grid.cellsWide)) : ((ushort) 0xffff));
        }

        public int column
        {
            get
            {
                return (!this.valid ? -1 : (this.id % CullGrid.grid.cellsWide));
            }
        }

        public CullGrid.CellID down
        {
            get
            {
                CullGrid.CellID lid;
                lid.id = !this.valid ? ((ushort) 0xffff) : NextDown(this.id);
                return lid;
            }
        }

        public Vector2 flatCenter
        {
            get
            {
                Vector2 vector;
                CullGrid.grid.GetCenter(this.id, out vector);
                return vector;
            }
        }

        public Vector2 flatMax
        {
            get
            {
                Vector2 vector;
                CullGrid.grid.GetMin(this.id, out vector);
                return vector;
            }
        }

        public Vector2 flatMin
        {
            get
            {
                Vector2 vector;
                CullGrid.grid.GetMax(this.id, out vector);
                return vector;
            }
        }

        public Rect flatRect
        {
            get
            {
                Rect rect;
                CullGrid.grid.GetRect(this.id, out rect);
                return rect;
            }
        }

        public NetworkGroup group
        {
            get
            {
                return (!this.valid ? NetworkGroup.unassigned : CullGrid.GroupIDFromCell(this.id));
            }
        }

        public int groupID
        {
            get
            {
                return (!this.valid ? NetworkGroup.unassigned.id : CullGrid.GroupIDFromCell(this.id));
            }
        }

        public CullGrid.CellID left
        {
            get
            {
                CullGrid.CellID lid;
                lid.id = !this.valid ? ((ushort) 0xffff) : NextLeft(this.id);
                return lid;
            }
        }

        public bool mostBottom
        {
            get
            {
                return (this.valid && ((this.id / CullGrid.grid.cellsWide) == 0));
            }
        }

        public bool mostLeft
        {
            get
            {
                return (this.valid && ((this.id % CullGrid.grid.cellsWide) == 0));
            }
        }

        public bool mostRight
        {
            get
            {
                return (this.valid && ((this.id % CullGrid.grid.cellsWide) == CullGrid.grid.cellWideLast));
            }
        }

        public bool mostTop
        {
            get
            {
                return (this.valid && ((this.id / CullGrid.grid.cellsWide) == CullGrid.grid.cellTallLast));
            }
        }

        public CullGrid.CellID right
        {
            get
            {
                CullGrid.CellID lid;
                lid.id = !this.valid ? ((ushort) 0xffff) : NextRight(this.id);
                return lid;
            }
        }

        public int row
        {
            get
            {
                return (!this.valid ? -1 : (this.id / CullGrid.grid.cellsWide));
            }
        }

        public CullGrid.CellID up
        {
            get
            {
                CullGrid.CellID lid;
                lid.id = !this.valid ? ((ushort) 0xffff) : NextUp(this.id);
                return lid;
            }
        }

        public bool valid
        {
            get
            {
                return (this.id < CullGrid.grid.numCells);
            }
        }

        public Bounds worldBounds
        {
            get
            {
                Bounds bounds;
                CullGrid.grid.GetBounds(this.id, out bounds);
                return bounds;
            }
        }

        public Vector3 worldCenter
        {
            get
            {
                Vector3 vector;
                CullGrid.grid.GetCenter(this.id, out vector);
                return vector;
            }
        }

        public Vector3 worldMax
        {
            get
            {
                Vector3 vector;
                CullGrid.grid.GetMin(this.id, out vector);
                return vector;
            }
        }

        public Vector3 worldMin
        {
            get
            {
                Vector3 vector;
                CullGrid.grid.GetMax(this.id, out vector);
                return vector;
            }
        }
    }

    private class CullGridRuntime : CullGridSetup
    {
        public ushort cellTallLast;
        public double cellTallLastTimesSquareDimension;
        public ushort cellWideLast;
        public double cellWideLastTimesSquareDimension;
        public CullGrid cullGrid;
        public double flat_tall_ofs;
        public double flat_wide_ofs;
        public double fx;
        public double fy;
        public double fz;
        public int groupEnd;
        public double halfCellTall;
        public double halfCellTallMinusHalfTwoMinusOddTall;
        public double halfCellWide;
        public double halfCellWideMinusHalfTwoMinusOddWide;
        public double halfTwoMinusOddTall;
        public double halfTwoMinusOddWide;
        private const double kMAX_WORLD_Y = 32000.0;
        private const double kMIN_WORLD_Y = -32000.0;
        public int numCells;
        public double px;
        public double py;
        public double pz;
        public double rx;
        public double ry;
        public double rz;
        public Transform transform;
        public int twoMinusOddTall;
        public int twoMinusOddWide;

        public CullGridRuntime(CullGrid cullGrid) : base(cullGrid.setup)
        {
            this.cullGrid = cullGrid;
            this.transform = cullGrid.transform;
            this.halfCellTall = ((double) base.cellsTall) / 2.0;
            this.halfCellWide = ((double) base.cellsWide) / 2.0;
            this.twoMinusOddTall = 2 - (base.cellsTall & 1);
            this.twoMinusOddWide = 2 - (base.cellsWide & 1);
            this.halfTwoMinusOddTall = ((double) this.twoMinusOddTall) / 2.0;
            this.halfTwoMinusOddWide /= 2.0;
            this.halfCellTallMinusHalfTwoMinusOddTall = this.halfCellTall - this.halfTwoMinusOddTall;
            this.halfCellWideMinusHalfTwoMinusOddWide = this.halfCellWide - this.halfTwoMinusOddWide;
            Vector3 forward = this.transform.forward;
            Vector3 right = this.transform.right;
            Vector3 position = this.transform.position;
            this.fx = forward.x;
            this.fy = forward.y;
            this.fz = forward.z;
            double num = Math.Sqrt(((this.fx * this.fx) + (this.fy * this.fy)) + (this.fz * this.fz));
            this.fx /= num;
            this.fy /= num;
            this.fz /= num;
            this.rx = right.x;
            this.ry = right.y;
            this.rz = right.z;
            num = Math.Sqrt(((this.rx * this.rx) + (this.ry * this.ry)) + (this.rz * this.rz));
            this.rx /= num;
            this.ry /= num;
            this.rz /= num;
            this.px = position.x;
            this.py = position.y;
            this.pz = position.z;
            this.flat_wide_ofs = base.cellSquareDimension * (this.halfCellWide - (((double) (1 - (base.cellsWide & 1))) / 2.0));
            this.flat_tall_ofs = base.cellSquareDimension * (this.halfCellTall - (((double) (1 - (base.cellsTall & 1))) / 2.0));
            this.cellTallLast = (ushort) (base.cellsTall - 1);
            this.cellWideLast = (ushort) (base.cellsWide - 1);
            this.cellTallLastTimesSquareDimension = this.cellTallLast * base.cellSquareDimension;
            this.cellWideLastTimesSquareDimension = this.cellWideLast * base.cellSquareDimension;
            this.numCells = base.cellsTall * base.cellsWide;
            this.groupEnd = base.groupBegin + this.numCells;
        }

        public bool Contains(int cell, ref Vector2 flatPoint)
        {
            return (((cell >= 0) && (cell < this.numCells)) && (this.FlatCell(ref flatPoint) == cell));
        }

        public bool Contains(int cell, ref Vector3 worldPoint)
        {
            return (((cell >= 0) && (cell < this.numCells)) && (this.WorldCell(ref worldPoint) == cell));
        }

        public bool Contains(int x, int y, ref Vector2 flatPoint)
        {
            return this.Contains((y * base.cellsWide) + x, ref flatPoint);
        }

        public bool Contains(int x, int y, ref Vector3 worldPoint)
        {
            return this.Contains((y * base.cellsWide) + x, ref worldPoint);
        }

        public List<ushort> EnumerateNearbyCells(int cell)
        {
            return this.EnumerateNearbyCells(cell, cell % CullGrid.grid.cellsWide, cell / CullGrid.grid.cellsWide);
        }

        public List<ushort> EnumerateNearbyCells(int x, int y)
        {
            return this.EnumerateNearbyCells((y * base.cellsWide) + x, x, y);
        }

        public List<ushort> EnumerateNearbyCells(int i, int x, int y)
        {
            if (i < 0)
            {
                throw new ArgumentOutOfRangeException("i", i, "i<0");
            }
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException("x", x, "x<0");
            }
            if (y < 0)
            {
                throw new ArgumentOutOfRangeException("y", y, "y<0");
            }
            List<ushort> list = new List<ushort>();
            int num = -(base.gatheringCellsCenter % base.gatheringCellsWide);
            int num2 = -(base.gatheringCellsCenter / base.gatheringCellsWide);
            for (int j = 0; j < base.gatheringCellsWide; j++)
            {
                int num4 = j + num;
                int num5 = x + num4;
                if ((num5 >= 0) && (num5 < base.cellsWide))
                {
                    for (int k = 0; k < base.gatheringCellsTall; k++)
                    {
                        int num7 = k + num2;
                        int num8 = y + num7;
                        if (((num8 >= 0) && (num8 < base.cellsTall)) && base.GetGatheringBit(j, k))
                        {
                            ushort item = (ushort) (num5 + (num8 * base.cellsWide));
                            if ((num8 == y) && (num5 == x))
                            {
                                list.Insert(0, item);
                            }
                            else
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public ushort FlatCell(ref Vector2 point)
        {
            int cellWideLast;
            int cellTallLast;
            double num2 = point.x + this.flat_wide_ofs;
            if (num2 <= 0.0)
            {
                cellWideLast = 0;
            }
            else if (num2 >= this.cellWideLastTimesSquareDimension)
            {
                cellWideLast = this.cellWideLast;
            }
            else
            {
                cellWideLast = (int) Math.Floor((double) (num2 / ((double) base.cellSquareDimension)));
            }
            double num4 = point.y + this.flat_tall_ofs;
            if (num4 <= 0.0)
            {
                cellTallLast = 0;
            }
            else if (num4 >= this.cellTallLastTimesSquareDimension)
            {
                cellTallLast = this.cellTallLast;
            }
            else
            {
                cellTallLast = (int) Math.Floor((double) (num4 / ((double) base.cellSquareDimension)));
            }
            return (ushort) ((cellTallLast * base.cellsWide) + cellWideLast);
        }

        public ushort FlatCell(ref Vector2 point, out ushort x, out ushort y)
        {
            double num = point.x + this.flat_wide_ofs;
            if (num <= 0.0)
            {
                x = 0;
            }
            else if (num >= this.cellWideLastTimesSquareDimension)
            {
                x = this.cellWideLast;
            }
            else
            {
                x = (ushort) Math.Floor((double) (num / ((double) base.cellSquareDimension)));
            }
            double num2 = point.y + this.flat_tall_ofs;
            if (num2 <= 0.0)
            {
                y = 0;
            }
            else if (num2 >= this.cellTallLastTimesSquareDimension)
            {
                y = this.cellTallLast;
            }
            else
            {
                y = (ushort) Math.Floor((double) (num2 / ((double) base.cellSquareDimension)));
            }
            return (ushort) ((y * base.cellsWide) + x);
        }

        public void GetBounds(int cell, out Bounds bounds)
        {
            Vector3 vector;
            int x = cell % base.cellsWide;
            int y = cell / base.cellsWide;
            this.GetCenter(x, y, out vector);
            bounds = new Bounds(vector, new Vector3((float) base.cellSquareDimension, 64000f, (float) base.cellSquareDimension));
        }

        public void GetBounds(int x, int y, out Bounds bounds)
        {
            Vector3 vector;
            this.GetCenter(x, y, out vector);
            bounds = new Bounds(vector, new Vector3((float) base.cellSquareDimension, 64000f, (float) base.cellSquareDimension));
        }

        public void GetCenter(int cell, out Vector2 center)
        {
            double num = ((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) * base.cellSquareDimension;
            double num2 = ((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) * base.cellSquareDimension;
            center.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            center.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetCenter(int cell, out Vector3 center)
        {
            double num = ((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) * base.cellSquareDimension;
            double num2 = ((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) * base.cellSquareDimension;
            center.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            center.y = (float) ((this.py + (this.fy * num2)) + (this.ry * num));
            center.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetCenter(int x, int y, out Vector2 center)
        {
            double num = (x - this.halfCellWideMinusHalfTwoMinusOddWide) * base.cellSquareDimension;
            double num2 = (y - this.halfCellTallMinusHalfTwoMinusOddTall) * base.cellSquareDimension;
            center.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            center.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetCenter(int x, int y, out Vector3 center)
        {
            double num = (x - this.halfCellWideMinusHalfTwoMinusOddWide) * base.cellSquareDimension;
            double num2 = (y - this.halfCellTallMinusHalfTwoMinusOddTall) * base.cellSquareDimension;
            center.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            center.y = (float) ((this.py + (this.fy * num2)) + (this.ry * num));
            center.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMax(int cell, out Vector2 max)
        {
            double num = (((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) + 0.5) * base.cellSquareDimension;
            double num2 = (((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) + 0.5) * base.cellSquareDimension;
            max.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            max.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMax(int cell, out Vector3 max)
        {
            double num = (((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) + 0.5) * base.cellSquareDimension;
            double num2 = (((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) + 0.5) * base.cellSquareDimension;
            max.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            max.y = (float) (32000.0 + ((this.py + (this.fy * num2)) + (this.ry * num)));
            max.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMax(int x, int y, out Vector2 max)
        {
            double num = ((x - this.halfCellTallMinusHalfTwoMinusOddTall) + 0.5) * base.cellSquareDimension;
            double num2 = ((y - this.halfCellTallMinusHalfTwoMinusOddTall) + 0.5) * base.cellSquareDimension;
            max.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            max.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMax(int x, int y, out Vector3 max)
        {
            double num = ((x - this.halfCellWideMinusHalfTwoMinusOddWide) + 0.5) * base.cellSquareDimension;
            double num2 = ((y - this.halfCellTallMinusHalfTwoMinusOddTall) + 0.5) * base.cellSquareDimension;
            max.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            max.y = (float) (32000.0 + ((this.py + (this.fy * num2)) + (this.ry * num)));
            max.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMin(int cell, out Vector2 min)
        {
            double num = (((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num2 = (((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            min.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            min.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMin(int cell, out Vector3 min)
        {
            double num = (((cell % base.cellsWide) - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num2 = (((cell / base.cellsWide) - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            min.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            min.y = (float) (-32000.0 + ((this.py + (this.fy * num2)) + (this.ry * num)));
            min.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMin(int x, int y, out Vector2 min)
        {
            double num = ((x - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num2 = ((y - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            min.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            min.y = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetMin(int x, int y, out Vector3 min)
        {
            double num = ((x - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num2 = ((y - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            min.x = (float) ((this.px + (this.fx * num2)) + (this.rx * num));
            min.y = (float) (-32000.0 + ((this.py + (this.fy * num2)) + (this.ry * num)));
            min.z = (float) ((this.pz + (this.fz * num2)) + (this.rz * num));
        }

        public void GetRect(int cell, out Rect rect)
        {
            float num7;
            float num8;
            float num9;
            float num10;
            int num = cell % base.cellsWide;
            int num2 = cell / base.cellsWide;
            double num3 = ((num - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num4 = ((num2 - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            double num5 = num3 + base.cellSquareDimension;
            double num6 = num4 + base.cellSquareDimension;
            double num11 = (this.px + (this.fx * num4)) + (this.rx * num3);
            double num12 = (this.px + (this.fx * num6)) + (this.ry * num5);
            if (num11 < num12)
            {
                num7 = (float) num11;
                num8 = (float) (num12 - num11);
            }
            else
            {
                num7 = (float) num12;
                num8 = (float) (num11 - num12);
            }
            num11 = (this.pz + (this.fz * num4)) + (this.rx * num3);
            num12 = (this.pz + (this.fz * num6)) + (this.rx * num5);
            if (num11 < num12)
            {
                num9 = (float) num11;
                num10 = (float) (num12 - num11);
            }
            else
            {
                num9 = (float) num12;
                num10 = (float) (num11 - num12);
            }
            rect = new Rect(num7, num9, num8, num10);
        }

        public void GetRect(int x, int y, out Rect rect)
        {
            float num5;
            float num6;
            float num7;
            float num8;
            double num = ((x - this.halfCellWideMinusHalfTwoMinusOddWide) - 0.5) * base.cellSquareDimension;
            double num2 = ((y - this.halfCellTallMinusHalfTwoMinusOddTall) - 0.5) * base.cellSquareDimension;
            double num3 = num + base.cellSquareDimension;
            double num4 = num2 + base.cellSquareDimension;
            double num9 = (this.px + (this.fx * num2)) + (this.rx * num);
            double num10 = (this.px + (this.fx * num4)) + (this.ry * num3);
            if (num9 < num10)
            {
                num5 = (float) num9;
                num6 = (float) (num10 - num9);
            }
            else
            {
                num5 = (float) num10;
                num6 = (float) (num9 - num10);
            }
            num9 = (this.pz + (this.fz * num2)) + (this.rx * num);
            num10 = (this.pz + (this.fz * num4)) + (this.rx * num3);
            if (num9 < num10)
            {
                num7 = (float) num9;
                num8 = (float) (num10 - num9);
            }
            else
            {
                num7 = (float) num10;
                num8 = (float) (num9 - num10);
            }
            rect = new Rect(num5, num7, num6, num8);
        }

        public ushort WorldCell(ref Vector3 point)
        {
            int cellWideLast;
            int cellTallLast;
            double num2 = point.x + this.flat_wide_ofs;
            if (num2 <= 0.0)
            {
                cellWideLast = 0;
            }
            else if (num2 >= this.cellWideLastTimesSquareDimension)
            {
                cellWideLast = this.cellWideLast;
            }
            else
            {
                cellWideLast = (int) Math.Floor((double) (num2 / ((double) base.cellSquareDimension)));
            }
            double num4 = point.z + this.flat_tall_ofs;
            if (num4 <= 0.0)
            {
                cellTallLast = 0;
            }
            else if (num4 >= this.cellTallLastTimesSquareDimension)
            {
                cellTallLast = this.cellTallLast;
            }
            else
            {
                cellTallLast = (int) Math.Floor((double) (num4 / ((double) base.cellSquareDimension)));
            }
            return (ushort) ((cellTallLast * base.cellsWide) + cellWideLast);
        }

        public ushort WorldCell(ref Vector3 point, out ushort x, out ushort y)
        {
            double num = point.x + this.flat_wide_ofs;
            if (num <= 0.0)
            {
                x = 0;
            }
            else if (num >= this.cellWideLastTimesSquareDimension)
            {
                x = this.cellWideLast;
            }
            else
            {
                x = (ushort) Math.Floor((double) (num / ((double) base.cellSquareDimension)));
            }
            double num2 = point.z + this.flat_tall_ofs;
            if (num2 <= 0.0)
            {
                y = 0;
            }
            else if (num2 >= this.cellTallLastTimesSquareDimension)
            {
                y = this.cellTallLast;
            }
            else
            {
                y = (ushort) Math.Floor((double) (num2 / ((double) base.cellSquareDimension)));
            }
            return (ushort) ((y * base.cellsWide) + x);
        }
    }
}

