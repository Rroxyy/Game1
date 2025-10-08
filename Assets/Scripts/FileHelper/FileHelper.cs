using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public static class FileHelper
{
    public static void SaveAsset(Object obj, string path)
    {
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Mesh 已保存到 {path}");
    }

   
    
    public static void ClearFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"⚠️ 目录不存在：{folderPath}");
            return;
        }

        // 删除所有文件
        string[] files = Directory.GetFiles(folderPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        // 删除所有子文件夹
        string[] dirs = Directory.GetDirectories(folderPath);
        foreach (string dir in dirs)
        {
            Directory.Delete(dir, true); // 第二个参数 true 表示递归删除
        }

        Debug.Log($"✅ 已清空文件夹：{folderPath}");
    }
    
    public static List<Object> GetAssetsInFolder(string folderPath)
    {
        List<Object> assets = new List<Object>();

        // 查找该目录下的所有资源（t: 表示所有类型）
        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 保证只查当前文件夹（不递归）
            string assetDir = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            if (assetDir != folderPath.TrimEnd('/'))
                continue;

            // 加载资源为 UnityEngine.Object
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (obj != null)
                assets.Add(obj);
        }

        return assets;
    }
}

#endif