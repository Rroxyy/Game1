using System.IO;
using System.Text;
using UnityEngine;

public class MeshExporter
{
    public static void SaveMeshAsOBJ(Mesh mesh, string path)
    {
        StringBuilder sb = new StringBuilder();

        // 顶点
        foreach (Vector3 v in mesh.vertices)
        {
            sb.AppendLine($"v {v.x} {v.y} {v.z}");
        }

        // 法线
        foreach (Vector3 n in mesh.normals)
        {
            sb.AppendLine($"vn {n.x} {n.y} {n.z}");
        }

        // 三角形（索引+1，因为 OBJ 文件索引从 1 开始）
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            sb.AppendLine($"f {triangles[i] + 1}//{triangles[i] + 1} {triangles[i + 1] + 1}//{triangles[i + 1] + 1} {triangles[i + 2] + 1}//{triangles[i + 2] + 1}");
        }

        File.WriteAllText(path, sb.ToString());
        Debug.Log($"Mesh 已导出到 {path}");
    }
}