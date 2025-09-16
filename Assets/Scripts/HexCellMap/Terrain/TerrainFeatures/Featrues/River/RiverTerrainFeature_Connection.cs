using System.Collections.Generic;
using UnityEngine;

public class RiverTerrainFeature_Connection : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private HexCellDirection riverDirection;
    private CellConnection connection;
    private HashSet<HexCellAllDirection> connectionDirections = new HashSet<HexCellAllDirection>();

    public override void SetCellItem(Cell_Item _cellItem, Ray _ray)
    {
        base.SetCellItem(_cellItem, _ray);
        connection=_cellItem as CellConnection;
    }

    public override void SetCellItem(Cell_Item _cellItem, HexCellAllDirection _riverDirection)
    {
        base.SetCellItem(_cellItem, _riverDirection);
        connectionDirections.Add(_riverDirection);
    }


    public override void SetFeatureToMesh(
        List<HexCellVertexData> verticesList,int startIndex)
    {
        Vector3 p2 = verticesList[startIndex + CellConnectionMetrics.LOD0_P2[0]].pos;
        Vector3 p3 = verticesList[startIndex + CellConnectionMetrics.LOD0_P3[0]].pos;

        for (int i = 0; i < CellConnectionMetrics.LOD0_P2.Count; i++)
        {
            HexCellMeshOperate.SetVertex(verticesList,startIndex + CellConnectionMetrics.LOD0_P2[i],p3);
        }

        for (int i = 0; i < CellConnectionMetrics.LOD0_P3.Count; i++)
        {
            HexCellMeshOperate.SetVertex(verticesList,startIndex + CellConnectionMetrics.LOD0_P3[i],p2);
        }
        // foreach (var direction in connectionDirections)
        // {
        //     switch (direction)
        //     {
        //         case HexCellAllDirection.Deg90:
        //            
        //     }
        // }
        //
        
    }
}