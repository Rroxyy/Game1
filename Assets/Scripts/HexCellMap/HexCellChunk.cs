using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCellChunk : MonoBehaviour
{
    [Header("Base")] [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private List<HexCell> cells = new List<HexCell>();


    private List<HexCell> dirtyCells = new List<HexCell>();
    private bool dirty = true;


    [Header("Show Coords")] [SerializeField]
    private bool testShowCoordsUI = false;

    private bool showCoords = false;
    private List<HexCellCoordUI> coordsUI = new List<HexCellCoordUI>();


    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Color> colors;
    private List<int> triangles;


    private Mesh hexMesh;

    private void Awake()
    {
        meshFilter.mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        colors = new List<Color>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (testShowCoordsUI)
        {
            GenerateCoordsUI();
        }
        else
        {
            ClearCoordsUI();
        }

        if (dirty)
        {
            RebuildChunkMesh();
        }
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

    public void SetDirty(HexCell cell)
    {
        dirty = true;
        dirtyCells.Add(cell);
    }

    public void AddHexCell(HexCell _cell)
    {
        dirty = true;
        cells.Add(_cell);
    }

    public void RebuildChunkMesh()
    {
        foreach (HexCell cell in cells)
        {
            cell.SetHexCellChunkMeshIndex(vertices.Count);
            BuildCellMesh(cell);
            BuildCellConnectionMesh(cell);
        }
        
        hexMesh.Clear();
        
        hexMesh.SetVertices(vertices);
        hexMesh.SetNormals(normals);
        hexMesh.SetColors(colors);
        hexMesh.SetTriangles(triangles, 0);
        

        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        colors.Clear();

        RefreshCoordsUI();

        dirty = false;
    }

    private void BuildCellMesh(HexCell cell)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                positionWS,
                HexCellMetrics.corners[i] + positionWS,
                HexCellMetrics.corners[(i + 1) % 6] + positionWS,
                cell.cellColor
            );
        }
    }


    private void BuildCellConnectionMesh(HexCell cell)
    {
        foreach (var dir in HexCellMetrics.HalfDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            if (neighbor == null) continue;


            cell.GetVertexByDirection(dir, out var a1, out var a2);
            neighbor.GetVertexByDirection(HexCellMetrics.GetInverseDirection(dir), out var b1, out var b2);

            AddQuad(a1, a2, b1, b2, cell.cellColor, neighbor.cellColor);
        }
    }


    private void RefreshSingleCellMesh(HexCell cell)
    {
        if (cell.hexCellChunkMesh_Index == -1)
        {
            Debug.LogWarning("RefreshSingleCellMesh called with invalid index");
            return;
        }
        
        BuildCellMesh(cell);
        BuildCellConnectionMesh(cell);
        
        
    }

    #endregion


    #region Mesh Operate

    void AddQuad(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, Color c1, Color c2)
    {
        AddTriangle(a1, b2, b1, c1, c2, c2);
        AddTriangle(b1, a2, a1, c2, c1, c1);
    }


    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
    {
        int start = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);

        Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color c1, Color c2, Color c3)
    {
        int start = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized; // 注意顺序
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    #endregion
}