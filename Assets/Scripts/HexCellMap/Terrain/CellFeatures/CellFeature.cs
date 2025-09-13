using System.Collections.Generic;


public enum CellFeatureType
{
    HexCell,
    Connection,
    GapTriangle
}

public abstract class CellFeature {
    protected CellFeatureType featureType;

    protected CellFeature(CellFeatureType featureType) {
        this.featureType = featureType;
    }
    public abstract void AddFeatureToCellMesh(HexCell cell, int startIndex,LOD_Level lod,List<HexCellVertexData> featureBuffer);
    public abstract void AddFeatureToConnection(HexCell cell, int startIndex,LOD_Level lod,List<HexCellVertexData> featureBuffer);
    public abstract void AddFeatureToGapTriangle(HexCell cell, int startIndex,LOD_Level lod,List<HexCellVertexData> featureBuffer);

    
}