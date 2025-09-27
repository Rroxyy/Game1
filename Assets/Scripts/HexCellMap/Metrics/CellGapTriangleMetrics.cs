using System.Collections.Generic;
using UnityEngine;

public static class CellGapTriangleMetrics
{
    public static readonly float CellGapTriangleRadius = CellConnectionMetrics.GapX;

    public static readonly float CellGapTriangleCircumRadius = CellGapTriangleRadius / Mathf.Sqrt(3f);

    public static readonly List<Vector3> CellGapTriangleCorners = new List<Vector3>()
    {
        // 从右下角开始，顺时针
        new Vector3(
            CellGapTriangleCircumRadius * Mathf.Cos(-30f * Mathf.Deg2Rad),
            0f,
            CellGapTriangleCircumRadius * Mathf.Sin(-30f * Mathf.Deg2Rad)
        ),
        new Vector3(
            CellGapTriangleCircumRadius * Mathf.Cos(-150f * Mathf.Deg2Rad),
            0f,
            CellGapTriangleCircumRadius * Mathf.Sin(-150f * Mathf.Deg2Rad)
        ),
        new Vector3(
            CellGapTriangleCircumRadius * Mathf.Cos(-270f * Mathf.Deg2Rad),
            0f,
            CellGapTriangleCircumRadius * Mathf.Sin(-270f * Mathf.Deg2Rad)
        ),
    };
}