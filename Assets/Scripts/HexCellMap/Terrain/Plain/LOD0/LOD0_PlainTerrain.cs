using System.Collections.Generic;
using UnityEngine;

public class LOD0_PlainTerrain : ITerrainOperate
{
    public void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
                positionWS,
                HexCellMetrics.corners[i] + positionWS,
                HexCellMetrics.corners[(i + 1) % 6] + positionWS,
                cell.cellColor,
                4,3,
                vertexBufferList
            );
        }
    }

    public void AddConnectionMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        
    }

    public void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> vertexBufferList)
    {
        
        
    }
}