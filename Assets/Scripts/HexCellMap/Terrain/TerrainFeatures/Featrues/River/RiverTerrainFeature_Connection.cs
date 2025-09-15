using System.Collections.Generic;

public class RiverTerrainFeature_Connection : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private HexCellDirection riverDirection;




    public override void SetFeatureToMesh(
        List<HexCellVertexData> verticesList,int startIndex)
    {
        
        
        
    }

   
}