using System.Collections.Generic;
using UnityEngine;

public struct CellVertexData
{
    public Vector3 pos;
    public Vector3 normal;
    public Color32 color;

    public CellVertexData(Vector3 pos, Vector3 normal, Color32 color)
    {
        this.pos = pos;
        this.normal = normal;
        this.color = color;
    }
    
}


public static class HexCellMeshOperate
{
    public static void AddCell(HexCell cell, List<CellVertexData> vertexBufferList)
    {
        AddCellMesh(cell, vertexBufferList);
        AddCellConnectionMesh(cell, vertexBufferList);
    }
    
    public static void AddCellConnectionMesh(HexCell cell, List<CellVertexData> vertexBufferList)
    {
        foreach (var dir in HexCellMetrics.HalfDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            if (neighbor == null) continue;


            cell.GetVertexByDirection(dir, out var a1, out var a2);
            neighbor.GetVertexByDirection(HexCellMetrics.GetInverseDirection(dir), out var b1, out var b2);

            AddQuad(a1, a2, b1, b2, cell.cellColor, neighbor.cellColor, vertexBufferList);
        }
    }
    
    public static void AddCellMesh(HexCell cell, List<CellVertexData> vertexBufferList)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                positionWS,
                HexCellMetrics.corners[i] + positionWS,
                HexCellMetrics.corners[(i + 1) % 6] + positionWS,
                cell.cellColor,
                vertexBufferList
            );
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    public static void  AddQuad(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, Color32 c1, Color32 c2, List<CellVertexData> vertexBufferList)
    {
        AddTriangle(a1, b2, b1, c1, c2, c2, vertexBufferList);
        AddTriangle(b1, a2, a1, c2, c1, c1, vertexBufferList);
    }
    public static void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color32 color, List<CellVertexData> vertexBufferList)=> AddTriangle(v1, v2, v3, color,color,color, vertexBufferList);
    
    public static void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color32 c1, Color32 c2, Color32 c3, List<CellVertexData> vertexBufferList)
    {
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
        AddSingleVertex(v1, normal, c1, vertexBufferList);
        AddSingleVertex(v2, normal, c2, vertexBufferList);
        AddSingleVertex(v3, normal, c3, vertexBufferList);
        
    }

    
    public static void AddSingleVertex(Vector3 pos, Vector3 normal, Color32 color, List<CellVertexData> vertexBufferList)
    {
        vertexBufferList.Add(new CellVertexData(pos,normal,color));
    }
    
    
}