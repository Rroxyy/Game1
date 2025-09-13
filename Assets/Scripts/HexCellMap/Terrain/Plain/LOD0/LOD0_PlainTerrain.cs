using System.Collections.Generic;
using UnityEngine;

public class LOD0_PlainTerrain : ITerrainOperate
{
    public void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList,List<int> indicesList)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
                positionWS,
                cell.GetVertexByIndex(i),
                cell.GetVertexByIndex(i+1),
                cell.cellColor,cell.cellColor,cell.cellColor,
                4,2,
                vertexBufferList,
                indicesList
            );
        }
    }

    public void AddConnectionMesh(HexCell cell, List<HexCellVertexData> vertexBufferList,List<int> indicesList)
    {
        foreach (var connection in cell.cellConnections.Values)
        {
            connection.GetVertices(out var a2,out var a1,out var b2,out var b1);
            Vector3 normal = Vector3.Cross((b2 - a1), (b1 - a1)).normalized;
            
            HexCellMeshOperate.AddQuadSubdivide_AllEdges(a2,a1,b2,b1,
                cell.cellColor,cell.cellColor,connection.OtherHexCell.cellColor,connection.OtherHexCell.cellColor,
                4,2,
                normal,
                vertexBufferList,indicesList
                );
        }
    }

    public void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> vertexBufferList,List<int> indicesList)
    {
        foreach (var triangle in cell.cellGapTriangles.Values)
        {
            triangle.GetVertices(out var a,out var b,out var c);
            HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
                a,b,c,
                cell.cellColor,triangle.preDirectionCell.cellColor,triangle.directionCell.cellColor,
                2,2,
                vertexBufferList,
                indicesList
            );
            
        }
    }

    public bool Contains(HexCell cell,Ray ray)
    {
        Plane plane = new Plane(Vector3.up, cell.positionWS);
        
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return Contains(cell,hitPoint);
        }
        
        
        return false;
    }

    public bool Contains(HexCell cell, Vector3 point)
    {
        Vector3 normal = Vector3.up;

        bool? inside = null;

        for (int i = 0; i < 6; i++)
        {
            Vector3 a = cell.GetVertexByIndex(i);
            Vector3 b = cell.GetVertexByIndex(i+1);;

            Vector3 ab = b - a;
            Vector3 ap = point - a;

            Vector3 cross = Vector3.Cross(ab, ap);

            float dot = Vector3.Dot(cross, normal);

            if (i == 0)
            {
                inside = dot >= 0;
            }
            else
            {
                if ((dot >= 0) != inside.Value)
                    return false;
            }
        }

        return true;
    }
}