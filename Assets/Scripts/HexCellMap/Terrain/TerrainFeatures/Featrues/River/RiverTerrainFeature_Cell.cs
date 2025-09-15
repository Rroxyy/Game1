using System.Collections.Generic;
using UnityEngine;

public class RiverTerrainFeature_Cell : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private HexCellDirection riverDirection;
    private HashSet<HexCellDirection> riverDirections = new HashSet<HexCellDirection>(); 

    private HexCell cell;

    public static List<int> farIndices = new List<int>(){20,21,25};
    public static List<int> nearIndices = new List<int>(){20,21,25};

    public override void SetCellItem(Cell_Item _cellItem, Ray _ray)
    {
        base.SetCellItem(_cellItem, _ray);
        cell = _cellItem as HexCell;
        if (cell == null)
        {
            Debug.LogError("Error: transform to hex cell failed");
            return;
        }
        Vector3 centor = cell.positionWS;
        foreach (var dir in HexCellMetrics.AllDirections)
        {
            cell.GetVertexByDirection(dir,out var a1,out var a2);
            if (Triangle.RayInTriangle(_ray, centor, a1, a2, out var t))
            {
                riverDirections.Add(dir);
                return;
            }
        }
        
    }

    public override void SetFeatureToMesh(
        List<HexCellVertexData> verticesList,int startIndex)
    {
        for (int i = 0; i < farIndices.Count; i++)
        {
            int index=startIndex+farIndices[i];
            var it = verticesList[index];
            it.pos.y-=HexCellMetrics.heightFactor;
            verticesList[index]=it;
        }
        
        
    }

   
}