using System.Collections.Generic;
using UnityEngine;

public static class TerrainMeshOperate
{

    private static readonly Dictionary<TerrainType, ITerrainLODGroup> _groups;

    static TerrainMeshOperate()
    {
        _groups = new Dictionary<TerrainType, ITerrainLODGroup>
        {
            { TerrainType.Plain, new PlainTerrainLODGroup() },
        };
    }

    public static void AddCell(HexCell cell, List<HexCellVertexData> verticesList, List<int> indicesList,LOD_Level lodLevel)
    {
        AddCellMesh(cell, verticesList,indicesList, lodLevel);
        AddConnectionMesh(cell, verticesList, indicesList,lodLevel);
        AddGapTriangleMesh(cell, verticesList, indicesList,lodLevel);
    }

    public static void AddCellMesh(HexCell cell, List<HexCellVertexData> verticesList, List<int> indicesList, LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddCellMesh(cell, verticesList,indicesList);
    }

    public static void AddConnectionMesh(HexCell cell, List<HexCellVertexData> verticesList,  List<int> indicesList,LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddConnectionMesh(cell, verticesList,indicesList);
    }

    public static void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> verticesList,  List<int> indicesList,LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddGapTriangleMesh(cell, verticesList,indicesList);
    }

    public static bool Contains(HexCell cell, Ray ray,LOD_Level lodLevel)
    {
        return _groups[cell.terrainType].GetLOD(lodLevel).Contains(cell, ray);
    }

    public static bool Contains(HexCell cell, Vector3 point, LOD_Level lodLevel)
    {
        return _groups[cell.terrainType].GetLOD(lodLevel).Contains(cell, point);
    }
}