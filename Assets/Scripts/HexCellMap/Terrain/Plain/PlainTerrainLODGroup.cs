using System.Collections.Generic;

public class PlainTerrainLODGroup : ITerrainLODGroup
{
    private readonly Dictionary<LOD_Level, ITerrainOperate> lods = new()
    {
        { LOD_Level.LOD0 ,new LOD0_PlainTerrain()},
        { LOD_Level.LOD1, new LOD1_PlainTerrain()},
        
    };

    public ITerrainOperate GetLOD(LOD_Level lod) => lods[lod];
}