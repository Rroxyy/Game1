



using UnityEngine;

public class PlainTerrainData : TerrainData
{
    public override TerrainType terrainType  => TerrainType.Plain;
    public Vector3 normal = Vector3.up;


}