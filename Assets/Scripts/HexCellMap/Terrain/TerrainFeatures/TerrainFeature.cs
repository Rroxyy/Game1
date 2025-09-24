using System.Collections.Generic;
using UnityEngine;

public enum TerrainFeatureType
{
    River,
    Road,
    
}

public abstract class TerrainFeature {
    public abstract TerrainFeatureType featureType { get; }
    protected Cell_Item cellItem;

    public  TerrainFeature(Cell_Item _cellItem)
    {
        cellItem = _cellItem;
    }
    
    
    public virtual void AddTerrainFeatureParams(params object[] _params)
    {   
    }
    
    public virtual void SetCellItem(Cell_Item _cellItem)
    {   
        cellItem = _cellItem;
    }

    public Cell_Item GetCellItem()
    {
        return cellItem;
    }
    
    public abstract void SetFeatureToMesh(List<HexCellVertexData> vertexBufferList,int startIndex);
   
}