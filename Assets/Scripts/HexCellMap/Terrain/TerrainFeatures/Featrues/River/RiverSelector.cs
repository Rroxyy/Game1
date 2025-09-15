using System;
using System.Collections.Generic;

public class RiverSelector : TerrainFeatureSelector
{
    private static readonly Dictionary<CellItemType, Func<TerrainFeature>> selector =
        new Dictionary<CellItemType, Func<TerrainFeature>>()
        {
            { CellItemType.HexCell ,() => new RiverTerrainFeature_Cell()},
            { CellItemType.Connection, () => new RiverTerrainFeature_Connection() },
            { CellItemType.GapTriangle ,()=>new RiverTerrainFeature_GapTriangle()}
        };

    public override TerrainFeature GetTerrainFeature(CellItemType cellItemType)
    {
        if (selector.TryGetValue(cellItemType, out var factory))
        {
            return factory();
        }
        return null;
    }
}
