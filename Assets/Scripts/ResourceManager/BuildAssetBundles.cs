using UnityEditor;
using System.IO;
using UnityEngine;

public class BuildAssetBundles
{
    public static void BuildSelectedMeshesBundle(string outputPath,string bundleName)
    {
        Debug.Log("------------Build AssetBundles ---------------");
        Debug.Log("Output Path: "+outputPath+", abName: "+bundleName);
        
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("⚠️ 没有选中任何资源，请在 Project 视图中选中需要打包的 Mesh 文件（如 .fbx / .obj / .asset）");
            return;
        }

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                continue;

            // 给每个选中的资源设置同一个 AssetBundle 名称
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.assetBundleName = bundleName;
                Debug.Log($"✅ 已添加到打包列表: {path}");
            }
        }

        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // 执行打包
        BuildPipeline.BuildAssetBundles(
            outputPath,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget
        );

        Debug.Log($"AB包已生成到: {outputPath}/{bundleName}");
    }
}