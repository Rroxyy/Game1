using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// [System.Serializable]
public struct HexCellCoords
{
    public int x;
    public int z;

    public HexCellCoords(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public override string ToString()
    {
        return $"({x}, {z})";
    }

    public static implicit operator (int, int)(HexCellCoords c)
    {
        return (c.x, c.z);
    }
}


// [System.Serializable]
public class HexCell
{
    public HexCellCoords HexCellCoords;
    public Vector3 positionWS;
    public Vector3 normal;
    public Color cellColor;

    private HexCellQuadtree hexCellQuadtree;
    public int hexCellChunkMesh_Index { get;private set; }

    public HexCell(HexCellCoords hexCellCoords, Vector3 _positionWS, Color color)
    {
        this.HexCellCoords = hexCellCoords;
        positionWS = _positionWS;
        cellColor = color;
        normal = Vector3.up;
        hexCellChunkMesh_Index = -1;
    }

    public void SetHexMapQuadtree(HexCellQuadtree _quadtree)
    {
        hexCellQuadtree = _quadtree;
    }

    public void SetHexCellChunkMeshIndex(int _chunkIndex)
    {
        hexCellChunkMesh_Index = _chunkIndex;
    }

   
    
    #region Mesh Info
    
    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point1,out Vector3 point2)
    {
        HexCellMetrics.GetVertexByDirection(direction, out var p1,out var p2);
        point1 = p1+positionWS;
        point2 = p2+positionWS;
    }

    public void SetColor(Color _color)
    {
        if (cellColor == _color) return;
        cellColor = _color;
        HexCellMapManager.instance.GetChunk(this).SetDirty(this);
    }

    public void SetHeight(float _height)
    {
        float temp = _height * HexCellMetrics.heightFactor;
        if (Mathf.Abs(positionWS.y - temp) < 1e-4) return;
        positionWS.y = temp;
        HexCellMapManager.instance.GetChunk(this).SetDirty(this);
        if (hexCellQuadtree == null)
        {
            Debug.Log(HexCellCoords + " hex cell quadtree is null");
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


   
}