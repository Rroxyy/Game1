using System;
using System.Collections.Generic;

public struct TerrainBuildKey : IEquatable<TerrainBuildKey>
{
    public TerrainType terrainType;
    public LOD_Level lodLevel;
    public CellPart cellPart;

    public TerrainBuildKey(TerrainType _terrainType, LOD_Level _lodLevel, CellPart _cellPart)
    {
        this.terrainType = _terrainType;
        this.lodLevel = _lodLevel;
        this.cellPart = _cellPart;
    }

    public string GetMeshName()
    {
        return $"{terrainType}_{lodLevel}_{cellPart}";
    }
    public string GetMeshAssetName()
    {
        return $"{GetMeshName()}.mesh";
    }

    #region Hash

    public bool Equals(TerrainBuildKey other)
    {
        return terrainType == other.terrainType && lodLevel == other.lodLevel && cellPart == other.cellPart;
    }

    public override bool Equals(object obj)
    {
        return obj is TerrainBuildKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)terrainType, (int)lodLevel, (int)cellPart);
    }

    #endregion
   
}

public class TerrainMeshFactory
{
    public static Dictionary<TerrainBuildKey, Action<List<HexCellVertexData>, List<int>>> buildActions = new()
    {
        {
            new TerrainBuildKey(TerrainType.Plain, LOD_Level.LOD0, CellPart.CellBody),
            PlainBuildMethod.Plain_LOD0_CellBodyMesh
        },
        {
            new TerrainBuildKey(TerrainType.Plain,LOD_Level.LOD0,CellPart.CellConnection),
            PlainBuildMethod.Plain_LOD0_CellConnectionMesh
        },
        {
            new TerrainBuildKey(TerrainType.Plain,LOD_Level.LOD0,CellPart.CellGapTriangle),
            PlainBuildMethod.Plain_LOD0_CellGapTriangleMesh
        },
        {
            new TerrainBuildKey(TerrainType.Plain,LOD_Level.LOD1,CellPart.CellBody),
            PlainBuildMethod.Plain_LOD1_CellBodyMesh
        },
        {
            new TerrainBuildKey(TerrainType.Plain,LOD_Level.LOD1,CellPart.CellConnection),
            PlainBuildMethod.Plain_LOD1_CellConnectionMesh
        },
        {
            new TerrainBuildKey(TerrainType.Plain,LOD_Level.LOD1,CellPart.CellGapTriangle),
            PlainBuildMethod.Plain_LOD1_CellGapTriangleMesh
        }
    };

    
    
    
}