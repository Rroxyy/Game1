




using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HexCellMesh
{
    private HexCellChunk chunk;
    private Mesh mesh;

    private List<CellVertexData> vertexBufferList;
    private List<Int16> indicesList;
    
    private static readonly MeshUpdateFlags 
        HexCellMeshUpdateFlag=MeshUpdateFlags.DontRecalculateBounds 
                              |MeshUpdateFlags.DontValidateIndices
                              // |MeshUpdateFlags.DontNotifyMeshUsers
                              ;
    
    
    public HexCellMesh(HexCellChunk _chunk, Mesh _mesh)
    {
        chunk = _chunk;
        mesh = _mesh;
        vertexBufferList = new List<CellVertexData>();
        indicesList = new List<Int16>();
    }
    
    

   

    public void RebuildMesh(List<HexCell> cells)
    {
        
    
        foreach (HexCell cell in cells)
        {
            cell.SetCellMeshIndex(vertexBufferList.Count);
            HexCellMeshOperate.AddCell(cell,vertexBufferList);
            
           
        }
    
        mesh.Clear();

        mesh.SetVertexBufferParams(vertexBufferList.Count, 
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal,   VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Color,    VertexAttributeFormat.UNorm8, 4)
        );
    
        mesh.SetVertexBufferData(vertexBufferList, 0, 0, vertexBufferList.Count, 0,HexCellMeshUpdateFlag);

        indicesList = new List<Int16>(vertexBufferList.Count);
        for (int i = 0; i < vertexBufferList.Count; i++)
            indicesList.Add((Int16)i);

        mesh.SetIndexBufferParams(indicesList.Count, IndexFormat.UInt16);
        mesh.SetIndexBufferData(indicesList, 0, 0, indicesList.Count,HexCellMeshUpdateFlag);

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indicesList.Count, MeshTopology.Triangles), HexCellMeshUpdateFlag);

        RebuildBounds();
        
        vertexBufferList = new List<CellVertexData>();
        indicesList = new List<Int16>();
    }

    public void RebuildSingleCellMesh(HexCell cell)
    {
        HexCellMeshOperate.AddCell(cell, vertexBufferList);
        
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
        AABB aabbCollider = HexCellMapManager.instance.root.GetCombinedCollider(chunk.aabb_id);
        mesh.bounds = new Bounds(aabbCollider.center, aabbCollider.size);
    }
    
    
}