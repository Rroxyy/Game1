using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

public class BuildAssetBundleHelper
{
    public static void BuildAssetBundle(string outputPath,string bundleName)
    {
        Debug.Log("------------Build AssetBundles ---------------");
        Debug.Log("Output Path: "+outputPath+", abName: "+bundleName);
        
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        BuildAssetBundle(outputPath, bundleName, selectedObjects);
    }

    public static void BuildAssetBundle(string outputPath, string bundleName, List<Object> selectedObjects)
    {
        BuildAssetBundle(outputPath, bundleName, selectedObjects.ToArray());
    }
    
    public static void BuildAssetBundle(string outputPath, string bundleName, Object[] selectedObjects)
    {
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("⚠️ 没有选中任何资源");
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