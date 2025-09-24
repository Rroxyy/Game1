using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// [System.Serializable]


// [System.Serializable]
public class HexCell : Cell_Item
{
    public override CellItemType itemType => CellItemType.HexCell;

    public HexCellCoords hexCellCoords;

    public Vector3 positionWS;

    // public Vector3 normal;
    public Color cellColor;

    public HexCellCollider hexCellCollider { get; private set; }

    //Terrain
    public TerrainData terrainData { get; private set; }
    public TerrainType terrainType => terrainData.terrainType;

    public HexCellChunk chunk { get; private set; }

    public Dictionary<HexCellDirection, CellConnection> cellConnections { get; private set; }
    public Dictionary<HexCellDirection, CellGapTriangle> cellGapTriangles { get; private set; }


    public HexCell(HexCellCoords hexCellCoords, Vector3 _positionWS, Color color, TerrainData _terrainData)
    {
        this.hexCellCoords = hexCellCoords;
        positionWS = _positionWS;
        cellColor = color;
        // normal = Vector3.up;
        cellMesh_Index = -1;

        terrainData = _terrainData;

        cellConnections = new Dictionary<HexCellDirection, CellConnection>();
        cellGapTriangles = new Dictionary<HexCellDirection, CellGapTriangle>();

        hexCellCollider = new HexCellCollider(this);
    }

    public void SetChunk(HexCellChunk _chunk)
    {
        chunk = _chunk;
    }


    #region Refresh Connection and Gap Triangle

    public void RefreshSingleCell()
    {
        foreach (var dir in HexCellMetrics.HalfDirections)
        {
            RefreshCTbyDir(dir);
        }
    }

    public void RefreshCellAndNeighbor()
    {
        RefreshSingleCell();

        foreach (var dir in HexCellMetrics.AllDirections)
        {
            HexCellMapManager.instance.GetCellNeighbors(this, dir)
                ?.RefreshSingleCell();
        }
    }

    public void RefreshCTbyDir(HexCellDirection dir)
    {
        var neighbor = HexCellMapManager.instance.GetCellNeighbors(this, dir);
        if (neighbor == null) return;
        if (!cellConnections.ContainsKey(dir))
        {
            var connection = new CellConnection(this, neighbor, dir);
            cellConnections.Add(dir, connection);
        }

        if (dir == HexCellMetrics.HalfDirections[0]) return;

        var preDir = HexCellMetrics.GetPrevioustDirection(dir);
        var preNeighbor = HexCellMapManager.instance.GetCellNeighbors(this, preDir);
        if (preNeighbor == null) return;

        if (!cellGapTriangles.ContainsKey(dir))
        {
            var gapTriangle = new CellGapTriangle(this, preNeighbor, neighbor, dir);
            cellGapTriangles.Add(dir, gapTriangle);
        }
    }

    #endregion


    #region Mesh Dirty Operations

    public void SetChunkDirty(bool dirtyNeighborCell = true)
    {
        chunk.SetCellDirty(this, dirtyNeighborCell);
    }

    public void SetColor(Color _color)
    {
        if (cellColor == _color) return;
        cellColor = _color;
        SetChunkDirty(true);
    }

    public void SetHeight(float _height)
    {
        float temp = _height * HexCellMetrics.heightFactor;
        if (Mathf.Abs(positionWS.y - temp) < 1e-4) return;
        positionWS.y = temp;
        SetChunkDirty(true); //render dirty
        hexCellCollider.SetDirty(); //collider dirty
    }

    #endregion


    #region Get Info

    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point)
    {
       ;
        point = HexCellMetrics.GetVertexByDirection(direction) + positionWS;
    }

    /// <summary>
    /// 顺时针返回point
    /// </summary>
    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point1, out Vector3 point2)
    {
        HexCellMetrics.GetVertexByDirection(direction, out var p1, out var p2);
        point1 = p1 + positionWS;
        point2 = p2 + positionWS;
    }

    public bool GetMidConnectionVerticesByDirection(HexCellDirection direction, ref Vector3 a1, ref Vector3 a2,
        ref Vector3 b1, ref Vector3 b2)
    {
        var neighbor = HexCellMapManager.instance.GetCellNeighbors(this, direction);
        if (neighbor == null) return false;
        GetVertexByDirection(direction, out a1, out a2);
        neighbor.GetVertexByDirection(HexCellMetrics.GetInverseDirection(direction), out b1, out b2);

        b1 = (b1 + a2);
        b2 = (b2 + a1);
        return true;
    }

    public CellConnection GetConnectionByDirection(HexCellDirection direction)
    {
        if (direction == HexCellDirection.R || direction == HexCellDirection.LU || direction == HexCellDirection.UR)
        {
            return cellConnections.ContainsKey(direction) ? cellConnections[direction] : null;
        }
        else
        {
            return HexCellMapManager.instance.GetCellNeighbors(this, direction)
                ?.GetConnectionByDirection(HexCellMetrics.GetInverseDirection(direction));
        }
    }

    public CellGapTriangle GetCellGapTriangleByDirection(HexCellDirection direction)
    {
        if (direction == HexCellDirection.UR || direction == HexCellDirection.R)
        {
            return cellGapTriangles.ContainsKey(direction) ? cellGapTriangles[direction] : null;
        }
        else
        {
            return HexCellMapManager.instance.GetCellNeighbors(this, direction)
                ?.GetCellGapTriangleByDirection(
                    HexCellMetrics.GetNextDirection(HexCellMetrics.GetInverseDirection(direction)));
        }
    }

    public Vector3 GetVertexByIndex(int index)
    {
        return HexCellMetrics.corners[index % 6] + positionWS;
    }

    #endregion


    #region Collider

    public AABB GetAABB_Collider()
    {
        return hexCellCollider.GetAABB();
    }

    public bool ContainsCell(Ray ray)
    {
        return TerrainMeshOperate.ContainsCell(this, ray, chunk.GetLOD());
    }

    public bool ContainsConnection(Ray ray, out CellConnection connection)
    {
        return TerrainMeshOperate.ContainsConnection(this, ray, out connection, chunk.GetLOD());
    }

    public bool ContainsGapTriangle(Ray ray, out CellGapTriangle gapTriangle)
    {
        return TerrainMeshOperate.ContainsGapTriangle(this, ray, out gapTriangle, chunk.GetLOD());
    }

    #endregion


    public override string ToString()
    {
        return "HexCell :" + hexCellCoords.ToString();
    }
}