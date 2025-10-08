using UnityEditor;


#if UNITY_EDITOR
public static class BuildAb_Terrain
{
    
    [MenuItem("Tools/Build AssetBundles/Terrain/Plain")]
    public static void BuildTerrainBundle()
    {
        var meshes = FileHelper.GetAssetsInFolder(ResourceData.generatePainMeshPath);
        FileHelper.ClearFolder(ResourceData.TerrainAbPath);
        BuildAssetBundleHelper.BuildAssetBundle(ResourceData.TerrainAbPath, nameof(TerrainType.Plain),meshes);
    }
}
#endif