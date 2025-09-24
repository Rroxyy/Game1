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

    public static TerrainFeature GetTerrainFeature(TerrainFeatureType terrainFeatureType, Cell_Item item)
        => FeatureFactories[terrainFeatureType].GetTerrainFeature(item);

    public static void SetTerrainFeature(HexCell cell, TerrainFeatureType terrainFeatureType, Ray ray)
    {
        
        if (cell.ContainsCell(ray))
        {
            cell.SetTerrainFeature(terrainFeatureType);
            cell.terrainFeature.AddTerrainFeatureParams(ray);
        }
        else if (cell.ContainsConnection(ray, out CellConnection connection))
        {
            connection.SetTerrainFeature(terrainFeatureType);
            connection.terrainFeature.AddTerrainFeatureParams(ray);
        }
        else if (cell.ContainsGapTriangle(ray, out CellGapTriangle gapTriangle))
        {
            gapTriangle.SetTerrainFeature(terrainFeatureType);
            gapTriangle.terrainFeature.AddTerrainFeatureParams(ray);
        }
        
        cell.chunk.SetCellDirty(cell);
    }

   
}