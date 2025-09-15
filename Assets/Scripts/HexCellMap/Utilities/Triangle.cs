using UnityEngine;

public class Triangle
{
    public static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        // 使用同向叉积法，判断点是否在三角形内部
        Vector3 ab = b - a;
        Vector3 bc = c - b;
        Vector3 ca = a - c;

        Vector3 ap = p - a;
        Vector3 bp = p - b;
        Vector3 cp = p - c;

        if (Vector3.Dot(Vector3.Cross(ab, ap), Vector3.Cross(ab, bc)) < 0) return false;
        if (Vector3.Dot(Vector3.Cross(bc, bp), Vector3.Cross(bc, ca)) < 0) return false;
        if (Vector3.Dot(Vector3.Cross(ca, cp), Vector3.Cross(ca, ab)) < 0) return false;

        return true;
    }

    public static bool RayInTriangle(Ray ray, Vector3 a, Vector3 b, Vector3 c, out float t)
    {
        t = 0f;

        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;

        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float det = Vector3.Dot(edge1, h);

        // 射线与三角形平行
        if (Mathf.Abs(det) < Mathf.Epsilon)
            return false;

        float invDet = 1.0f / det;
        Vector3 s = ray.origin - a;
        float u = Vector3.Dot(s, h) * invDet;

        if (u < 0f || u > 1f)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = Vector3.Dot(ray.direction, q) * invDet;

        if (v < 0f || u + v > 1f)
            return false;

        // 计算射线交点到 origin 的距离 t
        t = Vector3.Dot(edge2, q) * invDet;

        // 如果 t < 0，交点在射线反方向上
        if (t < 0f)
            return false;

        return true;
    }

}