using System.IO;
using System.Text;
using UnityEngine;

public static class MeshExporter
{
    public static void SaveMeshAsOBJ(Mesh mesh, string folderPath)
    {
        if (mesh == null)
        {
            Debug.LogWarning("⚠️ Mesh 为空，无法导出！");
            return;
        }

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("⚠️ 保存路径为空！");
            return;
        }

        // 确保目录存在
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, mesh.name + ".obj");

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("# Exported by MeshExporter");
        sb.AppendLine($"# Mesh name: {mesh.name}");
        sb.AppendLine();

        // 顶点
        foreach (Vector3 v in mesh.vertices)
        {
            sb.AppendLine($"v {v.x} {v.y} {v.z}");
        }

        sb.AppendLine();

        // 法线
        foreach (Vector3 n in mesh.normals)
        {
            sb.AppendLine($"vn {n.x} {n.y} {n.z}");
        }

        sb.AppendLine();

        // 三角形（OBJ 的索引从 1 开始）
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i] + 1;
            int v2 = triangles[i + 1] + 1;
            int v3 = triangles[i + 2] + 1;

            // 这里强制写 v//vn 格式，不使用 uv
            sb.AppendLine($"f {v1}//{v1} {v2}//{v2} {v3}//{v3}");
        }

        File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"✅ Mesh 已导出到: {fullPath}");
    }
}