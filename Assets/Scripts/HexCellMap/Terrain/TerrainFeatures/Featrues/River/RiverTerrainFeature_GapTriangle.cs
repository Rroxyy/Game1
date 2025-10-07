using System.Collections.Generic;

public class RiverTerrainFeature_GapTriangle : TerrainFeature
{
   

    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private CellBodyDirection _riverBodyDirection;

    public RiverTerrainFeature_GapTriangle(Cell_Item _cellItem) : base(_cellItem)
    {
    }


    public override void SetFeatureToMesh(
        List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        
        
        
    }

   
}