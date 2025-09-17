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
        List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        var p3 = vertexBufferList[startIndex + 16];
        vertexBufferList[startIndex + 12] = p3;

        var p7=vertexBufferList[startIndex + 13];
        vertexBufferList[startIndex + 15] = p7;



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