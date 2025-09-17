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

   

    public static void AddCellMesh(HexCell cell, List<HexCellVertexData> verticesList, List<int> indicesList, LOD_Level lodLevel,int startIndex)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddCellMesh(cell, verticesList,indicesList);
        cell.terrainFeature?.SetFeatureToMesh(verticesList,startIndex);
    }

    public static void AddConnectionMesh(CellConnection connection ,List<HexCellVertexData> verticesList,  List<int> indicesList,LOD_Level lodLevel,int startIndex)
    {
        _groups[connection.BelongsToHexCell.terrainType].GetLOD(lodLevel).AddConnectionMesh(connection, verticesList,indicesList,startIndex);
        connection.terrainFeature?.SetFeatureToMesh(verticesList,startIndex);

    }

    public static void AddGapTriangleMesh(CellGapTriangle gapTriangle, List<HexCellVertexData> verticesList,  List<int> indicesList,LOD_Level lodLevel,int startIndex)
    {
        _groups[gapTriangle.belongToCell.terrainType].GetLOD(lodLevel).AddGapTriangleMesh(gapTriangle, verticesList,indicesList);
        gapTriangle.terrainFeature?.SetFeatureToMesh(verticesList,startIndex);
    }

    public static bool ContainsCell(HexCell cell, Ray ray,LOD_Level lodLevel)
    {
        return _groups[cell.terrainType].GetLOD(lodLevel).ContainsCell(cell, ray);
    }

    public static bool ContainsConnection(HexCell cell, Ray ray, out CellConnection hitConnection, LOD_Level lodLevel)
    {
        return _groups[cell.terrainType].GetLOD(lodLevel).ContainsConnection(cell, ray, out hitConnection);
    }
    
    public static bool ContainsGapTriangle(HexCell cell, Ray ray, out CellGapTriangle hitGapTriangle, LOD_Level lodLevel)
    {
        return _groups[cell.terrainType].GetLOD(lodLevel).ContainsGapTriangle(cell, ray, out hitGapTriangle);
    }

   
}