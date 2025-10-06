using UnityEditor;

public static class BuildAb_Terrain
{
    public static readonly string path = "Assets/AssetBundles/Terrain";
    
    [MenuItem("Tools/Build AssetBundles/Terrain/Plain")]
    public static void BuildTerrainBundle()
    {
        BuildAssetBundles.BuildSelectedMeshesBundle(ResourceData.TerrainAbPath, nameof(TerrainType.Plain));
    }
}