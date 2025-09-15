using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCellChunk : MonoBehaviour
{
    [Header("Base")] 
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private List<HexCell> cells = new List<HexCell>();
    public AABB_Int aabb_id{get; private set;}


    
    
    
    [Header("Mesh")]
    private HexCellMesh hexCellMesh;
    
    private HashSet<HexCell> dirtyCells = new HashSet<HexCell>();
    private bool dirty = true;


    [Header("Show Coords")] 
    [SerializeField] private bool testShowCoordsUI = false;
    private bool showCoords = false;
    private List<HexCellCoordUI> coordsUI = new List<HexCellCoordUI>();

    [Header("AABB Collider")]
    [SerializeField]private bool ShowChunkAABB = false;
    [SerializeField]private bool ShowCellsAABB = false;
    public HexCellQuadtree root{get; private set;}


    [Header("Level of Detail")] 
    // private  LOD_Level lod=LOD_Level.LOD0;
    
    
    [Header("Test")]
    [SerializeField] private bool testShowTest = false;
    
    private void Awake()
    {
        meshFilter.mesh = new Mesh
        {
            name = "Hex Mesh"
        };
        hexCellMesh=new HexCellMesh(this, meshFilter.mesh);
    }

    void Start()
    {
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (testShowCoordsUI)
        {
            GenerateCoordsUI();
        }
        else
        {
            ClearCoordsUI();
        }

        
    }
    

    public void SetUp(AABB_Int _aadd_id)
    {
        aabb_id = _aadd_id;
        root = new HexCellQuadtree(aabb_id,null);
    }


    #region Show Coord Info

    private void RefreshCoordsUI()
    {
        if (!showCoords) return;
        ClearCoordsUI();
        GenerateCoordsUI();
    }

    private void GenerateCoordsUI()
    {
        if (showCoords) return;
        showCoords = true;
        coordsUI.Clear();
        foreach (HexCell cell in cells)
        {
            coordsUI.Add(HexCellCoordShowManager.Instance.GetHexCellCoordUI(cell));
        }
    }

    private void ClearCoordsUI()
    {
        if (!showCoords) return;
        showCoords = false;
        for (int i = 0; i < coordsUI.Count; i++)
        {
            Destroy(coordsUI[i].gameObject);
        }

        coordsUI.Clear();
    }

    #endregion


    #region Render Method

    public void SetCellDirty(HexCell cell,bool dirtyNeighborCell=true)
    {
        dirtyCells.Add(cell);

        if (dirtyNeighborCell)
        {
            foreach (var dir in HexCellMetrics.HalfInverseDirections)
            {
                HexCellMapManager.instance.GetCellNeighbors(cell,dir)?.SetChunkDirty(false);
            }
        }
        

        if (!dirty)
        {
            HexCellMapManager.instance.SetDirtyChunk(this);
        }
        dirty = true;
    }

   

    public void AddHexCell(HexCell _cell)
    {
        cells.Add(_cell);
        root.Insert(_cell.hexCellCollider);
    }

    public void InitializeChunkMesh()
    {
        hexCellMesh.RebuildMesh(cells);

        dirty = false;
        dirtyCells.Clear();
        
    }

    public void RefreshHexCellMesh()
    {
        if (!dirty) return;
        foreach (var cell in dirtyCells)
        {
            hexCellMesh.RebuildSingleCellMesh(cell);
        }
        
        RefreshCoordsUI();

        dirty = false;
        dirtyCells.Clear();
        
        hexCellMesh.RebuildBounds();
    }

    

    
    #endregion

    #region lod

    public LOD_Level GetLOD()
    {
        return HexCellMapManager.instance.GetLevel();
    }

    #endregion
    
    #region Gizmos

    private void OnDrawGizmos()
    {

        if (ShowChunkAABB)
        {
            AABB aabbCollider = root.GetCombinedCollider(aabb_id);
            Vector3 center = aabbCollider.center;
            Vector3 size= aabbCollider.size;
        
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
        }

        if (ShowCellsAABB)
        {
            Gizmos.color = Color.green;
            foreach (var cell in cells)
            {
                var aabb = cell.GetAABB_Collider();
                // var aabb = HexCellMetrics.GetAABB(cell.positionWS);
                Vector3 center = aabb.center;
                Vector3 size= aabb.size;
                Gizmos.DrawWireCube(center, size);
            }
        }

        if (testShowTest)
        {
            Gizmos.color = Color.green;

            var normals = meshFilter.mesh.normals;
            var vertices = meshFilter.mesh.vertices;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                // 顶点位置要转到世界坐标系
                Vector3 worldPos = meshFilter.transform.TransformPoint(vertices[i]);
                Vector3 worldNormal = meshFilter.transform.TransformDirection(normals[i]);

                // 画一条小线段表示法线
                Gizmos.DrawLine(worldPos, worldPos + worldNormal * 0.2f);
            }
        }
    }

    #endregion

}