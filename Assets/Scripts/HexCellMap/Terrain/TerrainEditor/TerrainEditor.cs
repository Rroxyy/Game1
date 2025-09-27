using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainEditor : MonoBehaviour
{
    [Header("Generate Terrain Mesh")] 
    [SerializeField] private bool showTerrainMesh = false;
    [SerializeField] private bool generateTerrainMesh = false;

    [Header("Save")]
    [SerializeField][TextArea] private string meshName = "";
    private readonly static string path = "Assets/Gen/TerrainMesh/Temp";


    [Space] 
    [SerializeField] private Material terrainMaterial;
    private Mesh terrainMesh;
    private GameObject go;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;


    private static readonly MeshUpdateFlags TerrainMeshUpdateFlag = MeshUpdateFlags.Default;

    private void Update()
    {
        if (generateTerrainMesh)
        {
            generateTerrainMesh = false;

            if (terrainMesh == null) return;
            terrainMesh.tangents = null;
            MeshExporter.SaveMeshAsOBJ(terrainMesh, path);
        }

        if (showTerrainMesh)
        {
            showTerrainMesh = false;

            if (go == null)
            {
                go = new GameObject("TerrainMesh");
                go.transform.SetParent(transform, false);

                meshFilter = go.AddComponent<MeshFilter>();
                terrainMesh = new Mesh { name = meshName.Length==0?"TerrainMesh":meshName };
                meshFilter.sharedMesh = terrainMesh;

                meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = terrainMaterial;

                // === 生成数据 ===
                List<HexCellVertexData> vertexBufferList = new List<HexCellVertexData>();
                List<int> indicesList = new List<int>();
                
                // Plain_LOD0_CellMesh(vertexBufferList, indicesList);
                // Plain_LOD0_ConnectionMesh(vertexBufferList, indicesList);
                Plain_LOD0_GapTriangleMesh(vertexBufferList, indicesList);
                
                // === 应用到 Mesh ===
                terrainMesh.Clear();

                terrainMesh.SetVertexBufferParams(vertexBufferList.Count,
                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.Normal,   VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.Color,    VertexAttributeFormat.UNorm8, 4)
                );

                terrainMesh.SetVertexBufferData(vertexBufferList, 0, 0, vertexBufferList.Count, 0, TerrainMeshUpdateFlag);

                terrainMesh.SetIndexBufferParams(indicesList.Count, IndexFormat.UInt32);
                terrainMesh.SetIndexBufferData(indicesList, 0, 0, indicesList.Count, TerrainMeshUpdateFlag);

                terrainMesh.SetSubMesh(0, new SubMeshDescriptor(0, indicesList.Count, MeshTopology.Triangles), TerrainMeshUpdateFlag);

                terrainMesh.RecalculateBounds();
            }
        }
    }

    #region Plain Terrain

    /// <summary>
    /// Plain
    /// </summary>
   
    private static void Plain_LOD0_CellMesh(List<HexCellVertexData> vertexBufferList,
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

    private static void Plain_LOD0_ConnectionMesh(List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        HexCellMeshOperate.AddQuadSubdivide_AllEdges(
            CellConnectionMetrics.ConnectionCorners[0], CellConnectionMetrics.ConnectionCorners[1],
            CellConnectionMetrics.ConnectionCorners[2], CellConnectionMetrics.ConnectionCorners[3],
            Color.white, Color.white, Color.white, Color.white,
            4, 2,
            Vector3.up,
            vertexBufferList,indicesList
        );

    }
    
    private static void Plain_LOD0_GapTriangleMesh(List<HexCellVertexData> vertexBufferList,
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
    
    

    #endregion

   
}
