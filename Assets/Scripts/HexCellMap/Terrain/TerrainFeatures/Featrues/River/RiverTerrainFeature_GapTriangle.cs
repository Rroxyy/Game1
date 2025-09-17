using System.Collections.Generic;

public class RiverTerrainFeature_GapTriangle : TerrainFeature
{
    public override TerrainFeatureType featureType => TerrainFeatureType.River;

    private HexCellDirection riverDirection;




    public override void SetFeatureToMesh(
        List<HexCellVertexData> vertexBufferList,int startIndex)
    {
        
        
        
    }

   
}