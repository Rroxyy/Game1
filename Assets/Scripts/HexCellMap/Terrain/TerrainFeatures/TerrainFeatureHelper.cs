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
       

    public static void SetTerrainFeature(HexCell cell, TerrainFeatureType terrainFeatureType, Ray ray)
    {
        
        if (cell.ContainsCell(ray))
        {
            var terrainFeature = FeatureFactories[terrainFeatureType].GetTerrainFeature(CellItemType.HexCell);
            cell.SetTerrainFeature(terrainFeature);
            terrainFeature.SetCellItem(cell,ray);
        }
        else if (cell.ContainsConnection(ray, out CellConnection connection))
        {
            var terrainFeature = FeatureFactories[terrainFeatureType].GetTerrainFeature(CellItemType.Connection);
            connection.SetTerrainFeature(terrainFeature);
            terrainFeature.SetCellItem(connection,ray);
        }
        else if (cell.ContainsGapTriangle(ray, out CellGapTriangle gapTriangle))
        {
            var terrainFeature = FeatureFactories[terrainFeatureType].GetTerrainFeature(CellItemType.GapTriangle);
            gapTriangle.SetTerrainFeature(terrainFeature);
            terrainFeature.SetCellItem(gapTriangle,ray);
        }
        
        cell.chunk.SetCellDirty(cell);
    }

   
}