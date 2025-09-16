using System;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainFeatureHelper
{
    public static Dictionary<TerrainFeatureType, TerrainFeatureSelector> FeatureFactories =
        new Dictionary<TerrainFeatureType, TerrainFeatureSelector>()
        {
            { TerrainFeatureType.River ,new RiverSelector()}
            
        };

    public static TerrainFeature GetTerrainFeature(TerrainFeatureType terrainFeatureType, CellItemType cellItemType)
        => FeatureFactories[terrainFeatureType].GetTerrainFeature(cellItemType);

    public static void SetTerrainFeature(HexCell cell, TerrainFeatureType terrainFeatureType, Ray ray)
    {
        
        if (cell.ContainsCell(ray))
        {
            cell.SetTerrainFeature(terrainFeatureType);
            cell.terrainFeature.SetCellItem(cell,ray);
        }
        else if (cell.ContainsConnection(ray, out CellConnection connection))
        {
            connection.SetTerrainFeature(terrainFeatureType);
            connection.terrainFeature.SetCellItem(connection,ray);
        }
        else if (cell.ContainsGapTriangle(ray, out CellGapTriangle gapTriangle))
        {
            gapTriangle.SetTerrainFeature(terrainFeatureType);
            gapTriangle.terrainFeature.SetCellItem(gapTriangle,ray);
        }
        
        cell.chunk.SetCellDirty(cell);
    }

   
}