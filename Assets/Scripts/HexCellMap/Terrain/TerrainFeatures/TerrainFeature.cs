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

    // public virtual void SetTerrainFeature(Cell_Item _cellItem,Ray _ray)
    // {   
    //     cellItem = _cellItem;
    // }
    // public virtual void SetTerrainFeature(Cell_Item _cellItem,HexAllDirection _direction)
    // {   
    //     cellItem = _cellItem;
    // }
    
    public virtual void AddTerrainFeatureParams(Cell_Item _cellItem,params object[] _params)
    {   
        cellItem = _cellItem;
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