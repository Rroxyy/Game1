




using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HexCellMesh
{
    private HexCellChunk chunk;
    private Mesh mesh;

    private List<HexCellVertexData> vertexBufferList;
    private List<int> indicesList;
    
    private static readonly MeshUpdateFlags 
        HexCellMeshUpdateFlag=MeshUpdateFlags.DontRecalculateBounds 
                              |MeshUpdateFlags.DontValidateIndices
                              // |MeshUpdateFlags.DontNotifyMeshUsers
                              ;

    public HexCellMesh(HexCellChunk _chunk, Mesh _mesh)
    {
        chunk = _chunk;
        mesh = _mesh;
        vertexBufferList = new List<HexCellVertexData>();
        indicesList = new List<int>();

    }
    
    

   

    public void RebuildMesh(List<HexCell> cells)
    {
        
        
        foreach (HexCell cell in cells)
        {
            cell.SetCellMeshIndex(vertexBufferList.Count);
            TerrainMeshOperate.AddCellMesh(cell, vertexBufferList,indicesList, chunk.GetLOD(),cell.GetCellMesh_Index());

            foreach (var connection in cell.cellConnections.Values)
            {
                connection.SetCellMeshIndex(vertexBufferList.Count);
                TerrainMeshOperate.AddConnectionMesh(connection, vertexBufferList, indicesList,chunk.GetLOD(),connection.GetCellMesh_Index());
            }

            foreach (var gapTriangle in cell.cellGapTriangles.Values)
            {
                gapTriangle.SetCellMeshIndex(vertexBufferList.Count);
                TerrainMeshOperate.AddGapTriangleMesh(gapTriangle, vertexBufferList, indicesList,chunk.GetLOD(),gapTriangle.GetCellMesh_Index());
            }
            
        }
    
        mesh.Clear();

        mesh.SetVertexBufferParams(vertexBufferList.Count, 
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal,   VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Color,    VertexAttributeFormat.UNorm8, 4)
        );
    
        mesh.SetVertexBufferData(vertexBufferList, 0, 0, vertexBufferList.Count, 0,HexCellMeshUpdateFlag);
        
        mesh.SetIndexBufferParams(indicesList.Count, IndexFormat.UInt32);
        mesh.SetIndexBufferData(indicesList, 0, 0, indicesList.Count,HexCellMeshUpdateFlag);

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indicesList.Count, MeshTopology.Triangles), HexCellMeshUpdateFlag);

        RebuildBounds();

        vertexBufferList = new List<HexCellVertexData>();
        indicesList = new List<int>();
    }

    public void RebuildSingleCellMesh(HexCell cell)
    {
        int baseIndex=cell.GetCellMesh_Index();
        TerrainMeshOperate.AddCellMesh(cell, vertexBufferList,indicesList, chunk.GetLOD(),cell.GetCellMesh_Index()-baseIndex);

        foreach (var connection in cell.cellConnections.Values)
        {
            TerrainMeshOperate.AddConnectionMesh(connection, vertexBufferList, indicesList,chunk.GetLOD(),connection.GetCellMesh_Index()-baseIndex);
        }

        foreach (var gapTriangle in cell.cellGapTriangles.Values)
        {
            TerrainMeshOperate.AddGapTriangleMesh(gapTriangle, vertexBufferList, indicesList,chunk.GetLOD(),gapTriangle.GetCellMesh_Index()-baseIndex);
        }
        
        mesh.SetVertexBufferData(vertexBufferList, 
            0, 
            cell.GetCellMesh_Index(),
            vertexBufferList.Count,
            0,
            HexCellMeshUpdateFlag);
        
        vertexBufferList.Clear();
        indicesList.Clear();
    }

    

    public void RebuildBounds()
    {
        AABB aabbCollider = chunk.root.GetCombinedCollider(chunk.aabb_id);
        mesh.bounds = new Bounds(aabbCollider.center, aabbCollider.size);
    }
    
    
}