using UnityEngine;

public static class Quad
{
    public static bool RayInQuad(Ray ray, Vector3 a, Vector3 b, Vector3 c, Vector3 d, out float t)
    {
        t = 0f;
        float t1, t2;

        // 检测三角形 abc
        if (Triangle.RayInTriangle(ray, a, b, c, out t1))
        {
            t = t1;
            return true;
        }

        // 检测三角形 acd
        if (Triangle.RayInTriangle(ray, a, c, d, out t2))
        {
            t = t2;
            return true;
        }

        return false;
    }
}