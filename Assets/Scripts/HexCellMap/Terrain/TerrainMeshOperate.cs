using System.Collections.Generic;

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

    public static void AddCell(HexCell cell, List<HexCellVertexData> verticesList, LOD_Level lodLevel)
    {
        AddCellMesh(cell, verticesList, lodLevel);
        AddConnectionMesh(cell, verticesList, lodLevel);
        AddGapTriangleMesh(cell, verticesList, lodLevel);
    }

    public static void AddCellMesh(HexCell cell, List<HexCellVertexData> verticesList, LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddCellMesh(cell, verticesList);
    }

    public static void AddConnectionMesh(HexCell cell, List<HexCellVertexData> verticesList, LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddConnectionMesh(cell, verticesList);
    }

    public static void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> verticesList, LOD_Level lodLevel)
    {
        _groups[cell.terrainType].GetLOD(lodLevel).AddGapTriangleMesh(cell, verticesList);
    }

}