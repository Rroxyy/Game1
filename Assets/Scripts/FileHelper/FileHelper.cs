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
}

#endif