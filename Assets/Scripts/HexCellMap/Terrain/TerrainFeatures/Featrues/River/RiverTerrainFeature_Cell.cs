using System.Collections.Generic;
using UnityEngine;

public class RiverTerrainFeature_Cell : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private HexCellDirection riverDirection;
    private HashSet<HexCellDirection> riverDirections = new HashSet<HexCellDirection>(); 

    private HexCell cell;



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
                var connection = cell.GetConnectionByDirection(dir);
                if (connection != null)
                {
                    connection.SetTerrainFeature(TerrainFeatureType.River);
                    connection.terrainFeature.SetCellItem(connection.BelongsToHexCell,
                        connection.BelongsToHexCell==cell?HexCellAllDirection.Deg90:HexCellAllDirection.Deg270);
                }
                return;
            }
        }
        
    }

    public override void SetFeatureToMesh(List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        foreach (var dir in riverDirections)
        {
            for (int i = 0; i < HexCellMetrics.LOD0_P8.Count; i++)
            {
                int index=startIndex+HexCellMetrics.LOD0_P8[i]+36*(int)dir;
                var it = vertexBufferList[index];
                it.pos.y = cell.positionWS.y - RiverTerrainMetrics.RiverHeightFactor/2.0f;
                vertexBufferList[index]=it;
                
                HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
            }

            for (int i = 0; i < HexCellMetrics.LOD0_P3.Count; i++)
            {
                int index=startIndex+HexCellMetrics.LOD0_P3[i]+36*(int)dir;
                var it = vertexBufferList[index];
                it.pos.y = cell.positionWS.y - RiverTerrainMetrics.RiverHeightFactor/4.0f;

                vertexBufferList[index]=it;
                
                HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
            }
        }
        
        
        
    }

   
}