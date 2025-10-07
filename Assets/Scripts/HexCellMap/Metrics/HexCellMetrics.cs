using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;





public static class HexCellMetrics
{
    public static readonly float outerRadius = 0.8f;
    public static readonly float innerRadius = Mathf.Sqrt(3f) * outerRadius / 2f;

    public static readonly float heightFactor = 0.3f;

    public static readonly CellBodyDirection[] AllDirections =
    {
        CellBodyDirection.R,
        CellBodyDirection.RD,
        CellBodyDirection.DL,
        CellBodyDirection.L,
        CellBodyDirection.LU,
        CellBodyDirection.UR
    };
    
    public static readonly CellBodyDirection[] HalfDirections =
    {
        CellBodyDirection.LU,
        CellBodyDirection.UR,
        CellBodyDirection.R,
    };
    
    public static readonly CellBodyDirection[] HalfInverseDirections =
    {
        CellBodyDirection.RD,
        CellBodyDirection.DL,
        CellBodyDirection.L,
    };


   

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

    public static CellAllDirection ToAllDirection(this CellBodyDirection dir)
    {
        return (CellAllDirection)((int)dir * 2);
    }

    // HexCellAllDirection -> HexCellDirection
    public static CellBodyDirection ToEdgeDirection(this CellAllDirection allDir)
    {
        return (CellBodyDirection)((int)allDir / 2);
    }

    #endregion
    

    #region Get Vertex Info
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetVertexByDirection(CellBodyDirection bodyDirection)
    {
        int i = (int)bodyDirection;
        return corners[i];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetVertexByDirection(CellBodyDirection bodyDirection, out Vector3 point1, out Vector3 point2)
    {
        int i = (int)bodyDirection;
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

    public static CellBodyDirection GetNextDirection(CellBodyDirection bodyDirection)
    {
        return (CellBodyDirection)(((int)bodyDirection + 1) % 6);
    }
    
    public static CellBodyDirection GetPrevioustDirection(CellBodyDirection bodyDirection)
    {
        return (CellBodyDirection)(((int)bodyDirection +5) % 6);
    }

    public static CellBodyDirection GetInverseDirection(CellBodyDirection bodyDirection)
    {
        return (CellBodyDirection)(((int)bodyDirection + 3) % 6);
    }


    public static bool TryGetNeighborDirection(HexCell a, HexCell b, out CellBodyDirection dir)
    {
        var (ax, ay, az) = OffsetToCube(a.hexCellCoords);
        var (bx, by, bz) = OffsetToCube(b.hexCellCoords);

        int dx = bx - ax;
        int dy = by - ay;
        int dz = bz - az;

        if (dx == 1 && dy == -1 && dz == 0) { dir = CellBodyDirection.R; return true; }
        if (dx == 1 && dy == 0 && dz == -1) { dir = CellBodyDirection.RD; return true; }
        if (dx == 0 && dy == 1 && dz == -1) { dir = CellBodyDirection.DL; return true; }
        if (dx == -1 && dy == 1 && dz == 0) { dir = CellBodyDirection.L; return true; }
        if (dx == -1 && dy == 0 && dz == 1) { dir = CellBodyDirection.LU; return true; }
        if (dx == 0 && dy == -1 && dz == 1) { dir = CellBodyDirection.UR; return true; }

        dir = default;
        return false;
    }



    public static HexCellCoords GetHexCellNeighborCoords(HexCell cell, CellBodyDirection bodyDirection)
    {
        HexCellCoords coords = cell.hexCellCoords;
        var offsets = (coords.z & 1) == 0 ? evenRowOffsets : oddRowOffsets;

        var (dx, dz) = offsets[(int)bodyDirection];
        return new HexCellCoords(coords.x + dx, coords.z + dz);
    }
    
    public static CellBodyDirection NextDirection(CellBodyDirection dir)
    {
        return (CellBodyDirection)(((int)dir + 1) % 6);
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