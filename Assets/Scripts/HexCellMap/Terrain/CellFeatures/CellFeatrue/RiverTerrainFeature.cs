using System.Collections.Generic;

public class RiverTerrainFeature : CellFeature
{
    private HexCellDirection riverDirection;
    
    
    
    public RiverTerrainFeature(CellFeatureType featureType,HexCellDirection _dir) : base(featureType)
    {
        riverDirection = _dir;
    }

    public override void AddFeatureToCellMesh(HexCell cell, int startIndex, LOD_Level lod,
        List<HexCellVertexData> featureBuffer)
    {
        if (lod >= LOD_Level.LOD1) return;
        
    }

    public override void AddFeatureToConnection(HexCell cell, int startIndex, LOD_Level lod, List<HexCellVertexData> featureBuffer)
    {
    }

    public override void AddFeatureToGapTriangle(HexCell cell, int startIndex, LOD_Level lod, List<HexCellVertexData> featureBuffer)
    {
    }
}