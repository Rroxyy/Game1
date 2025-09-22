using System.Collections.Generic;
using UnityEngine;

public class RiverTerrainFeature_Connection : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private CellConnection connection;
    private HashSet<HexAllDirection> riverDirections = new HashSet<HexAllDirection>();


    
    public override void AddTerrainFeatureParams(Cell_Item _cellItem,params object[] _params)
    {
        base.AddTerrainFeatureParams(_cellItem, _params);
        connection=_cellItem as CellConnection;
        if (_params[0]?.GetType() == typeof(HexAllDirection))
        {
            riverDirections.Add((HexAllDirection)_params[0]);
        }
    }


    public override void SetFeatureToMesh(
        List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        
        var p3 = vertexBufferList[startIndex + 16];
        vertexBufferList[startIndex + 12] = p3;//p2=p3
        
        var p7=vertexBufferList[startIndex + 13];
        vertexBufferList[startIndex + 15] = p7;//p8=p7

        if (connection == null) return;
        if (connection.BelongsToHexCell == null) return;
        if (connection.BelongsToHexCell.terrainFeature == null) return;
        var cellRiver = connection.BelongsToHexCell.terrainFeature as RiverTerrainFeature_Cell;
        
        if (cellRiver != null)
        {
           SetVertexHeight(vertexBufferList,10,cellRiver.GetOuterRiverHeight(connection.direction));
           SetVertexHeight(vertexBufferList,17,cellRiver.GetOuterRiverHeight(connection.direction));
        
        }
        
        
        vertexBufferList[startIndex + 30]=p7;
        
        var p11=vertexBufferList[startIndex + 27];
        vertexBufferList[startIndex + 33] = p11;
        
        
        
    }

    public void SetVertexHeight(List<HexCellVertexData> vertexBufferList, int index, float height)
    {
        var point = vertexBufferList[index];
        point.pos.y = height;
        vertexBufferList[index] = point;
        HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
    }
}