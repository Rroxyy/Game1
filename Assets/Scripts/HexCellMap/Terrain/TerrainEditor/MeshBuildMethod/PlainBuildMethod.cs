using System.Collections.Generic;
using UnityEngine;

public static class PlainBuildMethod
{

    public static void Plain_LOD0_CellBodyMesh(List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        foreach (var dir in HexCellMetrics.AllDirections)
        {
            HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
                Vector3.zero,
                HexCellMetrics.GetVertexByDirection(dir),
                HexCellMetrics.GetVertexByDirection(HexCellMetrics.GetNextDirection(dir)),
                Color.white, Color.white, Color.white,
                4, 2,
                vertexBufferList,
                indicesList
            );
        }
    }

    public static void Plain_LOD0_CellConnectionMesh(List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        HexCellMeshOperate.AddQuadSubdivide_AllEdges(
            CellConnectionMetrics.ConnectionCorners[0], CellConnectionMetrics.ConnectionCorners[1],
            CellConnectionMetrics.ConnectionCorners[2], CellConnectionMetrics.ConnectionCorners[3],
            Color.white, Color.white, Color.white, Color.white,
            4, 2,
            Vector3.up,
            vertexBufferList, indicesList
        );
    }

    public static void Plain_LOD0_CellGapTriangleMesh(List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
            CellGapTriangleMetrics.CellGapTriangleCorners[0], CellGapTriangleMetrics.CellGapTriangleCorners[1],
            CellGapTriangleMetrics.CellGapTriangleCorners[2],
            Color.white, Color.white, Color.white,
            2, 2,
            vertexBufferList,
            indicesList
        );
    }

    public static void Plain_LOD1_CellBodyMesh(List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        for (int i = 0; i < 6; i++)
        {
            HexCellMeshOperate.AddTriangle(
                Vector3.zero,
                HexCellMetrics.corners[i],
                HexCellMetrics.corners[(i + 1) % 6],
                Color.white,
                vertexBufferList
            );
        }

        for (int i = 0; i < 18; i++)
        {
            indicesList.Add(indicesList.Count);
        }
    }

}