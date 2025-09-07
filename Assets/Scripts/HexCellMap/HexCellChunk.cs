using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


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
    
    
    [Header("Test")]
    [SerializeField]private bool gizmosTest = false;

    private bool showCoords = false;
    private List<HexCellCoordUI> coordsUI = new List<HexCellCoordUI>();
    
    
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


    #region Change Mesh Data

    public void SetCellDirty(HexCell cell,bool dirtyCell=true)
    {
        dirtyCells.Add(cell);

        if (dirtyCell)
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

    #region Gizmos

    private void OnDrawGizmos()
    {
        if(!gizmosTest) return;
        
        AABB aabbCollider = HexCellMapManager.instance.root.GetCombinedCollider(aabb_id);
        Vector3 center = aabbCollider.center;
        Vector3 size= aabbCollider.size;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    #endregion

}