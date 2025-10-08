using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR

public class TerrainEditor : MonoBehaviour
{
    [Header("Show Terrain Mesh")] [SerializeField]
    private bool showTerrainMesh = false;
    [SerializeField]private TerrainType terrainType;
    [SerializeField]private LOD_Level lodLevel;
    [SerializeField]private CellPart cellPart;

    [Header("Save Temp(Test)")] [SerializeField]
    private bool generateTerrainMesh = false;

    [SerializeField] [TextArea] string tempName = "TestMesh";

    [Header("Regenerate All Terrain Mesh")] [SerializeField]
    private bool regenerateAllTerrainMesh = false;

    [Header("Load Terrain Mesh")] [SerializeField]
    private bool loadTest = false;


    private readonly static string tempPath = "Assets/Gen/TerrainMesh/Temp";
    

    [Header("Base")] [Space] [SerializeField]
    private Material terrainMaterial;


    private Mesh terrainMesh;
    private GameObject go;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;


    private static readonly MeshUpdateFlags TerrainMeshUpdateFlag = MeshUpdateFlags.Default;


    private void Awake()
    {
        go = new GameObject("TerrainMesh");
        go.transform.SetParent(transform, false);

        meshFilter = go.AddComponent<MeshFilter>();
        terrainMesh = new Mesh { name = tempName };
        meshFilter.sharedMesh = terrainMesh;

        meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
    }

    private void Update()
    {
        if (regenerateAllTerrainMesh)
        {
            FileHelper.ClearFolder(ResourceData.generatePainMeshPath);
            regenerateAllTerrainMesh = false;
            

            foreach (var it in TerrainMeshFactory.buildActions)
            {
                Mesh tempMesh = new Mesh();
                List<HexCellVertexData> vertexBufferList = new List<HexCellVertexData>();
                List<int> indicesList = new List<int>();
                string savePath = Path.Combine(ResourceData.generatePainMeshPath, it.Key.GetMeshAssetName());
                it.Value.Invoke(vertexBufferList, indicesList);
                UpdateTerrainMesh(tempMesh, vertexBufferList, indicesList);
                tempMesh.name = it.Key.GetMeshName();
                FileHelper.SaveAsset(tempMesh, savePath);
            }

        }
        if (generateTerrainMesh)
        {
            generateTerrainMesh = false;

            if (terrainMesh == null) return;

            if (terrainMesh.vertices.Length == 0)
            {
                Debug.LogWarning("No terrain mesh");
                return;
            }

            terrainMesh.tangents = null;
            var savePath = Path.Combine(tempPath, terrainMesh.name + ".mesh");
            FileHelper.SaveAsset(terrainMesh, savePath);
        }

        if (showTerrainMesh)
        {
            showTerrainMesh = false;

            terrainMesh.name = tempName;
            List<HexCellVertexData> vertexBufferList = new List<HexCellVertexData>();
            List<int> indicesList = new List<int>();

            var generateMethod=
                TerrainMeshFactory.
                    buildActions[new TerrainBuildKey(terrainType, lodLevel,cellPart)] ;
            generateMethod.Invoke(vertexBufferList, indicesList);

            UpdateTerrainMesh(terrainMesh,vertexBufferList, indicesList);
        }

        if (loadTest)
        {
            loadTest = false;
            string abName = GetTerrainAbName(TerrainType.Plain, LOD_Level.LOD0, CellPart.CellConnection);
            string path = Path.Combine(ResourceData.TerrainAbPath, nameof(TerrainType.Plain));
            terrainMesh = ResourceManager.LoadAsset<Mesh>(path, abName);
            meshFilter.sharedMesh = terrainMesh;
        }
    }

    private void UpdateTerrainMesh(Mesh mesh,List<HexCellVertexData> vertexBufferList, List<int> indicesList)
    {
        // === 应用到 Mesh ===
        mesh.Clear();

        mesh.SetVertexBufferParams(vertexBufferList.Count,
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4)
        );

        mesh.SetVertexBufferData(vertexBufferList, 0, 0, vertexBufferList.Count, 0, TerrainMeshUpdateFlag);

        mesh.SetIndexBufferParams(indicesList.Count, IndexFormat.UInt32);
        mesh.SetIndexBufferData(indicesList, 0, 0, indicesList.Count, TerrainMeshUpdateFlag);

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indicesList.Count, MeshTopology.Triangles),
            TerrainMeshUpdateFlag);

        mesh.RecalculateBounds();
    }


    //Asset Bundles
    public static string GetTerrainAbName(TerrainType type, LOD_Level lodLevel, CellPart section)
    {
        return $"{type}_{lodLevel}_{section}Mesh.mesh";
    }
}

#endif