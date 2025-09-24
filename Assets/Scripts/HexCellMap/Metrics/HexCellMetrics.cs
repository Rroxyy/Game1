using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


/////////////////////////
///		LU		UR    ///
/// L				R ///
///		DL		RD    ///
/////////////////////////
public enum HexCellDirection
{
    R,
    RD,
    DL,
    L,
    LU,
    UR,
    
}

public enum HexAllDirection
{
    Deg0   = 0,  // 正右
    Deg30  = 1,  // 右下角
    Deg60  = 2,  // 右下边
    Deg90  = 3,  // 下右角
    Deg120 = 4,  // 左下边
    Deg150 = 5,  // 左下角
    Deg180 = 6,  // 正左
    Deg210 = 7,  // 左上角
    Deg240 = 8,  // 左上边
    Deg270 = 9,  // 上左角
    Deg300 = 10, // 右上边
    Deg330 = 11  // 右上角
}


public static class HexCellMetrics
{
    public static readonly float outerRadius = 0.8f;
    public static readonly float innerRadius = Mathf.Sqrt(3f) * outerRadius / 2f;

    public static readonly float heightFactor = 0.3f;

    public static readonly HexCellDirection[] AllDirections =
    {
        HexCellDirection.R,
        HexCellDirection.RD,
        HexCellDirection.DL,
        HexCellDirection.L,
        HexCellDirection.LU,
        HexCellDirection.UR
    };
    
    public static readonly HexCellDirection[] HalfDirections =
    {
        HexCellDirection.LU,
        HexCellDirection.UR,
        HexCellDirection.R,
    };
    
    public static readonly HexCellDirection[] HalfInverseDirections =
    {
        HexCellDirection.RD,
        HexCellDirection.DL,
        HexCellDirection.L,
    };


    public static readonly float GapX = outerRadius/2.0f;
    public static readonly float GapZ = GapX / 2 * Mathf.Sqrt(3);

    public static readonly AABB aabb = new AABB(new Vector3(-innerRadius, 0, -outerRadius),
        new Vector3(innerRadius, 0, outerRadius));


    public static readonly Vector3[] corners =
    {
        new Vector3(outerRadius * Mathf.Cos(30f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(30f * Mathf.Deg2Rad)),
        new Vector3(outerRadius * Mathf.Cos(-30f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(-30f * Mathf.Deg2Rad)),
        new Vector3(outerRadius * Mathf.Cos(-90f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(-90f * Mathf.Deg2Rad)),
        new Vector3(outerRadius * Mathf.Cos(-150f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(-150f * Mathf.Deg2Rad)),
        new Vector3(outerRadius * Mathf.Cos(-210f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(-210f * Mathf.Deg2Rad)),
        new Vector3(outerRadius * Mathf.Cos(-270f * Mathf.Deg2Rad), 0f, outerRadius * Mathf.Sin(-270f * Mathf.Deg2Rad)),
    };
    
    // 偶数行的偏移
    private static readonly (int dx, int dz)[] evenRowOffsets = new (int, int)[]
    {
        ( 1,  0), // R
        ( 0, -1), // RD
        (-1, -1), // DL
        (-1,  0), // L
        (-1,  1), // LU
        ( 0,  1), // UR
    };

    // 奇数行的偏移
    private static readonly (int dx, int dz)[] oddRowOffsets = new (int, int)[]
    {
        ( 1,  0), // R
        ( 1, -1), // RD
        ( 0, -1), // DL
        (-1,  0), // L
        ( 0,  1), // LU
        ( 1,  1), // UR
    };


    public static List<int> LOD0_P0 = new List<int>() { 0,3,6,9};
    public static List<int> LOD0_P1 = new List<int>() { 1,12,17};
    public static List<int> LOD0_P3 = new List<int>() {5,7,22,24,29};
    public static List<int> LOD0_P8 = new List<int>() {20,21,25};

    #region Direction Transform

    public static HexAllDirection ToAllDirection(this HexCellDirection dir)
    {
        return (HexAllDirection)((int)dir * 2);
    }

    // HexCellAllDirection -> HexCellDirection
    public static HexCellDirection ToEdgeDirection(this HexAllDirection allDir)
    {
        return (HexCellDirection)((int)allDir / 2);
    }

    #endregion
    

    #region Get Vertex Info
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetVertexByDirection(HexCellDirection direction)
    {
        int i = (int)direction;
        return corners[i];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetVertexByDirection(HexCellDirection direction, out Vector3 point1, out Vector3 point2)
    {
        int i = (int)direction;
        int j = (i + 1) % 6; 
        point1 = corners[i];
        point2 = corners[j];
    }


    #endregion

    #region aabb
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB GetAABB(Vector3 pos)
    {
        return new AABB(aabb.min + pos, aabb.max + pos);
    }
    

    #endregion
    

    #region Get Neighbors Info

    public static HexCellDirection GetNextDirection(HexCellDirection direction)
    {
        return (HexCellDirection)(((int)direction + 1) % 6);
    }
    
    public static HexCellDirection GetPrevioustDirection(HexCellDirection direction)
    {
        return (HexCellDirection)(((int)direction +5) % 6);
    }

    public static HexCellDirection GetInverseDirection(HexCellDirection direction)
    {
        return (HexCellDirection)(((int)direction + 3) % 6);
    }


    public static bool TryGetNeighborDirection(HexCell a, HexCell b, out HexCellDirection dir)
    {
        var (ax, ay, az) = OffsetToCube(a.hexCellCoords);
        var (bx, by, bz) = OffsetToCube(b.hexCellCoords);

        int dx = bx - ax;
        int dy = by - ay;
        int dz = bz - az;

        if (dx == 1 && dy == -1 && dz == 0) { dir = HexCellDirection.R; return true; }
        if (dx == 1 && dy == 0 && dz == -1) { dir = HexCellDirection.RD; return true; }
        if (dx == 0 && dy == 1 && dz == -1) { dir = HexCellDirection.DL; return true; }
        if (dx == -1 && dy == 1 && dz == 0) { dir = HexCellDirection.L; return true; }
        if (dx == -1 && dy == 0 && dz == 1) { dir = HexCellDirection.LU; return true; }
        if (dx == 0 && dy == -1 && dz == 1) { dir = HexCellDirection.UR; return true; }

        dir = default;
        return false;
    }



    public static HexCellCoords GetHexCellNeighborCoords(HexCell cell, HexCellDirection direction)
    {
        HexCellCoords coords = cell.hexCellCoords;
        var offsets = (coords.z & 1) == 0 ? evenRowOffsets : oddRowOffsets;

        var (dx, dz) = offsets[(int)direction];
        return new HexCellCoords(coords.x + dx, coords.z + dz);
    }
    
    public static HexCellDirection NextDirection(HexCellDirection dir)
    {
        return (HexCellDirection)(((int)dir + 1) % 6);
    }

    
    #endregion


    #region Coords Transform

    /// <summary>
    /// offset(x,z) → cube(x,y,z)
    /// </summary>
    public static (int x, int y, int z) OffsetToCube(HexCellCoords offset)
    {
        int x = offset.x + ((offset.z & 1) - offset.z) / 2;
        int z = offset.z;
        int y = -x - z;
        return (x, y, z);
    }

    /// <summary>
    /// cube(x,y,z) → offset(x,z)
    /// </summary>
    public static HexCellCoords CubeToOffset(int x, int y, int z)
    {
        int ox = x + (z - (z & 1)) / 2;
        int oz = z;
        return new HexCellCoords(ox, oz);
    }

    #endregion
    
   
}