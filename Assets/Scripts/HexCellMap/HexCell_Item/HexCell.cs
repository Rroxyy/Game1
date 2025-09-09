using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// [System.Serializable]



// [System.Serializable]
public class HexCell
{
    public HexCellCoords hexCellCoords;
    public Vector3 positionWS;
    public Vector3 normal;
    public Color cellColor;

    public TerrainType terrainType { get; private set; }

    private HexCellChunk chunk;
    private HexCellQuadtree hexCellQuadtree;

    public Dictionary<HexCellDirection, CellConnection> cellConnections { get; private set; }
    public Dictionary<HexCellDirection, CellGapTriangle> cellGapTriangles { get; private set; }
    public int cellMesh_Index { get; private set; }

    public HexCell(HexCellCoords hexCellCoords, Vector3 _positionWS, Color color, TerrainData _terrainData)
    {
        this.hexCellCoords = hexCellCoords;
        positionWS = _positionWS;
        cellColor = color;
        normal = Vector3.up;
        cellMesh_Index = -1;

        cellConnections = new Dictionary<HexCellDirection, CellConnection>();
        cellGapTriangles = new Dictionary<HexCellDirection, CellGapTriangle>();
    }

    public void SetChunk(HexCellChunk _chunk)
    {
        chunk = _chunk;
    }

    public void SetHexMapQuadtree(HexCellQuadtree _quadtree)
    {
        hexCellQuadtree = _quadtree;
    }

    public void SetCellMeshIndex(int _chunkIndex)
    {
        cellMesh_Index = _chunkIndex;
    }

    #region Gap Triangle and Cell Connection

    public void RefreshAllConnectionsAndTriangles()
    {
        foreach (var dir in HexCellMetrics.HalfDirections)
        {
            RefreshSingleConnectionAndTriangle(dir);
        }

        foreach (var dir in HexCellMetrics.HalfInverseDirections)
        {
            HexCellMapManager.instance.GetCellNeighbors(this, dir)
                ?.RefreshSingleConnectionAndTriangle(HexCellMetrics.GetInverseDirection(dir));
        }
    }

    public void RefreshSingleConnectionAndTriangle(HexCellDirection dir)
    {
        var neighbor = HexCellMapManager.instance.GetCellNeighbors(this, dir);
        if (neighbor == null) return;
        if (!cellConnections.ContainsKey(dir))
        {
            var connection = new CellConnection(this, neighbor, dir);
            cellConnections.Add(dir, connection);
        }


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


    #region Mesh Info

    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point)
    {
        HexCellMetrics.GetVertexByDirection(direction, out var p1);
        point = p1 + positionWS;
    }

    /// <summary>
    /// 顺时针返回points
    /// </summary>
    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point1, out Vector3 point2)
    {
        HexCellMetrics.GetVertexByDirection(direction, out var p1, out var p2);
        point1 = p1 + positionWS;
        point2 = p2 + positionWS;
    }

    public void SetChunkDirty(bool dirtyCell = true)
    {
        chunk.SetCellDirty(this, dirtyCell);
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
        SetChunkDirty(true);
        if (hexCellQuadtree == null)
        {
            Debug.Log(hexCellCoords + " hex cell quadtree is null");
            return;
        }

        hexCellQuadtree.AABBCollider_Dirty();
    }

    #endregion


    #region Collider

    public AABB GetAABB_Collider()
    {
        return HexCellMetrics.GetAABB(positionWS);
    }

    public bool Contains(Ray ray)
    {
        float denom = Vector3.Dot(normal, ray.direction);
        if (Mathf.Abs(denom) < 1e-6f)
            return false;

        float t = Vector3.Dot(normal, positionWS - ray.origin) / denom;

        if (t < 0f) return false;

        Vector3 point = ray.origin + t * ray.direction;

        return Contains(point);
    }


    public bool Contains(Vector3 point)
    {
        Vector3 p = point - positionWS;
        return HexCellMetrics.ContainsPoint(p);
    }

    #endregion

    public override string ToString()
    {
        return hexCellCoords.ToString();
    }
}