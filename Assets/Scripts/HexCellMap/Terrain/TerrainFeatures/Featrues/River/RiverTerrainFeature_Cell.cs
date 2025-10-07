using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverTerrainFeature_Cell : TerrainFeature
{
  

    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private CellBodyDirection _riverBodyDirection;
    private HashSet<CellBodyDirection> riverDirections = new HashSet<CellBodyDirection>(); 

    private HexCell cell;

    public RiverTerrainFeature_Cell(Cell_Item _cellItem) : base(_cellItem)
    {
        cell=_cellItem as HexCell;
    }

    public override void  AddTerrainFeatureParams(params object[] _params)
    {

        if (_params is null)
        {
            Debug.LogError("RiverTerrainFeature_Cell.SetTerrainFeature: _params is null");
            return;
        }
        
        Ray _ray = (Ray)_params[0];
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

                    var dd = dir;
                    if (!HexCellMetrics.HalfDirections.Contains(dd))
                    {
                        dd = HexCellMetrics.GetInverseDirection(dd);
                    }
                    connection.terrainFeature.AddTerrainFeatureParams(connection.BelongsToHexCell,dd);
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
                it.pos.y = GetOuterRiverHeight(dir);
                vertexBufferList[index]=it;
                
                HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
            }

            for (int i = 0; i < HexCellMetrics.LOD0_P3.Count; i++)
            {
                int index=startIndex+HexCellMetrics.LOD0_P3[i]+36*(int)dir;
                var it = vertexBufferList[index];
                it.pos.y = GetInnerRiverHeight(dir);

                vertexBufferList[index]=it;
                
                HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
            }
        }
        
        
        
    }


    public float GetOuterRiverHeight(CellBodyDirection bodyDirection)
    {
        float height = cell.positionWS.y;

        if (riverDirections.Contains(bodyDirection))
        {
            height -= RiverTerrainMetrics.RiverHeightFactor;
        }
        return height;
    }

    public float GetInnerRiverHeight(CellBodyDirection bodyDirection)
    {
        float height = cell.positionWS.y;

        if (riverDirections.Contains(bodyDirection))
        {
            height -= RiverTerrainMetrics.RiverHeightFactor/2.0f;
        }
        return height;
    }
   
}