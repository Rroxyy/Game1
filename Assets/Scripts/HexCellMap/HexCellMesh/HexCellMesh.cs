




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
            TerrainMeshOperate.AddCell(cell,vertexBufferList,indicesList,chunk.GetLOD());
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
        TerrainMeshOperate.AddCell(cell, vertexBufferList,indicesList,chunk.GetLOD());
        
        mesh.SetVertexBufferData(vertexBufferList, 
            0, 
            cell.cellMesh_Index,
            vertexBufferList.Count,
            0,
            HexCellMeshUpdateFlag);

        vertexBufferList.Clear();
    }

    

    public void RebuildBounds()
    {
        AABB aabbCollider = chunk.root.GetCombinedCollider(chunk.aabb_id);
        mesh.bounds = new Bounds(aabbCollider.center, aabbCollider.size);
    }
    
    
}