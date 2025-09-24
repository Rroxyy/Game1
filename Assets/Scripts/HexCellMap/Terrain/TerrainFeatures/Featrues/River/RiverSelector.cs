using System;
using System.Collections.Generic;

public class RiverSelector : TerrainFeatureSelector
{
    private static readonly Dictionary<CellItemType, Func<Cell_Item,TerrainFeature>> selector =
        new Dictionary<CellItemType, Func<Cell_Item,TerrainFeature>>()
        {
            { CellItemType.HexCell ,(Cell_Item item) => new RiverTerrainFeature_Cell(item)},
            { CellItemType.Connection, (Cell_Item item) => new RiverTerrainFeature_Connection(item) },
            { CellItemType.GapTriangle ,(Cell_Item item)=>new RiverTerrainFeature_GapTriangle(item)}
        };

    public override TerrainFeature GetTerrainFeature(Cell_Item item)
    {
        if (selector.TryGetValue(item.itemType, out var factory))
        {
            return factory(item);
        }
        return null;
    }
}
