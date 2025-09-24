using System.Collections.Generic;
using UnityEngine;

public class RiverTerrainFeature_Connection : TerrainFeature
{
   

    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private CellConnection connection;
    private HashSet<HexAllDirection> riverDirections = new HashSet<HexAllDirection>();

    
    public RiverTerrainFeature_Connection(Cell_Item _cellItem) : base(_cellItem)
    {
        connection=_cellItem as CellConnection;
    }

    
    public override void AddTerrainFeatureParams(params object[] _params)
    {
        if (_params[0]?.GetType() == typeof(HexAllDirection))
        {
            riverDirections.Add((HexAllDirection)_params[0]);
        }
        else if (_params[0]?.GetType() == typeof(Ray))
        {
            if (riverDirections.Count == 4) return;
            connection.GetVertices(out var a2, out var a1,out var b2,out var b1);
            
            var midB=(b1+b2)/2;
            var midA=(a1+a2)/2;
            
            var ray=(Ray)_params[0];
            if (!riverDirections.Contains(HexAllDirection.Deg180)&&
                Quad.RayInQuad(ray, midA, a1, b2, midB, out var _))
            {
                riverDirections.Add(HexAllDirection.Deg180);
            }
            else if (!riverDirections.Contains(HexAllDirection.Deg0) &&
                     Quad.RayInQuad(ray,a2,midA,midB,b1, out var _))
            {
                riverDirections.Add(HexAllDirection.Deg0);
            }
        }
    }


    public override void SetFeatureToMesh(
        List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        
        
        var p3 = vertexBufferList[startIndex + 16];
        vertexBufferList[startIndex + 12] = p3;//p2=p3
        
        var p7=vertexBufferList[startIndex + 13];
        vertexBufferList[startIndex + 15] = p7;//p8=p7
        
        vertexBufferList[startIndex + 30]=p7;
        var p11=vertexBufferList[startIndex + 27];
        vertexBufferList[startIndex + 33] = p11;


        foreach (var dir in riverDirections)
        {
            SetRiverByDirection(dir,vertexBufferList,startIndex);
        }
       
    }

    private void SetRiverByDirection(HexAllDirection direction, List<HexCellVertexData> vertexBufferList,
        int startIndex)
    {
        if (direction == HexAllDirection.Deg90)
        {
            var cellRiver = connection.BelongsToHexCell.terrainFeature as RiverTerrainFeature_Cell;
            if (cellRiver != null)
            {
                SetVertexHeight(vertexBufferList,startIndex+10,cellRiver.GetOuterRiverHeight(connection.direction));
                SetVertexHeight(vertexBufferList,startIndex+17,cellRiver.GetOuterRiverHeight(connection.direction));
            }
        }
        else if (direction == HexAllDirection.Deg270)
        {
            var otherRiver= connection.OtherHexCell.terrainFeature as RiverTerrainFeature_Cell;
            var inDir = HexCellMetrics.GetInverseDirection(connection.direction);
            if (otherRiver != null)
            {
                SetVertexHeight(vertexBufferList,startIndex+32,otherRiver.GetOuterRiverHeight(inDir));
                SetVertexHeight(vertexBufferList,startIndex+37,otherRiver.GetOuterRiverHeight(inDir));
            }
        }
        else if (direction == HexAllDirection.Deg0)
        {
            
        }
    }

    public void SetVertexHeight(List<HexCellVertexData> vertexBufferList, int index, float height)
    {
        var point = vertexBufferList[index];
        point.pos.y = height;
        vertexBufferList[index] = point;
        HexCellMeshOperate.RebuildNormal(vertexBufferList,index/3);
    }
}