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

    #region operator
    
    public bool Equals(HexCellCoords other)
    {
        return x == other.x && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is HexCellCoords other && Equals(other);
    }

    
    public static HexCellCoords operator /(HexCellCoords a, (int ,int) p)
    {
        return new HexCellCoords(a.x / p.Item1, a.z / p.Item2);
    }

    public static HexCellCoords operator /(HexCellCoords a, int d)
    {
        return new HexCellCoords(a.x / d, a.z / d);
    }
    
    public static HexCellCoords operator *(HexCellCoords a, int d)
    {
        return new HexCellCoords(a.x * d, a.z * d);
    }
    

    public static implicit operator (int, int)(HexCellCoords c)
    {
        return (c.x, c.z);
    }
    
    public static implicit operator HexCellCoords((int, int) t)
    {
        return new HexCellCoords(t.Item1, t.Item2);
    }

    #endregion
   
    
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + x.GetHashCode();
            hash = hash * 31 + z.GetHashCode();
            return hash;
        }
    }
}


// [System.Serializable]
public class HexCell
{
    public HexCellCoords HexCellCoords;
    public Vector3 positionWS;
    public Vector3 normal;
    public Color cellColor;
    
    private HexCellChunk chunk;
    private HexCellQuadtree hexCellQuadtree;
    public int cellMesh_Index { get;private set; }

    public HexCell(HexCellCoords hexCellCoords, Vector3 _positionWS, Color color)
    {
        this.HexCellCoords = hexCellCoords;
        positionWS = _positionWS;
        cellColor = color;
        normal = Vector3.up;
        cellMesh_Index = -1;
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

    
    
    #region Mesh Info
    
    public void GetVertexByDirection(HexCellDirection direction, out Vector3 point1,out Vector3 point2)
    {
        HexCellMetrics.GetVertexByDirection(direction, out var p1,out var p2);
        point1 = p1+positionWS;
        point2 = p2+positionWS;
    }

    public void SetChunkDirty(bool dirtyCell = true)
    {
        chunk.SetCellDirty(this,dirtyCell);
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
            Debug.Log(HexCellCoords + " hex cell quadtree is null");
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


   
}