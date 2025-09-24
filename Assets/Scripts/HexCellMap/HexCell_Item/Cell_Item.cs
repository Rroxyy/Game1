
using System.Collections.Generic;

public enum CellItemType
{
    HexCell,
    Connection,
    GapTriangle
}

public abstract class Cell_Item
{
    public abstract CellItemType itemType { get; }
    public TerrainFeature terrainFeature{get;private set;}
    protected int cellMesh_Index;


    public virtual void SetTerrainFeature(TerrainFeatureType terrainFeatureType)
    {
        if (terrainFeature == null||terrainFeature.featureType != terrainFeatureType)
        {
            terrainFeature = TerrainFeatureHelper.GetTerrainFeature(terrainFeatureType,this);
        }
       
    }

    public int GetCellMesh_Index()
    {
        return cellMesh_Index;
    }
    
    public void SetCellMeshIndex(int _chunkIndex)
    {
        cellMesh_Index = _chunkIndex;
    }
}