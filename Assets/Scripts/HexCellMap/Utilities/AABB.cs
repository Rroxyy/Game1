using System;
using UnityEngine;  // 引入 Unity 的 Vector3

// 泛型 AABB（修改为使用 Vector3 和 float）
[System.Serializable]
public class AABB
{
    public Vector3 min;
    public Vector3 max;
    public Vector3 center => (this.min + this.max) / 2f;
    public Vector3 size => (this.max - this.min);

    public AABB()
    {
        min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }
    
    public AABB(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }

    public void Reset()
    {
        min.x = float.PositiveInfinity;
        min.y = float.PositiveInfinity;
        min.z = float.PositiveInfinity;

        max.x = float.NegativeInfinity;
        max.y = float.NegativeInfinity;
        max.z = float.NegativeInfinity;
    }


    // 更新 AABB，使其包含新的点
    public void Encapsulate(Vector3 point)
    {
        min = Vector3.Min(min, point);
        max = Vector3.Max(max, point);
    }

    // 更新 AABB，使其包含另一个 AABB
    public void Encapsulate(AABB other)
    {
        min = Vector3.Min(min, other.min);
        max = Vector3.Max(max, other.max);
    }

    // 合并两个 AABB
    public static AABB Merge(AABB a, AABB b)
    {
        return new AABB(
            Vector3.Min(a.min, b.min),
            Vector3.Max(a.max, b.max)
        );
    }

    // 判断点是否在 AABB 内
    public bool Contains(Vector3 point)
    {
        return (point.x >= min.x && point.x <= max.x) &&
               (point.y >= min.y && point.y <= max.y) &&
               (point.z >= min.z && point.z <= max.z);
    }

    public bool Contains(Ray ray)
    {
        // AABB 最小值和最大值
        Vector3 invDir = new Vector3(
            1f / ray.direction.x,
            1f / ray.direction.y,
            1f / ray.direction.z
        );

        Vector3 deltaMin = min - ray.origin;
        Vector3 deltaMax = max - ray.origin;

        Vector3 t1 = new Vector3(deltaMin.x * invDir.x, deltaMin.y * invDir.y, deltaMin.z * invDir.z);
        Vector3 t2 = new Vector3(deltaMax.x * invDir.x, deltaMax.y * invDir.y, deltaMax.z * invDir.z);


        // 每个轴上的 tMin 和 tMax
        float tMin = Mathf.Max(
            Mathf.Max(Mathf.Min(t1.x, t2.x), Mathf.Min(t1.y, t2.y)),
            Mathf.Min(t1.z, t2.z)
        );

        float tMax = Mathf.Min(
            Mathf.Min(Mathf.Max(t1.x, t2.x), Mathf.Max(t1.y, t2.y)),
            Mathf.Max(t1.z, t2.z)
        );

        // 如果 tMax < 0 → AABB 在射线背后
        // 如果 tMin > tMax → 没有交点
        if (tMax < 0 || tMin > tMax)
            return false;

        return true;
    }


    public static bool Contains(AABB a, Vector3 point)
    {
        return (point.x >= a.min.x && point.x <= a.max.x) &&
               (point.y >= a.min.y && point.y <= a.max.y) &&
               (point.z >= a.min.z && point.z <= a.max.z);
    }
    
    // 判断在每个轴向上是否有间隙，如果有间隙则不重叠
    public bool Overlaps(AABB other)
    {
        if (max.x < other.min.x || min.x > other.max.x) return false;
        if (max.y < other.min.y || min.y > other.max.y) return false;
        if (max.z < other.min.z || min.z > other.max.z) return false;

        return true;
    }

    // 静态版本
    public static bool Overlaps(AABB a, AABB b)
    {
        if (a.max.x < b.min.x || a.min.x > b.max.x) return false;
        if (a.max.y < b.min.y || a.min.y > b.max.y) return false;
        if (a.max.z < b.min.z || a.min.z > b.max.z) return false;
        return true;
    }


    public override string ToString() => $"Min: {min}, Max: {max}";
}




[System.Serializable]
public class AABB_Int
{
    public Vector3Int min { get; private set; }
    public Vector3Int max { get; private set; }
    public Vector3Int center => (this.min + this.max) / 2;
    public float size => (this.max - this.min).magnitude; 
    public AABB_Int(Vector3Int min, Vector3Int max)
    {
        this.min = min;
        this.max = max;
    }

    // 更新 AABB，使其包含新的点
    public void Encapsulate(Vector3Int point)
    {
        min = Vector3Int.Min(min, point);
        max = Vector3Int.Max(max, point);
    }

    // 更新 AABB，使其包含另一个 AABB
    public void Encapsulate(AABB_Int other)
    {
        min = Vector3Int.Min(min, other.min);
        max = Vector3Int.Max(max, other.max);
    }

    // 合并两个 AABB
    public static AABB_Int Merge(AABB_Int a, AABB_Int b)
    {
        return new AABB_Int(
            Vector3Int.Min(a.min, b.min),
            Vector3Int.Max(a.max, b.max)
        );
    }

    // 判断某个点是否在 AABB 内
    public bool Contains(Vector3Int point)
    {
        return (point.x >= min.x && point.x <= max.x) &&
               (point.y >= min.y && point.y <= max.y) &&
               (point.z >= min.z && point.z <= max.z);
    }

    // 静态方法，判断某个点是否在 AABB 内
    public static bool Contains(AABB_Int a, Vector3Int point)
    {
        return (point.x >= a.min.x && point.x <= a.max.x) &&
               (point.y >= a.min.y && point.y <= a.max.y) &&
               (point.z >= a.min.z && point.z <= a.max.z);
    }
    
    // 判断在每个轴向上是否有间隙，如果有间隙则不重叠
    public bool Overlaps(AABB_Int other)
    {
        if (max.x < other.min.x || min.x > other.max.x) return false;
        if (max.y < other.min.y || min.y > other.max.y) return false;
        if (max.z < other.min.z || min.z > other.max.z) return false;

        // 没有间隙则重叠
        return true;
    }

    // 静态版本
    public static bool Overlaps(AABB_Int a, AABB_Int b)
    {
        if (a.max.x < b.min.x || a.min.x > b.max.x) return false;
        if (a.max.y < b.min.y || a.min.y > b.max.y) return false;
        if (a.max.z < b.min.z || a.min.z > b.max.z) return false;
        return true;
    }

    public override string ToString() => $"Min: {min}, Max: {max}";
}
