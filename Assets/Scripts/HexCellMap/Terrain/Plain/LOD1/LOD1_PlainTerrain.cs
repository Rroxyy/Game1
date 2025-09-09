using System.Collections.Generic;
using UnityEngine;

public  class LOD1_PlainTerrain :ITerrainOperate
{
    public void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            HexCellMeshOperate.AddTriangle(
                positionWS,
                HexCellMetrics.corners[i] + positionWS,
                HexCellMetrics.corners[(i + 1) % 6] + positionWS,
                cell.cellColor,
                vertexBufferList
            );
        }
    }

    public void AddConnectionMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        foreach (var connection in cell.cellConnections.Values)
        {
            connection.GetVertices(out var a2,out var a1,out var b2,out var b1);
            HexCellMeshOperate.AddQuad( a2,a1,  b2,b1,
                connection.BelongsToHexCell.cellColor,
                connection.OtherHexCell.cellColor,
                vertexBufferList);
        }
    }

    public void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        foreach (var triangle in cell.cellGapTriangles.Values)
        {
            triangle.GetVertices(out var a,out var b,out var c);
            HexCellMeshOperate.AddTriangle(a, b, c,
                 cell.cellColor,
                 triangle.preDirectionCell.cellColor,
                 triangle.directionCell.cellColor,
                vertexBufferList
                 );
        }
    }
}